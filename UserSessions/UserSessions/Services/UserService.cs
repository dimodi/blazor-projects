using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using UserSessions.Data;

namespace UserSessions.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public UserService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        internal async Task<List<User>> GetUsersAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Users.ToList();
        }

        public async Task<int?> GetUserIdByCredentials(string email, string password)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? user = dbContext.Users.FirstOrDefault(x => x.Email == email && x.Active);

            if (user != null && user.Password == GetEncryptedPassword(password, user.Salt))
            {
                return user.Id;
            }

            return default;
        }

        internal async Task<bool> CheckIfUserEmailExists(string email)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? user = dbContext.Users.FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                return true;
            }

            return false;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Users.FirstOrDefault(x => x.Id == id && x.Active);
        }

        internal async Task<bool> GetUserActiveAsync(int userId)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Users.First(x => x.Id == userId).Active;
        }

        internal async Task<int> CreateUserAsync(User newUser)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            newUser.Password = GetEncryptedPassword(newUser.Password, newUser.Salt);
            newUser.ActivationToken = Guid.NewGuid().ToString();
            newUser.ActivationTokenDeadline = DateTime.Now.AddDays(7);

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            return newUser.Id;
        }

        internal async Task<(User?, bool)> ActivateUserAsync(int id, string token)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? userToActivate = await dbContext.FindAsync<User>(id);

            bool activationSuccess = false;

            if (userToActivate != null)
            {
                if (userToActivate.ActivationToken == token && !userToActivate.Active && userToActivate.ActivationTokenDeadline >= DateTime.Now)
                {
                    userToActivate.Active = true;
                    userToActivate.ActivationToken = string.Empty;
                    userToActivate.ActivationTokenDeadline = null;

                    await dbContext.SaveChangesAsync();

                    activationSuccess = true;
                }

                return (userToActivate, activationSuccess);
            }

            return default;
        }

        internal async Task UpdateUserAsync(User updatedUser)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? originalUser = await dbContext.FindAsync<User>(updatedUser.Id);

            if (originalUser != null)
            {
                updatedUser.Password = originalUser.Password;
                updatedUser.Salt = originalUser.Salt;

                if (updatedUser.Email != originalUser.Email || !updatedUser.Active && originalUser.Active)
                {
                    updatedUser.Active = false;
                    updatedUser.ActivationToken = Guid.NewGuid().ToString();
                    updatedUser.ActivationTokenDeadline = DateTime.Now.AddDays(7);
                }

                dbContext.Entry(originalUser).CurrentValues.SetValues(updatedUser);
                await dbContext.SaveChangesAsync();
            }
        }

        internal async Task UpdateUserRestrictedAsync(User updatedUser)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? originalUser = await dbContext.FindAsync<User>(updatedUser.Id);

            if (originalUser != null)
            {
                originalUser.Email = updatedUser.Email;
                originalUser.Name = updatedUser.Name;
                await dbContext.SaveChangesAsync();
            }
        }

        internal async Task<bool> UpdateUserPaswordAsync(int userId, string originalPassword, string updatedPassword)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            User? originalUser = dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (originalUser != null && originalUser.Password == GetEncryptedPassword(originalPassword, originalUser.Salt))
            {
                originalUser.Salt = Guid.NewGuid().ToString();
                originalUser.Password = GetEncryptedPassword(updatedPassword, originalUser.Salt);
                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        internal async Task DeleteUserAsync(User deletedUser)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Users.Remove(deletedUser);
            await dbContext.SaveChangesAsync();
        }

        private static string GetEncryptedPassword(string plainText, string salt)
        {
            string saltedPassword = string.Concat(plainText, salt);

            return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(saltedPassword)));
        }
    }
}
