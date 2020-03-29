using FeedGen.Server.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FeedGen.Server {
    public class Startup {
        public Startup( IConfiguration configuration ) {
            Configuration = configuration;

            var section = Configuration.GetSection( "FeedGen" );
            ApplicationConfiguration = new ApplicationConfiguration(
                section["RootDirectory"],
                section["ExternalAddress"]
            );
        }

        public IConfiguration Configuration { get; }
        public ApplicationConfiguration ApplicationConfiguration { get; }

        public void ConfigureServices( IServiceCollection services ) {
            services.AddControllers();
            services.AddSingleton<FeedFactory>();
            services.AddSingleton( ApplicationConfiguration );
        }

        public void Configure( IApplicationBuilder app, IWebHostEnvironment env ) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles( new StaticFileOptions {
                FileProvider = new PhysicalFileProvider( ApplicationConfiguration.RootDirectory ),
                RequestPath = "/media",
            } );
            app.UseRouting();
            app.UseEndpoints( endpoints => {
                endpoints.MapControllers();
            } );
        }
    }
}
