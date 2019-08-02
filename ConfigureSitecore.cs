// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Sample
{
    using System.Reflection;
    using global::Plugin.Sample.NoteList.Pipelines.Blocks.DoActions;
    using global::Plugin.Sample.NoteList.Pipelines.Blocks.EntityViews;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config =>
                config
                    .ConfigurePipeline<IGetEntityViewPipeline>(c =>
                    {
                        c.Add<GetNoteListViewBlock>().After<GetSellableItemDetailsViewBlock>();
                    })
                    .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                    {
                        c.Add<PopulateNoteListActionBlock>().After<InitializeEntityViewActionsBlock>();
                    })
                    .ConfigurePipeline<IDoActionPipeline>(c =>
                    {
                        c.Add<DoActionCreateNoteBlock>().After<ValidateEntityVersionBlock>();
                    })
                    .ConfigurePipeline<IDoActionPipeline>(c =>
                    {
                        c.Add<DoActionEditNoteBlock>().After<ValidateEntityVersionBlock>();
                    })
            );
        }
    }
}