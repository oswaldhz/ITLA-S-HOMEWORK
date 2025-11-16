using ProductApi.Interfaces;
using ProductApi.Models;

namespace ProductApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products;
        private int _nextId = 1;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ILogger<ProductRepository> logger)
        {
            _products = new List<Product>();
            _logger = logger;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Datos de ejemplo
            var sampleProducts = new[]
            {
                new Product { Id = _nextId++, Name = "Laptop Gaming", Description = "Laptop para gaming", Price = 1200.99m, Stock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Mouse Inalámbrico", Description = "Mouse ergonómico", Price = 49.99m, Stock = 25, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Teclado Mecánico", Description = "Teclado RGB", Price = 89.99m, Stock = 15, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
            };

            _products.AddRange(sampleProducts);
            _logger.LogInformation("Sample data initialized with {Count} products", _products.Count);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogInformation("Getting all products");
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = _nextId++;
            product.CreatedDate = DateTime.UtcNow;
            product.UpdatedDate = DateTime.UtcNow;

            _products.Add(product);
            _logger.LogInformation("Created new product with ID: {ProductId}", product.Id);

            return Task.FromResult(product);
        }

        public Task<Product?> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Stock = product.Stock;
                existing.UpdatedDate = DateTime.UtcNow;

                _logger.LogInformation("Updated product with ID: {ProductId}", product.Id);
            }
            else
            {
                _logger.LogWarning("Product with ID: {ProductId} not found for update", product.Id);
            }

            return Task.FromResult(existing);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                _logger.LogInformation("Deleted product with ID: {ProductId}", id);
                return Task.FromResult(true);
            }

            _logger.LogWarning("Product with ID: {ProductId} not found for deletion", id);
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return Task.FromResult(_products.Any(p => p.Id == id));
        }
    }
}