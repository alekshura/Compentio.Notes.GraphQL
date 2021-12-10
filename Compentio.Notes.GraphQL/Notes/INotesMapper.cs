namespace Compentio.Notes.GraphQL.Notes
{
    using Compentio.SourceMapper.Attributes;

    [Mapper]
    public partial interface INotesMapper
    {
        [InverseMapping(InverseMethodName = "MapToDao")]
        Note MapFromDao(NoteDao notedao); 
    }
}
