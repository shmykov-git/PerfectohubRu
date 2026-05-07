using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Shared.Extensions
{
    public static class MapsterExtensions
    {
        public static IServiceCollection AddSharedMapster(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                throw new ArgumentException("set at least one assemly for mapster");

            ServiceCollectionExtensions.AddMapster(services);
            services.AddSingleton(new TypeAdapterConfig());

            var registerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IRegister).IsAssignableFrom(t)
                           && !t.IsAbstract && !t.IsInterface)
                .ToList();

            foreach (var type in registerTypes)
            {
                services.AddSingleton(type);
            }

            return services;
        }

        public static IServiceProvider UseMapster(this IServiceProvider sp, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                throw new ArgumentException("set at least one assemly for mapster");

            var config = sp.GetRequiredService<TypeAdapterConfig>();

            var registerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IRegister).IsAssignableFrom(t)
                           && !t.IsAbstract && !t.IsInterface)
                .ToList();

            foreach (var type in registerTypes)
            {
                var register = (IRegister)sp.GetRequiredService(type);
                register.Register(config);
            }

            return sp;
        }
    }
}