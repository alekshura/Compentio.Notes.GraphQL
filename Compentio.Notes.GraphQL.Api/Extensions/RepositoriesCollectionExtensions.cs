namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Compentio.Notes.GraphQL.Notes;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class RepositoriesCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<INotesRepository, NotesRepository>();
            return services;
        }
    }
}
