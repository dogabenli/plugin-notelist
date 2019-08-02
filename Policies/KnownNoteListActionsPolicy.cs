using Sitecore.Commerce.Core;

namespace Plugin.Sample.Notes.Policies
{
    public class KnownNoteListActionsPolicy : Policy
    {
        public string CreateNote { get; set; } = nameof(CreateNote);

        public string EditNote { get; set; } = nameof(EditNote);
    }
}
