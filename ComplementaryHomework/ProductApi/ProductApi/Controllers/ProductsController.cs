using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Product>>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(new ApiResponse<IEnumerable<Product>>(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new ApiResponse<string>(null, "Internal server error", false));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new ApiResponse<string>(null, $"Product with ID {id} not found", false));
                }

                return Ok(new ApiResponse<Product>(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                return StatusCode(500, new ApiResponse<string>(null, "Internal server error", false));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Product>>> CreateProduct(ProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<string>(null, "Invalid product data", false));
                }

                var createdProduct = await _productService.CreateProductAsync(productDto);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id },
                    new ApiResponse<Product>(createdProduct, "Product created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new ApiResponse<string>(null, "Internal server error", false));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> UpdateProduct(int id, ProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<string>(null, "Invalid product data", false));
                }

                var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
                if (updatedProduct == null)
                {
                    return NotFound(new ApiResponse<string>(null, $"Product with ID {id} not found", false));
                }

                return Ok(new ApiResponse<Product>(updatedProduct, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(500, new ApiResponse<string>(null, "Internal server error", false));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (!deleted)
                {
                    return NotFound(new ApiResponse<string>(null, $"Product with ID {id} not found", false));
                }

                return Ok(new ApiResponse<string>(null, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(500, new ApiResponse<string>(null, "Internal server error", false));
            }
        }
    }

    // Clase para respuestas API estandarizadas
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse(T? data, string message = "", bool success = true)
        {
            Data = data;
            Message = message;
            Success = success;
        }
    }
}