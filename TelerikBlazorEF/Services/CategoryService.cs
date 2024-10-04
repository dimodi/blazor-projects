using Microsoft.EntityFrameworkCore;
using TelerikBlazorEF.Data;

namespace TelerikBlazorEF.Services
{
    public class CategoryService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return dbContext.Categories.ToList();
            }
        }

        public async Task UpdateCategoryAsync(Category updatedCategory)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var CategoryToUpdate = dbContext.Categories.FirstOrDefault(x => x.Id == updatedCategory.Id);

                if (CategoryToUpdate != null)
                {
                    CategoryToUpdate.Name = updatedCategory.Name;

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<int> CreateCategoryAsync(Category newCategory)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                dbContext.Categories.Add(newCategory);

                await dbContext.SaveChangesAsync();

                return newCategory.Id;
            }
        }

        public async Task DeleteCategoryAsync(Category Category)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var CategoryToDelete = dbContext.Categories.FirstOrDefault(x => x.Id == Category.Id);

                if (CategoryToDelete != null)
                {
                    dbContext.Categories.Remove(CategoryToDelete);

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public CategoryService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task GenerateData(int categoryCount = 9)
        {
            var wordGenerator = new NameGenerator();

            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
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
        }

        public async Task ClearData()
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                dbContext.Categories.RemoveRange(dbContext.Categories);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
