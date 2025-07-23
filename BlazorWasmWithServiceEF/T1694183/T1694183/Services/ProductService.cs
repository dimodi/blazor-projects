using Microsoft.EntityFrameworkCore;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
using T1694183.Data;
using T1694183.Shared.Data;
using System.Reflection;

namespace T1694183.Services
{
    public class ProductService : IProductService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public ProductService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Products.ToList();
        }

        public async Task<DataSourceResult> GetProductsAsync(DataSourceRequest request)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            if (dbContext.Products.Count() == 0)
            {
                await GenerateData(dbContext);
            }

            if (request.Filters.Count > 0)
            {
                FixDeserializedFilterDescriptors(request.Filters);
            }

            return await dbContext.Products.ToDataSourceResultAsync(request);
        }

        private void FixDeserializedFilterDescriptors(IList<IFilterDescriptor> filterDescriptors)
        {
            Type productType = typeof(Product);

            foreach (IFilterDescriptor ifd in filterDescriptors)
            {
                if (ifd is FilterDescriptor fd)
                {
                    // Type values are lost during serialization
                    // https://www.telerik.com/blazor-ui/documentation/knowledge-base/grid-json-serializer-null-membertype
                    fd.MemberType = productType.GetProperty(fd.Member)?.PropertyType;

                    // DateOnly is deserialized as DateTime
                    if (fd.Member == nameof(Product.ReleaseDateOnly) && fd.Value is DateTime)
                    {
                        //fd.Value = DateOnly.FromDateTime(DateTime.Parse(fd.Value.ToString()!));
                    }
                }
                else if (ifd is CompositeFilterDescriptor cfd)
                {
                    FixDeserializedFilterDescriptors(cfd.FilterDescriptors);
                }
            }
        }

        public async Task<int> CreateProductAsync(Product newProduct)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.Add(newProduct);
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
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(Product deletedProduct)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.Remove(deletedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task GenerateData(DbContextEF dbContext, int productCount = 123)
        {
            for (int i = 1; i <= productCount; i++)
            {
                DateTime releaseDate = DateTime.Today
                        .AddYears(-Random.Shared.Next(1, 10))
                        .AddMonths(Random.Shared.Next(1, 12))
                        .AddDays(Random.Shared.Next(1, 30));

                dbContext.Products.Add(new Product()
                {
                    Id = i,
                    Discontinued = i % 5 == 0,
                    Name = $"Product {i}",
                    Price = Random.Shared.Next(1, 100),
                    Quantity = Random.Shared.Next(0, 1000),
                    ReleaseDateOnly = DateOnly.FromDateTime(releaseDate),
                    ReleaseDateTime = releaseDate
                });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task ClearData()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Products.RemoveRange(dbContext.Products);

            await dbContext.SaveChangesAsync();
        }
    }
}
