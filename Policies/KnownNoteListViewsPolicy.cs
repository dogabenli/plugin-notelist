using Sitecore.Commerce.Core;

namespace Plugin.Sample.Notes.Policies
{
    public class KnownNoteListViewsPolicy : Policy
    {
        public string SellableItemNoteList { get; set; } = nameof(SellableItemNoteList);
    }
}
