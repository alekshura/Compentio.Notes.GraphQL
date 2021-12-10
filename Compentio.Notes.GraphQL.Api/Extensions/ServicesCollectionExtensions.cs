namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Compentio.Notes.GraphQL.Notes;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {            
            services.AddHealthChecks();

            // add new services below 
            services.AddTransient<INotesService, NotesService>();
            services.AddTransient<INotesMapper, NotesMapper>();
            return services;
        }
    }
}
