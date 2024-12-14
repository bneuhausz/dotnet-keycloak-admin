using Dotnet_Keycloak_Admin.Configuration;
using Dotnet_Keycloak_Admin.Repositories;
using Dotnet_Keycloak_Admin.Repositories.Interfaces;
using Dotnet_Keycloak_Admin.Services;
using Dotnet_Keycloak_Admin.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Dotnet_Keycloak_Admin.Extensions;

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
        });
    }
}
