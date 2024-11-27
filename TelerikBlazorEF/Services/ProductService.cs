using Microsoft.EntityFrameworkCore;
using TelerikBlazorEF.Data;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace TelerikBlazorEF.Services
{
    public class ProductService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        private DbContextEF? QueryableContext { get; set; }

        public async Task<List<Product>> GetProductsAsync()
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Products.Include(p => p.Category).ToList();
        }

        public async Task<DataSourceResult> GetProductsAsync(DataSourceRequest request)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            return await dbContext.Products.ToDataSourceResultAsync(request);
        }

        public async Task<IQueryable<Product>> GetProductsAsyncAsQueryable()
        {
            if (QueryableContext == null)
            {
                QueryableContext = await _contextFactory.CreateDbContextAsync();
            }

            return QueryableContext.Products.AsQueryable();
        }

        public async Task<int> CreateProductAsync(Product newProduct)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.Add(newProduct);
            await dbContext.SaveChangesAsync();

            return newProduct.Id;
        }

        public async Task UpdateProductAsync(Product updatedProduct)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            Product? originalProduct = await dbContext.FindAsync<Product>(updatedProduct.Id);

            if (originalProduct != null)
            {
                dbContext.Entry(originalProduct).State = EntityState.Detached;
                await dbContext.Entry(updatedProduct).Reference(c => c.Category).LoadAsync();
                dbContext.Update(updatedProduct);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(Product product)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            Product? productToDelete = dbContext.Products.FirstOrDefault(x => x.Id == product.Id);

            if (productToDelete != null)
            {
                dbContext.Products.Remove(productToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public ProductService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task GenerateData(int productCount = 123)
        {
            var wordGenerator = new NameGenerator();

            using var dbContext = await _contextFactory.CreateDbContextAsync();

            if (!dbContext.Products.Any() && dbContext.Categories.Any())
            {
                var categoryIds = dbContext.Categories.Select(x => x.Id).ToList();

                for (int i = 1; i <= productCount; i++)
                {
                    dbContext.Products.Add(new Product()
                    {
                        Id = i,
                        CategoryId = categoryIds.ElementAt(Random.Shared.Next(0, categoryIds.Count)),
                        Discontinued = i % 5 == 0,
                        Name = $"{wordGenerator.Word(5, 9)} {i}",
                        Price = Random.Shared.Next(1, 100),
                        Quantity = Random.Shared.Next(0, 1000),
                        ReleaseDate = DateTime.Today.AddYears(-Random.Shared.Next(1, 10)).AddMonths(Random.Shared.Next(1, 12)).AddDays(Random.Shared.Next(1, 30))
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearData()
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.RemoveRange(dbContext.Products);

            await dbContext.SaveChangesAsync();
        }
    }
}
