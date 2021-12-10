namespace Compentio.Notes.GraphQL.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Compentio.Notes.GraphQL.Api.Middleware;
    using Compentio.Notes.GraphQL.Api.Extensions;

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;            
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(Configuration);
            
            services.AddAppProblemDetails(Environment);

            services.AddAuthentication(Configuration);

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            
            services.AddSwagger(Configuration)
                .AddServices()
                .AddRepositories()
                .ConfigureGraphQL();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHttpHeaders();
            }

            app.UseAppExceptionHandler(env);

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseAppSwaggerUI(Configuration);

            app.UseGraphQLAltair();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}