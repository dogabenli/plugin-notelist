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

namespace Plugin.Sample.NoteList.Pipelines.Blocks.EntityViews
{
    [PipelineDisplayName(NoteListConstants.Pipelines.Blocks.GetNoteListViewBlock)]
    public class GetNoteListViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public GetNoteListViewBlock(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument cannot be null.");

            var request = this._viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            var catalogViewsPolicy = context.GetPolicy<KnownCatalogViewsPolicy>();

            var notesViewsPolicy = context.GetPolicy<KnownNoteListViewsPolicy>();
            var notesActionsPolicy = context.GetPolicy<KnownNoteListActionsPolicy>();

            var isVariationView = request.ViewName.Equals(catalogViewsPolicy.Variant, StringComparison.OrdinalIgnoreCase);
            var isConnectView = arg.Name.Equals(catalogViewsPolicy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);

            // Make sure that we target the correct views
            if (string.IsNullOrEmpty(request.ViewName) ||
                !request.ViewName.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(catalogViewsPolicy.Details, StringComparison.OrdinalIgnoreCase) &&
                !request.ViewName.Equals(notesViewsPolicy.SellableItemNoteList, StringComparison.OrdinalIgnoreCase) &&
                !isVariationView &&
                !isConnectView)
            {
                return Task.FromResult(arg);
            }

            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return Task.FromResult(arg);
            }

            var sellableItem = (SellableItem)request.Entity;

            // See if we are dealing with the base sellable item or one of its variations.
            var variationId = string.Empty;
            if (isVariationView && !string.IsNullOrEmpty(arg.ItemId))
            {
                variationId = arg.ItemId;
            }

            var targetView = arg;

            //Check if the edit action was requested
            var isCreateView = !string.IsNullOrEmpty(arg.Action) && arg.Action.Equals(notesActionsPolicy.CreateNote, StringComparison.OrdinalIgnoreCase);
            var isEditView = !string.IsNullOrEmpty(arg.Action) && arg.Action.Equals(notesActionsPolicy.EditNote, StringComparison.OrdinalIgnoreCase);

            if (!(isCreateView || isEditView))
            {
                // Create a new view and add it to the current entity view.
                var view = new EntityView
                {
                    Name = context.GetPolicy<KnownNoteListViewsPolicy>().SellableItemNoteList,
                    DisplayName = "Note List",
                    EntityId = arg.EntityId,
                    ItemId = variationId,
                    EntityVersion = arg.EntityVersion,
                    UiHint = "Table"
                };

                arg.ChildViews.Add(view);

                targetView = view;
            }

            if (sellableItem != null && (sellableItem.HasComponent<NoteListComponent>(variationId) || isConnectView || isCreateView || isEditView))
            {
                var component = sellableItem.GetComponent<NoteListComponent>(variationId);
                AddPropertiesToView(targetView, component, isCreateView, isEditView);
            }

            return Task.FromResult(arg);
        }

        private void AddPropertiesToView(EntityView entityView, NoteComponent component)
        {
            entityView.Properties.Add(
            new ViewProperty
            {
                Name = nameof(NoteComponent.NoteText),
                RawValue = string.Empty,
                IsReadOnly = false,
                IsRequired = false,
                OriginalType = "Html"
            });
        }

        private void AddPropertiesToView(EntityView entityView, NoteListComponent component, bool isCreateView, bool isEditView)
        {
            if (isCreateView || isEditView)
            {
                var noteComponent = !string.IsNullOrEmpty(entityView.ItemId) ? component.NoteListComponents[entityView.ItemId] : null;

                entityView.Properties.Add(
                new ViewProperty
                {
                    Name = nameof(NoteComponent.NoteText),
                    RawValue = noteComponent != null && isEditView ? noteComponent.NoteText : string.Empty,
                    IsReadOnly = false,
                    IsRequired = true,
                    OriginalType = "Html"
                });

                return;
            }

            foreach (var item in component.NoteListComponents.Values)
            {
                var childView = new EntityView
                {
                    Name = "Note",
                    EntityId = entityView.EntityId,
                    ItemId = item.Id,
                    EntityVersion = entityView.EntityVersion
                };

                childView.Properties.Add(
                new ViewProperty
                {
                    Name = "Note",
                    RawValue = item.NoteText,
                    IsReadOnly = true,
                    OriginalType = "Html"
                });

                childView.Properties.Add(
                new ViewProperty
                {
                    Name = "Created on",
                    RawValue = item.CreatedOnUtc.ToString("F"),
                    IsReadOnly = true,
                    OriginalType = "Html"
                });

                var editedOnUtc = item.EditedOnUtc > DateTime.MinValue ? item.EditedOnUtc.ToString("F") : string.Empty; 

               childView.Properties.Add(
               new ViewProperty
               {
                   Name = "Edited on",
                   RawValue = editedOnUtc,
                   IsReadOnly = true,
                   OriginalType = "Html"
               });

                entityView.ChildViews.Add(childView);
            }
        }
    }
}
