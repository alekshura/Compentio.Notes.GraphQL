namespace Compentio.Notes.GraphQL.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Compentio.Notes.GraphQL.Api.Middleware;
    using Compentio.Notes.GraphQL.Api.Extensions;
    using Microsoft.Extensions.Logging;

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

            services.AddLogging(builder => builder.AddConsole());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpHeaders();
                app.UseHttpsRedirection();
            }

            app.UseAppExceptionHandler(env);

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