using System.Linq;
using System.Reflection;
using LiveShot.API.Background;
using LiveShot.API.Background.ContextOptions;
using LiveShot.API.Drawing;
using LiveShot.API.Upload;
using LiveShot.API.Upload.Custom;
using LiveShot.API.Upload.Imgur;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiveShot.API
{
    public static class Container
    {
        public static IServiceCollection ConfigureAPI(this IServiceCollection services, IConfiguration? configuration)
        {
            services.AddSingleton<IEventPipeline, EventPipeline>();
            services.AddSingleton<IBackgroundApplication, BackgroundApplication>();

            if (configuration != null)
            {
                string? uploadType = configuration["UploadType"];

                if (uploadType != null && uploadType.ToLower().Equals("imgur"))
                {
                    services.AddSingleton<IUploadService, ImgurService>();
                }
                else
                {
                    services.AddSingleton<IUploadService, CustomUploadService>();
                }
            }
            else
            {
                services.AddSingleton<IUploadService, ImgurService>();
            }
            
            services.AddSingleton<ILiveShotService, LiveShotService>();

            var drawingTools = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.GetInterfaces().Contains(typeof(IDrawingTool)) && !a.IsAbstract);

            foreach (var tool in drawingTools)
            {
                services.AddSingleton(typeof(IDrawingTool), tool);
            }

            var contextOptions = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.GetInterfaces().Contains(typeof(IContextOption)) && !a.IsAbstract);

            foreach (var option in contextOptions)
            {
                services.AddSingleton(typeof(IContextOption), option);
            }

            return services;
        }
    }
}