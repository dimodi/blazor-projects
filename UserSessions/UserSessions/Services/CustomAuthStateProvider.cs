using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using UserSessions.Data;

namespace UserSessions.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal CurrentUser { get; set; } = new(new ClaimsIdentity());

        private UserService UserService { get; set; }
        private SessionService SessionService { get; set; }

        private readonly IConfiguration Configuration;
        private string CookieName { get; set; }

        public CustomAuthStateProvider(UserService userService, SessionService sessionService, IConfiguration configuration)
        {
            UserService = userService;
            SessionService = sessionService;

            Configuration = configuration;
            CookieName = Configuration.GetValue(typeof(string), "AuthenticationCookieName")?.ToString() ?? string.Empty;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(CurrentUser));
        }

        internal void SignIn(string email, string name, Role role, bool isPersistent, int userId, string? token = null)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Hash, token ?? string.Empty),
                new Claim(ClaimTypes.SerialNumber, userId.ToString()),
                new Claim(ClaimTypes.IsPersistent, isPersistent.ToString(), ClaimValueTypes.Boolean),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Custom Authentication");

            CurrentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        internal async Task SignInFromCookie(int userId, string token, bool persistent)
        {
            User? user = await UserService.GetUserByIdAsync(userId);

            if (user != null)
            {
                SignIn(user.Email, user.Name, user.Role, persistent, user.Id, token);
            }
        }

        internal async Task RefreshUserDetails()
        {
            int userId = int.Parse(CurrentUser.Claims.First(x => x.Type == ClaimTypes.SerialNumber).Value);
            bool persistent = Convert.ToBoolean(CurrentUser.Claims.First(x => x.Type == ClaimTypes.IsPersistent).Value);
            string token = CurrentUser.Claims.First(x => x.Type == ClaimTypes.Hash).Value;

            User? user = await UserService.GetUserByIdAsync(userId);

            if (user != null)
            {
                SignIn(user.Email, user.Name, user.Role, persistent, user.Id, token);
            }
        }

        internal async Task SignOut()
        {
            AuthenticationState authState = GetAuthenticationStateAsync().Result;

            string token = authState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Hash && !string.IsNullOrEmpty(x.Value))?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                await SessionService.DeleteSessionAsync(token);
            }

            CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        internal string GetUserCookieValue(HttpContext context)
        {
            IRequestCookieCollection cookies = context.Request.Cookies;
            return cookies[CookieName] ?? string.Empty;
        }

        internal void SetUserAuthCookie(HttpContext context, int userId, string token, bool persistent)
        {
            string cookieValue = CreateUserTokenPair(userId, token);

            DateTimeOffset? expires = persistent ? new(CustomAuthStateProvider.GetSessionExpiration(persistent)) : null;

            CookieOptions cookieOptions = new()
            {
                Expires = expires,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            context.Response.Cookies.Append(CookieName, cookieValue, cookieOptions);
        }

        internal void DeleteUserAuthCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(CookieName);
        }

        internal static (int, string) SplitCookiValue(string cookieValue)
        {
            string[] cookieValueSegments = cookieValue.Split("-", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (cookieValueSegments.Length == 6)
            {
                bool userSuccess = int.TryParse(cookieValueSegments[^1], out int userId);
                string token = cookieValue[..Guid.Empty.ToString().Length];

                if (userSuccess)
                {
                    return (userId, token);
                }
            }


            return default;
        }

        internal static string CreateUserTokenPair(int userId, string token)
        {
            return $"{token}-{userId}";
        }

        internal static DateTime GetSessionExpiration(bool persistent)
        {
            return persistent ? DateTime.Now.AddMonths(1) : DateTime.Now.AddDays(1);
        }
    }
}
