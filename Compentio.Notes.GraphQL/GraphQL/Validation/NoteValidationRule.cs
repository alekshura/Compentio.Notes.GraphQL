using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compentio.Notes.GraphQL.GraphQL.Validation
{
    public class NoteValidationRule : IValidationRule
    {
        public NoteValidationRule()
        {
        }

        public async Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return new NodeVisitors(
                new MatchingNodeVisitor<Argument>((arg, context) =>
                {
                    ValidateAsync(arg, context, context.TypeInfo.GetArgument());
                })
            );
        }

        private void ValidateAsync(IHaveValue node, ValidationContext context, QueryArgument argument)
        {
            if (argument is null)
                return;

            if (!IsNoteArgument(argument.Name))
                return;

            var note = context.Inputs.FirstOrDefault(x => IsNoteArgument(x.Key)).Value as Dictionary<string, object>;
            var noteTitle = note["title"] as string;

            if (!string.IsNullOrEmpty(noteTitle) && noteTitle.Length > 50)
            {
                context.ReportError(new ValidationError(context.Document.OriginalQuery, "1.0", $"Field 'title' in argument '{argument.Name}' can not be longer than 50", node));
            }
        }
        private static bool IsNoteArgument(string argumentName)
        {
            return argumentName.Equals("note", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
