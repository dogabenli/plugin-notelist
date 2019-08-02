using Plugin.Sample.NoteList.Components;
using Plugin.Sample.Notes;
using Plugin.Sample.Notes.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.NoteList.Pipelines.Blocks.DoActions
{
    [PipelineDisplayName(NoteListConstants.Pipelines.Blocks.DoActionCreateNoteListBlock)]
    public class DoActionCreateNoteBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionCreateNoteBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var notesActionsPolicy = context.GetPolicy<KnownNoteListActionsPolicy>();

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(arg.Action) || !arg.Action.Equals(notesActionsPolicy.CreateNote, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            // Get the sellable item from the context
            var entity = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(arg.EntityId));
            if (entity == null)
            {
                return Task.FromResult(arg);
            }

            // Get the notes component from the sellable item or its variation
            var listComponent = entity.GetComponent<NoteListComponent>(arg.ItemId);

            var component = new NoteComponent
            {
                NoteText = arg.Properties.FirstOrDefault(x => x.Name.Equals(nameof(NoteComponent.NoteText), StringComparison.OrdinalIgnoreCase))?.Value,
                CreatedOnUtc = DateTime.UtcNow
            };

            // Map entity view properties to component
            listComponent.NoteListComponents.Add(component.Id, component);

            // Persist changes
            this._commerceCommander.Pipeline<IPersistEntityPipeline>().Run(new PersistEntityArgument(entity), context);

            return Task.FromResult(arg);
        }
    }
}
