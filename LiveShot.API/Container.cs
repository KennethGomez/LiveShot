using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.Objects
{
    public static class Container
    {
        public static IServiceCollection ConfigureAPI(this IServiceCollection services)
        {
            services.AddSingleton<IEventPipeline, EventPipeline>();

            return services;
        }
    }
}