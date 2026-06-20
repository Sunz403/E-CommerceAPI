using E_CommAPI.Data;
using E_CommAPI.Models;
using E_CommAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_CommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly ECommerceContext _context;

        public AuthController(IApiKeyService apiKeyService, ECommerceContext context)
        {
            _apiKeyService = apiKeyService;
            _context = context;
        }

        [HttpPost("register")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
                return BadRequest("Username or email already exists");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var apiKey = await _apiKeyService.GenerateApiKeyAsync(user.Id, user.Role);
            return Ok(new { ApiKey = apiKey, UserId = user.Id, Role = user.Role });
        }

        [HttpPost("login")]
        [AllowAnonymous]// No API key required
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
             u.Username == request.Username &&
             u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // Generate new API key
            var newApiKey = await _apiKeyService.GenerateApiKeyAsync(user.Id, user.Role);

            return Ok(new
            {
                ApiKey = newApiKey,
                UserId = user.Id,
                Role = user.Role
            });
        }
        
        [HttpPost("revoke")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> RevokeApiKey()
        {
            var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
                return BadRequest("API key is required");

            var result = await _apiKeyService.RevokeApiKeyAsync(apiKey);
            if (!result)
                return NotFound("API key not found");

            return Ok("API key revoked successfully");
        }

        // Helper method to get current user
        private async Task<User?> GetCurrentUserAsync()
        {
            var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            return await _apiKeyService.ValidateApiKeyAsync(apiKey);
        }

        [HttpPost("reset-key")]
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> ResetApiKey()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            var newKey = await _apiKeyService.GenerateApiKeyAsync(user.Id, user.Role);
            return Ok(new { ApiKey = newKey });
        }
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
