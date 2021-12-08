﻿namespace Compentio.Notes.GraphQL.Api.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Compentio.Notes.GraphQL.Notes;
    using Compentio.SourceMapper.DependencyInjection;

    public static class ServicesCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {            
            services.AddHealthChecks();

            // add new services below 
            services.AddTransient<INotesService, NotesService>();
            services.AddMappers();
        }
    }
}
