
namespace Sitecore.Support.ContentSearch.LuceneProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Sitecore.ContentSearch.Diagnostics;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.ContentSearch.LuceneProvider;
    using Sitecore.ContentSearch.Pipelines;
    using Sitecore.ContentSearch.Pipelines.QueryGlobalFilters;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.ContentSearch.Security;
    using Sitecore.ContentSearch.Utilities;

    public class LuceneSearchContext : Sitecore.ContentSearch.LuceneProvider.LuceneSearchContext
    {
        private readonly IContentSearchConfigurationSettings settings;

        public LuceneSearchContext(ILuceneProviderIndex index, SearchSecurityOptions securityOptions = SearchSecurityOptions.Default) : base(index, securityOptions)
        {
            this.settings = index.Locator.GetInstance<IContentSearchConfigurationSettings>();
        }

        public LuceneSearchContext(ILuceneProviderIndex index, IEnumerable<ILuceneProviderSearchable> searchables, SearchSecurityOptions securityOptions = SearchSecurityOptions.Default) : base(index, searchables, securityOptions)
        {
            this.settings = index.Locator.GetInstance<IContentSearchConfigurationSettings>();
        }

        public LuceneSearchContext(ILuceneProviderIndex index, IEnumerable<ILuceneProviderSearchable> searchables, LuceneIndexAccess indexAccess = LuceneIndexAccess.ReadOnly | LuceneIndexAccess.ReadOnlyCached, SearchSecurityOptions securityOptions = SearchSecurityOptions.Default) : base(index, searchables, indexAccess, securityOptions)
        {
            this.settings = index.Locator.GetInstance<IContentSearchConfigurationSettings>();
        }

        public override IQueryable<TItem> GetQueryable<TItem>(params IExecutionContext[] executionContexts)
        {
            var linqIndex = new Sitecore.Support.ContentSearch.LuceneProvider.LinqToLuceneIndex<TItem>(this, executionContexts);

            if (this.settings.EnableSearchDebug())
            {
                (linqIndex as IHasTraceWriter).TraceWriter = new LoggingTraceWriter(SearchLog.Log);
            }

            var queryable = linqIndex.GetQueryable();

            if (typeof(SearchResultItem).IsAssignableFrom(typeof(TItem)))
            {
                var args = new QueryGlobalFiltersArgs(linqIndex.GetQueryable(), typeof(TItem), executionContexts.ToList());
                this.Index.Locator.GetInstance<ICorePipeline>().Run(PipelineNames.QueryGlobalFilters, args);
                queryable = (IQueryable<TItem>)args.Query;
            }

            return queryable;
        }
    }
}