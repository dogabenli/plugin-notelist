using Plugin.Sample.Notes;
using Plugin.Sample.Notes.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;

namespace Plugin.Sample.NoteList.Pipelines.Blocks.EntityViews
{
    [PipelineDisplayName(NoteListConstants.Pipelines.Blocks.PopulateNoteListActionsBlock)]
    public class PopulateNoteListActionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var viewsPolicy = context.GetPolicy<KnownNoteListViewsPolicy>();

            if (string.IsNullOrEmpty(arg?.Name) ||
                !arg.Name.Equals(viewsPolicy.SellableItemNoteList, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(arg);
            }

            var actionPolicy = arg.GetPolicy<ActionsPolicy>();

            actionPolicy.Actions.Add(
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownNoteListActionsPolicy>().CreateNote,
                    DisplayName = "Create Item Note",
                    Description = "Create sellable item note",
                    IsEnabled = true,
                    EntityView = arg.Name,
                    Icon = "add"
                });

            actionPolicy.Actions.Add(
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownNoteListActionsPolicy>().EditNote,
                    DisplayName = "Edit Item Note",
                    Description = "Edit sellable item note",
                    IsEnabled = true,
                    EntityView = arg.Name,
                    Icon = "edit"
                });

            return Task.FromResult(arg);
        }
    }
}
