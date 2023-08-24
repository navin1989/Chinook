using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Helper
{
    public class Authentication : IAuthentication
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public string CurrentUserId { get; set; }

        public Authentication(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
            GetUserId();
        }

        private void GetUserId()
        {
            CurrentUserId = this.GetUser().Result;
        }

        private async Task<string?> GetUser()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(c=>c.Type.Contains(ClaimTypes.NameIdentifier))?.Value ?? string.Empty;
        }
    }
}
