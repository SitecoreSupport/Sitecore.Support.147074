
namespace Sitecore.Support.ContentSearch.LuceneProvider
{
    using System.Reflection;
    using Diagnostics;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.ContentSearch.Linq.Lucene;
    using Sitecore.ContentSearch.Linq.Parsing;
    using Sitecore.ContentSearch.LuceneProvider.Search;

    public class LinqToLuceneIndex<TItem> : Sitecore.ContentSearch.LuceneProvider.LinqToLuceneIndex<TItem>
    {
        private readonly LuceneQueryOptimizer queryOptimizer;

        private readonly QueryMapper<LuceneQuery> queryMapper;

        private static readonly MethodInfo miApplySearchMethods;
        private static readonly MethodInfo miApplyScalarMethods;

        static LinqToLuceneIndex()
        {
            var typeLinqToLuceneIndex = typeof(Sitecore.ContentSearch.LuceneProvider.LinqToLuceneIndex<TItem>);
            miApplySearchMethods = typeLinqToLuceneIndex.GetMethod("ApplySearchMethods", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(miApplySearchMethods, "Can't find ApplySearchMethods method");
            miApplyScalarMethods = typeLinqToLuceneIndex.GetMethod("ApplyScalarMethods", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(miApplyScalarMethods, "Can't find ApplyScalarMethods method");
        }

        public LinqToLuceneIndex(Sitecore.ContentSearch.LuceneProvider.LuceneSearchContext context) : base(context)
        {
            Assert.IsNotNull(this.Parameters, "Parameters can't be null...");
            this.queryOptimizer = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryOptimizer();
            this.queryMapper = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryMapper(this.Parameters);
        }

        public LinqToLuceneIndex(Sitecore.ContentSearch.LuceneProvider.LuceneSearchContext context, IExecutionContext executionContext) : base(context, executionContext)
        {
            Assert.IsNotNull(this.Parameters, "Parameters can't be null...");
            this.queryOptimizer = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryOptimizer();
            this.queryMapper = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryMapper(this.Parameters);
        }

        public LinqToLuceneIndex(Sitecore.ContentSearch.LuceneProvider.LuceneSearchContext context, IExecutionContext[] executionContexts) : base(context, executionContexts)
        {
            Assert.IsNotNull(this.Parameters, "Parameters can't be null...");
            this.queryOptimizer = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryOptimizer();
            this.queryMapper = new Sitecore.Support.ContentSearch.Linq.Lucene.LuceneQueryMapper(this.Parameters);
        }

        protected override IQueryOptimizer QueryOptimizer
        {
            get { return this.queryOptimizer; }
        }

        protected override QueryMapper<LuceneQuery> QueryMapper
        {
            get { return this.queryMapper; }
        }

        [UsedImplicitly]
        private object ApplySearchMethods<TElement>(LuceneQuery query, ITopDocs searchHits)
        {
            var miApplySearchMethodsGeneric = miApplySearchMethods.MakeGenericMethod(typeof(TElement));
            return miApplySearchMethodsGeneric.Invoke(this, new object[] {query, searchHits});
        }

        [UsedImplicitly]
        private TResult ApplyScalarMethods<TResult, TDocument>(LuceneQuery query, object processedResults, ITopDocs results)
        {
            var documentType = typeof(TResult).GetGenericArguments()[0];
            var miApplyScalarMethodsGeneric = miApplyScalarMethods.MakeGenericMethod(typeof(TResult), documentType);
            return (TResult)miApplyScalarMethodsGeneric.Invoke(this, new object[] {query, processedResults, results});
        }
        
    }
}