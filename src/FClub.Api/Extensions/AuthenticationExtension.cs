using FClub.Core.Constants;
using FClub.Api.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;

namespace FClub.Api.Extensions;

public static class AuthenticationExtension
{
    public static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthenticationSchemeConstants.ApiKeyAuthenticationScheme;
                options.DefaultChallengeScheme = AuthenticationSchemeConstants.ApiKeyAuthenticationScheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                AuthenticationSchemeConstants.ApiKeyAuthenticationScheme, _ => { });
        
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(
                AuthenticationSchemeConstants.ApiKeyAuthenticationScheme).RequireAuthenticatedUser().Build();
        });
    }
}