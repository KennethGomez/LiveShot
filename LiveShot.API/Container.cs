using LiveShot.API.Upload;
using LiveShot.API.Upload.Imgur;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.API
{
    public static class Container
    {
        public static IServiceCollection ConfigureAPI(this IServiceCollection services)
        {
            services.AddSingleton<IEventPipeline, EventPipeline>();
            services.AddSingleton<IUploadService, ImgurService>();

            return services;
        }
    }
}