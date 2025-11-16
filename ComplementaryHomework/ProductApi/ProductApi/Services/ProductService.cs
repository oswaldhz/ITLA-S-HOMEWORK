using ProductApi.Interfaces;
using ProductApi.Models;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Retrieving all products");
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock
            };

            _logger.LogInformation("Creating new product: {ProductName}", productDto.Name);
            return await _repository.CreateAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(int id, ProductDto productDto)
        {
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found for update", id);
                return null;
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.Stock = productDto.Stock;

            _logger.LogInformation("Updating product with ID: {ProductId}", id);
            return await _repository.UpdateAsync(existingProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);
            return await _repository.DeleteAsync(id);
        }
    }
}