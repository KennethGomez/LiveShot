using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.UI
{
    public static class Container
    {
        public static IServiceCollection ConfigureUI(this IServiceCollection services)
        {
            IEnumerable<Type>? views = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.Namespace == typeof(App).Namespace + ".Views");

            foreach (var view in views) services.AddTransient(view);

            return services;
        }
    }
}