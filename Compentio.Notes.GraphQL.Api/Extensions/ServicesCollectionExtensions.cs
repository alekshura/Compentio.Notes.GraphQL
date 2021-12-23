namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Compentio.Notes.GraphQL.Notes;
    using Compentio.SourceMapper.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {            
            services.AddHealthChecks();
            services.AddTransient<INotesService, NotesService>();
            services.AddMappers();
            return services;
        }
    }
}
