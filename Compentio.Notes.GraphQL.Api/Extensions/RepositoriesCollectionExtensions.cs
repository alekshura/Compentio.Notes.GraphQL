namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Compentio.Notes.GraphQL.Notes;

    public static class RepositoriesCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<INotesRepository, NotesRepository>();
        }
    }
}
