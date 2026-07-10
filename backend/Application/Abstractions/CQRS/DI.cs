namespace backend.Application.Abstractions.CQRS;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
  public static IServiceCollection AddCqrsHandlers(this IServiceCollection services, Assembly assembly)
  {
    var handlerInterfaces = new[] { typeof(ICommandHandler<>), typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };

    var types = assembly.GetTypes()
      .Where(t => t.IsClass && !t.IsAbstract);

    foreach (var type in types)
    {
      var interfaces = type.GetInterfaces()
        .Where(i => i.IsGenericType && handlerInterfaces.Contains(i.GetGenericTypeDefinition()));

      foreach (var @interface in interfaces)
        services.AddScoped(@interface, type);
    }

    return services;
  }
}