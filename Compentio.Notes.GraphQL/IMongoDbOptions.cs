namespace Compentio.Notes.GraphQL.Notes
{
    public interface IMongoDbOptions
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
