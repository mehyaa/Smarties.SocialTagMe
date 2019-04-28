using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smarties.SocialTagMe.Abstractions.Services;

namespace Smarties.SocialTagMe.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IIdService, MockIdService>();
            //services.AddSingleton<IImageService, MockImageService>();
            //services.AddSingleton<ITagService, MockTagService>();

            services.AddSingleton<IIdService, Framework.IdService>();
            services.AddSingleton<IImageService, Framework.ImageService>();
            services.AddSingleton<ITagService, Framework.TagService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseMvc();

            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var imageService = serviceScope.ServiceProvider.GetRequiredService<IImageService>();

                    await imageService.TrainAsync();
                }
            });
        }
    }
}