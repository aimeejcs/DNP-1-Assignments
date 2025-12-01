using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorClient.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly IAuthService authService;

    public SimpleAuthProvider(IAuthService authService)
    {
        this.authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await authService.GetCurrentUserAsync();

        if (user == null)
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "custom");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
