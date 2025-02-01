using Microsoft.EntityFrameworkCore;
using UserSessions.Data;

namespace UserSessions.Services
{
    public class SessionService : ISessionService
    {
        private readonly IDbContextFactory<DbContextEF> _contextFactory;

        public SessionService(IDbContextFactory<DbContextEF> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        internal async Task<List<Session>> GetSessionsAsync()
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            return dbContext.Sessions.ToList();
        }

        internal async Task<(int, bool, string)> GetSessionDetailsAsync(string cookieValue)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            (int userId, string token) = CustomAuthStateProvider.SplitCookiValue(cookieValue);
            Session? oldSession = await dbContext.Sessions.FirstOrDefaultAsync(x => x.Token == token && x.UserId == userId);
            string newToken = string.Empty;

            if (oldSession != null)
            {
                if (oldSession.Expires > DateTime.Now)
                {
                    newToken = Guid.NewGuid().ToString();
                    dbContext.Sessions.Add(new()
                    {
                        Expires = CustomAuthStateProvider.GetSessionExpiration(oldSession.Pesistent),
                        Pesistent = oldSession.Pesistent,
                        Token = newToken,
                        UserId = userId
                    });
                }

                dbContext.Sessions.Remove(oldSession);
                await dbContext.SaveChangesAsync();

                return (userId, oldSession.Pesistent, newToken);
            }

            return default;
        }

        public async Task CreateSessionAsync(Session newSession)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Sessions.Add(newSession);
            await dbContext.SaveChangesAsync();
        }

        internal async Task DeleteSessionAsync(Session deletedSession)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            dbContext.Sessions.Remove(deletedSession);
            await dbContext.SaveChangesAsync();
        }

        internal async Task DeleteSessionAsync(string token)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            Session? tokenToDelete = await dbContext.Sessions.FirstOrDefaultAsync(x => x.Token == token);

            if (tokenToDelete != null)
            {
                dbContext.Sessions.Remove(tokenToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        internal async Task DeleteUserSessionsAsync(int userId)
        {
            using DbContextEF dbContext = await _contextFactory.CreateDbContextAsync();

            IEnumerable<Session> sessionsToDelete = dbContext.Sessions.Where(x => x.UserId == userId);

            dbContext.Sessions.RemoveRange(sessionsToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}
