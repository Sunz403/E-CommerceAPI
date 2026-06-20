using E_CommAPI.Data;
using E_CommAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync(string? search = null, string? category = null);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<List<Product>> GetRecommendedProductsAsync(string userQuery);
    }

    public class ProductService : IProductService
    {
        private readonly ECommerceContext _context;

        public ProductService(ECommerceContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync(string? search = null, string? category = null)
        {
            var query = _context.Products.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.Category = product.Category;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetRecommendedProductsAsync(string userQuery)
        {
            var query = userQuery.ToLower();
            return await _context.Products
                .Where(p => p.IsActive &&
                           (p.Name.ToLower().Contains(query) ||
                            p.Description.ToLower().Contains(query) ||
                            p.Category.ToLower().Contains(query)))
                .OrderByDescending(p => p.Category == query)
                .ThenBy(p => p.Name)
                .Take(5)
                .ToListAsync();
        }
    }
}
