using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TaskCBT.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
