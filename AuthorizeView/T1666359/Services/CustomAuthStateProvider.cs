using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace T1666359.Services
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
        private ClaimsPrincipal _currentUser { get; set; } = new(new ClaimsIdentity());
    
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public void SignIn(string userIdentifier)
        {
            var claims = new Claim[] { new Claim(ClaimTypes.Name, userIdentifier) };

            var identity = new ClaimsIdentity(claims, "Custom Authentication");

            _currentUser = new ClaimsPrincipal(identity);
        
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public void SignOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
