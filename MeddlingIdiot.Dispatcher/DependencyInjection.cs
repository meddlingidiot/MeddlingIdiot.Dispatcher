using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MeddlingIdiot.Dispatcher;
public static class DependencyInjection
{
    public static IServiceCollection AddDispatcher(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        services.Scan(scan => scan.FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
        );

        return services;
    }

    public static IServiceCollection AddOpenBehavior(
        this IServiceCollection services,
        Type behaviorType)
    {
        if (!behaviorType.IsGenericType)
        {
            throw new ArgumentException("{Name} must be a generic type", behaviorType.FullName);
        }

        if (behaviorType.GetGenericArguments().Length == 2)
        {
            var implementedGenericInterfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition());
            var implementedOpenBehaviorInterfaces = new HashSet<Type>(
                implementedGenericInterfaces
                .Where(i => i == typeof(IPipelineBehavior<,>)));
            foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
            {
                services.AddTransient(openBehaviorInterface, behaviorType);
            }
        }
        else if (behaviorType.GetGenericArguments().Length == 1)
        {
            var implementedGenericInterfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition());
            var implementedOpenBehaviorInterfaces = new HashSet<Type>(
                implementedGenericInterfaces
                .Where(i => i == typeof(IPipelineBehavior<>)));
            foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
            {
                services.AddTransient(openBehaviorInterface, behaviorType);
            }
        }
        else
        {
            throw new ArgumentException("Must be a generic type definition with one or two generic parameters.", nameof(behaviorType));
        }
        return services;
    }
}
