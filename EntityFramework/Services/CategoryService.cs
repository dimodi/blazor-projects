using Microsoft.EntityFrameworkCore;
using TelerikBlazorEF.Data;

namespace TelerikBlazorEF.Services
{
    public class CategoryService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public CategoryService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Categories.ToList();
        }

        public async Task<int> CreateCategoryAsync(Category newCategory)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Categories.Add(newCategory);
            await dbContext.SaveChangesAsync();

            return newCategory.Id;
        }

        public async Task UpdateCategoryAsync(Category updatedCategory)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Category? originalCategory = await dbContext.FindAsync<Category>(updatedCategory.Id);

            if (originalCategory != null)
            {
                dbContext.Entry(originalCategory).CurrentValues.SetValues(updatedCategory);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Category? categoryToDelete = dbContext.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (categoryToDelete != null)
            {
                dbContext.Categories.Remove(categoryToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task GenerateData(int categoryCount = 9)
        {
            var wordGenerator = new NameGenerator();

            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            if (!dbContext.Categories.Any())
            {
                for (int i = 1; i <= categoryCount; i++)
                {
                    dbContext.Categories.Add(new Category()
                    {
                        Id = i,
                        Name = $"{wordGenerator.Word(5)} {i}"
                    });
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearData()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Categories.RemoveRange(dbContext.Categories);

            await dbContext.SaveChangesAsync();
        }
    }
}
