using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSessions.Data;
using UserSessions.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerLocalization.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ISessionService ControllerSessionService;

        private readonly IUserService ControllerUserService;

        private readonly CustomAuthStateProvider AuthStateProvider;

        public UserController(ISessionService sessionService, IUserService userService, AuthenticationStateProvider authenticationStateProvider)
        {
            ControllerSessionService = sessionService;

            ControllerUserService = userService;

            AuthStateProvider = (CustomAuthStateProvider)authenticationStateProvider;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password, bool remember, string redirectUri)
        {
            int? userId = await ControllerUserService.GetUserIdByCredentials(email, password);

            if (userId.HasValue)
            {
                Session session = new()
                {
                    Expires = CustomAuthStateProvider.GetSessionExpiration(remember),
                    Pesistent = remember,
                    Token = Guid.NewGuid().ToString(),
                    UserId = userId.Value
                };

                await ControllerSessionService.CreateSessionAsync(session);
                AuthStateProvider.SetUserAuthCookie(HttpContext, userId.Value, session.Token, remember);

                return LocalRedirect(redirectUri);
            }
            else
            {
                return LocalRedirect("/sign-in?invalidcredentials=true");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignInDotNetAuth(string email, string password, bool remember, string redirectUri)
        {
            int? userId = await ControllerUserService.GetUserIdByCredentials(email, password);

            if (userId.HasValue)
            {
                Session session = new()
                {
                    Expires = CustomAuthStateProvider.GetSessionExpiration(remember),
                    Pesistent = remember,
                    Token = Guid.NewGuid().ToString(),
                    UserId = userId.Value
                };

                await ControllerSessionService.CreateSessionAsync(session);
                //AuthStateProvider.SetUserAuthCookie(HttpContext, userId.Value, session.Token, remember);

                User? userToSignInDotNet = await ControllerUserService.GetUserByIdAsync(userId.Value);

                if (userToSignInDotNet != null)
                {
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Email, userToSignInDotNet.Email),
                        new Claim(ClaimTypes.SerialNumber, userToSignInDotNet.Id.ToString()),
                        new Claim(ClaimTypes.Name, userToSignInDotNet.Name),
                        new Claim(ClaimTypes.Role, userToSignInDotNet.Role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }

                return LocalRedirect(redirectUri);
            }
            else
            {
                return LocalRedirect("/sign-in?invalidcredentials=true");
            }
        }

        public IActionResult SignOut(string? redirectUri)
        {
            AuthStateProvider.DeleteUserAuthCookie(HttpContext);

            return LocalRedirect(redirectUri ?? "/");
        }

        public async Task<IActionResult> SignOutDotNetAuth(string? redirectUri)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return LocalRedirect(redirectUri ?? "/");
        }
    }
}
