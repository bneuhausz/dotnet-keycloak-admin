using Nbx.DotnetKeycloak.Admin.Configuration;
using Nbx.DotnetKeycloak.Admin.Repositories;
using Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;
using Nbx.DotnetKeycloak.Admin.Services;
using Nbx.DotnetKeycloak.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nbx.DotnetKeycloak.Admin.Helpers;

namespace Nbx.DotnetKeycloak.Admin.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
                        }
                    }
                }
            });

            var securiteRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                }
            };

            o.AddSecurityRequirement(securiteRequirement);
        });

        return services;
    }

    internal static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.Audience = configuration["Authentication:Audience"];
                o.MetadataAddress = configuration["Authentication:MetadataAddress"]!;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
    }

    internal static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("UserManagerPolicy", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    var resourceAccessClaim = context.User.FindFirst("resource_access");
                    if (resourceAccessClaim != null)
                    {
                        var resourceAccess = System.Text.Json.JsonDocument.Parse(resourceAccessClaim.Value);

                        if (resourceAccess.RootElement.TryGetProperty("dotnet-public", out var clientRoles) &&
                            clientRoles.TryGetProperty("roles", out var rolesArray))
                        {
                            return rolesArray.EnumerateArray()
                                .Any(role => role.GetString() == "user-manager");
                        }
                    }
                    return false;
                });
            });
        });
    }

    internal static void ConfigureRepopsitories(this IServiceCollection services)
    {
        services.AddScoped<IKeycloakAdminRepository, KeycloakAdminRepository>();
    }

    internal static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();
    }

    internal static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KeycloakAdminOptions>().BindConfiguration(KeycloakAdminOptions.Section);
    }

    internal static void ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IKeycloakAdminRepository, KeycloakAdminRepository>((serviceProvider, client) =>
        {
            var keycloakConfig = serviceProvider.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
            client.BaseAddress = new Uri(keycloakConfig.BaseAddress);
        }).AddHttpMessageHandler<KeycloakAdminTokenHandler>();

        services.AddScoped<KeycloakAdminTokenHandler>();
    }
}
