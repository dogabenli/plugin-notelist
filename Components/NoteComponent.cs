using Sitecore.Commerce.Core;
using System;

namespace Plugin.Sample.NoteList.Components
{
    public class NoteComponent : Component
    {
        public string NoteText { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime EditedOnUtc { get; set; }

        public string CreatedBy { get; set; }
    }
}
