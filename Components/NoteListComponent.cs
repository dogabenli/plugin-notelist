using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Plugin.Sample.NoteList.Components
{
    public class NoteListComponent : Component
    {
        public IDictionary<string, NoteComponent> NoteListComponents { get; set; }

        public NoteListComponent()
        {
            NoteListComponents = new Dictionary<string, NoteComponent>();
        }
    }
}
