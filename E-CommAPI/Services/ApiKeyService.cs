using E_CommAPI.Data;
using E_CommAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommAPI.Services
{
    public interface IApiKeyService
    {
        Task<User?> ValidateApiKeyAsync(string apiKey);
        Task<string> GenerateApiKeyAsync(int userId, string role);
        Task<bool> RevokeApiKeyAsync(string apiKey);
    }

    public class ApiKeyService : IApiKeyService
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;

        public ApiKeyService(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> ValidateApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ApiKey == apiKey && u.IsActive && u.ApiKeyExpiry > DateTime.UtcNow);

            return user;
        }

        public async Task<string> GenerateApiKeyAsync(int userId, string role)
        {
            var apiKey = GenerateSecureApiKey();
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.ApiKey = apiKey;
                user.ApiKeyExpiry = DateTime.UtcNow.AddYears(1);
                user.Role = role;
                await _context.SaveChangesAsync();
            }

            return apiKey;
        }

        public async Task<bool> RevokeApiKeyAsync(string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user != null)
            {
                user.ApiKey = string.Empty;
                user.ApiKeyExpiry = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private string GenerateSecureApiKey()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }
    }
}
