using UserSessions.Data;

namespace UserSessions.Services
{
    public interface ISessionService
    {
        public Task CreateSessionAsync(Session newSession);
    }
}
