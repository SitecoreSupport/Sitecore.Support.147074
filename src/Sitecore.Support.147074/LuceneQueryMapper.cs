
namespace Sitecore.Support.ContentSearch.Linq.Lucene
{
    using global::Lucene.Net.Search;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.ContentSearch.Linq.Lucene;
    using Sitecore.ContentSearch.Linq.Nodes;

    public partial class LuceneQueryMapper : Sitecore.ContentSearch.Linq.Lucene.LuceneQueryMapper
    {
        protected readonly IExecutionContextsModifier executionContextsModifier;

        public LuceneQueryMapper(LuceneIndexParameters parameters) : base(parameters)
        {
            this.executionContextsModifier = new ExecutionContextsModifier(parameters);
        }

        protected override Query Visit(QueryNode node, LuceneQueryMapperState mappingState)
        {
            switch (node.NodeType)
            {
                case QueryNodeType.InContext:
                    return this.VisitInContext((InContextNode)node, mappingState);
                default:
                    return base.Visit(node, mappingState);
            }
        }

        protected virtual Query VisitInContext(InContextNode node, LuceneQueryMapperState mappingState)
        {
            mappingState.ExecutionContexts.Add(node.ExecutionContext);

            var newExecutionContext = node.ExecutionContext;

            var originalCultureContext = this.SetExecutionContext(newExecutionContext);
            var nativeQuery = this.Visit(node.SourceNode, mappingState);

            if (originalCultureContext != null)
            {
                this.SetExecutionContext(originalCultureContext); 
            }
            else
            {
                this.RemoveExecutionContext(newExecutionContext);
            }

            return nativeQuery;
        }

        protected virtual IExecutionContext SetExecutionContext(IExecutionContext newExecutionContext)
        {
            var originalExecutionContext = this.executionContextsModifier.ReplaceFirstMatched(newExecutionContext);

            if (originalExecutionContext == null)
            {
                this.executionContextsModifier.Add(newExecutionContext);
            }

            return originalExecutionContext;
        }

        protected virtual void RemoveExecutionContext(IExecutionContext newExecutionContext)
        {
            this.executionContextsModifier.Remove(newExecutionContext);
        }

        
    }
}