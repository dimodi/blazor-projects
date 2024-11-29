using Microsoft.EntityFrameworkCore;
using TelerikBlazorEF.Data;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace TelerikBlazorEF.Services
{
    public class ProductService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public ProductService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private DbContextEF? QueryableContext { get; set; }

        public async Task<List<Product>> GetProductsAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Products.Include(p => p.Category).ToList();
        }

        public async Task<DataSourceResult> GetProductsAsync(DataSourceRequest request)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

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
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.Add(newProduct);
            await dbContext.Entry(newProduct).Reference(c => c.Category).LoadAsync();
            await dbContext.SaveChangesAsync();

            return newProduct.Id;
        }

        public async Task UpdateProductAsync(Product updatedProduct)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Product? originalProduct = await dbContext.FindAsync<Product>(updatedProduct.Id);

            if (originalProduct != null)
            {
                dbContext.Entry(originalProduct).CurrentValues.SetValues(updatedProduct);
                await dbContext.Entry(updatedProduct).Reference(c => c.Category).LoadAsync();
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(Product deletedProduct)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.Remove(deletedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task GenerateData(int productCount = 123)
        {
            var wordGenerator = new NameGenerator();

            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

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
                        ReleaseDate = DateTime.Today
                            .AddYears(-Random.Shared.Next(1, 10))
                            .AddMonths(Random.Shared.Next(1, 12))
                            .AddDays(Random.Shared.Next(1, 30))
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearData()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.RemoveRange(dbContext.Products);

            await dbContext.SaveChangesAsync();
        }
    }
}
