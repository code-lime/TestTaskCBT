using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;
using TaskCBT.Infrastructure.DataBase;
using TaskCBT.Infrastructure.Services;

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

            .AddScoped<IJwtService, JwtService>()
            .AddScoped<IEmailService, EmailService>()

            .AddScoped<IEmailRegistry, EmailRegistry>()

            .Configure<JwtConfig>(configuration.GetRequiredSection(JwtConfig.SectionKey))
            .Configure<EmailConfig>(configuration.GetRequiredSection(EmailConfig.SectionKey));
    }
}
