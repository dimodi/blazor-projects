using UserSessions.Data;

namespace UserSessions.Services
{
    public interface IUserService
    {
        public Task<int?> GetUserIdByCredentials(string email, string password);

        public Task<User?> GetUserByIdAsync(int userId);
    }
}
