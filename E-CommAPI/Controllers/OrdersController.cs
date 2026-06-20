using E_CommAPI.Models;
using E_CommAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace E_CommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IApiKeyService _apiKeyService;

        public OrdersController(IOrderService orderService, IApiKeyService apiKeyService)
        {
            _orderService = orderService;
            _apiKeyService = apiKeyService;
        }

        [HttpGet]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> GetOrders()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var orders = user.Role == "Admin"
                ? await _orderService.GetOrdersAsync()
                : await _orderService.GetOrdersAsync(user.Id);

            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var order = user.Role == "Admin"
                ? await _orderService.GetOrderByIdAsync(id)
                : await _orderService.GetOrderByIdAsync(id, user.Id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();
            if (user == null || user.Role != "Customer")
                return Unauthorized("Customer access required");

            var order = new Order
            {
                CustomerId = user.Id,
                ShippingAddress = request.ShippingAddress,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                Status = "Pending"
            };

            var orderItems = request.OrderItems.Select(oi => new OrderItem
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
            }).ToList();

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(order, orderItems);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var user = await GetCurrentUserAsync();
            if (user?.Role != "Admin")
                return Unauthorized("Admin access required");

            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("customer/my-orders")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> GetMyOrders()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.Role != "Customer")
                return Unauthorized("Customer access required");

            var orders = await _orderService.GetCustomerOrdersAsync(user.Id);
            return Ok(orders);
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            return await _apiKeyService.ValidateApiKeyAsync(apiKey);
        }
    }

    public class CreateOrderRequest
    {
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
