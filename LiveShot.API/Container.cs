using System.Linq;
using System.Reflection;
using LiveShot.API.Drawing;
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

            var drawingTools = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.GetInterfaces().Contains(typeof(IDrawingTool)) && !a.IsAbstract);

            foreach (var tool in drawingTools)
            {
                services.AddSingleton(typeof(IDrawingTool), tool);
            }

            return services;
        }
    }
}