using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;
using TaskCBT.Services;

namespace TaskCBT;

public static class ConfigureServices
{
    public static IServiceCollection AddServerServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(v => v.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddCors();
        services.AddControllers();
        services.AddSingleton(Log.Logger);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<ISecure, SecureSHA256>();

        JwtConfig jwtConfig = configuration.GetRequiredSection(JwtConfig.SectionKey).Get<JwtConfig>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = jwtConfig.GetSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
            });
        
        services.AddAuthorizationBuilder()

            .AddPolicy("user", policy => policy
                .RequireAssertion(context =>
                    context.User.IsInRole("user")))

            .AddPolicy("create", policy => policy
                .RequireAssertion(context =>
                    context.User.IsInRole("create")))

            .AddPolicy("confirm", policy => policy
                .RequireAssertion(context =>
                    context.User.IsInRole("confirm")));


        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
