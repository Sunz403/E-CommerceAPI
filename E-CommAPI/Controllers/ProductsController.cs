using E_CommAPI.Models;
using E_CommAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IApiKeyService _apiKeyService;

        public ProductsController(IProductService productService, IApiKeyService apiKeyService)
        {
            _productService = productService;
            _apiKeyService = apiKeyService;
        }

        [HttpGet]
        [Produces("application/xml")]
        public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] string? category)
        {
            var products = await _productService.GetProductsAsync(search, category);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [Produces("application/xml")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Produces("application/xml")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();
            if (user?.Role != "Admin")
                return Unauthorized("Admin access required");

            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [Produces("application/xml")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetCurrentUserAsync();
            if (user?.Role != "Admin")
                return Unauthorized("Admin access required");

            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Produces("application/xml")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user?.Role != "Admin")
                return Unauthorized("Admin access required");

            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            return await _apiKeyService.ValidateApiKeyAsync(apiKey);
        }
    }

}
