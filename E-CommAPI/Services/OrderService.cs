using E_CommAPI.Data;
using E_CommAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommAPI.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersAsync(int? customerId = null);
        Task<Order?> GetOrderByIdAsync(int id, int? customerId = null);
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems);
        Task<Order?> UpdateOrderStatusAsync(int id, string status);
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
    }

    public class OrderService : IOrderService
    {
        private readonly ECommerceContext _context;

        public OrderService(ECommerceContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersAsync(int? customerId = null)
        {
            // Start with including the related entities first
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable(); // Explicitly cast to IQueryable

            // Then apply the filter
            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id, int? customerId = null)
        {
            // Start with including related entities first
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == id);

            // Then apply customer filter if provided
            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate stock and calculate total
                decimal totalAmount = 0;
                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
                    }

                    product.StockQuantity -= item.Quantity;
                    item.UnitPrice = product.Price;
                    totalAmount += product.Price * item.Quantity;
                }

                order.TotalAmount = totalAmount;
                order.OrderDate = DateTime.UtcNow;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in orderItems)
                {
                    item.OrderId = order.Id;
                    _context.OrderItems.Add(item);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return the created order with included items
                return await GetOrderByIdAsync(order.Id) ?? order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return null;

            order.Status = status;
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
