﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;
using TaskCBT.Infrastructure.DataBase;
using TaskCBT.Infrastructure.Services;
using TaskCBT.Infrastructure.Services.Registry;

namespace TaskCBT.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection database = configuration.GetRequiredSection("DataBase");
        if (database.Value is not string connectionString)
        {
            MySqlConnectionStringBuilder builder = [];
            foreach (IConfigurationSection child in database.GetChildren())
                builder[child.Key] = child.Get<string>();
            connectionString = builder.ConnectionString;
        }
        return services
            .AddDbContext<IContext, ApplicationDbContext>(options => options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .UseLazyLoadingProxies()
                .UseSnakeCaseNamingConvention())

            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IAuthRepository, AuthRepository>()
            .AddScoped<IUserRepository, UserRepository>()

            .AddSingleton<IJwtService, JwtService>()
            .AddSingleton<IEmailService, EmailService>()
            .AddSingleton<IPhoneService, PhoneService>()

            .AddScoped<IEmailRegistry, EmailRegistry>()
            .AddScoped<IPhoneRegistry, PhoneRegistry>()

            .Configure<JwtConfig>(configuration.GetRequiredSection(JwtConfig.SectionKey))
            .Configure<EmailConfig>(configuration.GetRequiredSection(EmailConfig.SectionKey))
            .Configure<PhoneConfig>(configuration.GetRequiredSection(PhoneConfig.SectionKey));
    }
}
