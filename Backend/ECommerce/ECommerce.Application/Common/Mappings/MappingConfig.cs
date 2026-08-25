using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application.Common.Mappings;

public static class MappingConfig
{
    public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Scan only concrete non-abstract class types that implement IRegister
        // Skip record types (they have no parameterless constructor) to avoid MissingMethodException
        var registerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract
                       && !t.IsInterface
                       && t.IsClass
                       && typeof(IRegister).IsAssignableFrom(t)
                       && t.GetConstructor(Type.EmptyTypes) != null); // Must have parameterless ctor

        foreach (var type in registerTypes)
        {
            var instance = (IRegister)Activator.CreateInstance(type)!;
            instance.Register(config);
        }

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}