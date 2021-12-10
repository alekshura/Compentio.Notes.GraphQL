using System.Collections.Generic;

namespace Compentio.Notes.GraphQL.GraphQL
{
    public static class GraphQLUserContextExtensions
    {
        private const string FetchIdIdKey = "FetchId";

        public static void SetFetchId(this IDictionary<string, object> userContextDictionary, string fetchId)
        {
            userContextDictionary[FetchIdIdKey] = fetchId;
        }

        public static string GetFetchId(this IDictionary<string, object> userContextDictionary)
        {
            return (string)userContextDictionary[FetchIdIdKey];
        }

        public static bool HasFetchId(this IDictionary<string, object> userContextDictionary)
        {
            if (!userContextDictionary.TryGetValue(FetchIdIdKey, out var fetchId))
            {
                return false;
            }

            if (fetchId is null)
            {
                return false;
            }

            return true;
        }
    }
}
