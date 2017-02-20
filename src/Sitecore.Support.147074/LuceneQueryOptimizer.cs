
namespace Sitecore.Support.ContentSearch.Linq.Lucene
{
    using Sitecore.ContentSearch.Linq.Lucene;
    using Sitecore.ContentSearch.Linq.Nodes;
    using Sitecore.ContentSearch.Linq.Parsing;

    public class LuceneQueryOptimizer : Sitecore.ContentSearch.Linq.Lucene.LuceneQueryOptimizer
    {
        public override IndexQuery Optimize(IndexQuery query)
        {
            var state = this.BuildOptimizerState();
            QueryNode rootNode = this.Visit(query.RootNode, state);
            this.RetrieveAndApplyContext(ref rootNode, state);

            return this.CreateQueryInstance(rootNode, query.ElementType);
        }

        protected override QueryNode VisitInContext(InContextNode node, LuceneQueryOptimizerState state)
        {            
           var exState = state as ExLuceneQueryOptimizerState;

           if (exState == null)
           {
               // if there is no extended state then skip InContext because it is handled incorrectly 
               return this.Visit(node.SourceNode, state);
           }

           exState.CurrentInContextRoot = null;
           var optimizedDescNodes = this.Visit(node.SourceNode, exState);

           this.RetrieveAndApplyContext(ref optimizedDescNodes, exState);

           exState.CurrentInContextRoot = node;

           return optimizedDescNodes;
        }

        protected override QueryNode VisitUnion(UnionNode node, LuceneQueryOptimizerState state)
        {
            var innerState = this.BuildOptimizerState();
            var outerQueryNode = this.Visit(node.OuterQuery, innerState);
            this.RetrieveAndApplyContext(ref outerQueryNode, innerState);

            innerState = this.BuildOptimizerState();
            var innerQueryNode = this.Visit(node.InnerQuery, innerState);
            this.RetrieveAndApplyContext(ref innerQueryNode, innerState);

            return new UnionNode(outerQueryNode, innerQueryNode, node.OuterQueryExpression, node.InnerQueryExpression, node.GetQueryableDelegate);
        }

        protected void RetrieveAndApplyContext(ref QueryNode rootNode, LuceneQueryOptimizerState state)
        {
            var exState = state as ExLuceneQueryOptimizerState;
            if (exState == null)
            {
                return;
            }

            var descInContextNode = exState.CurrentInContextRoot;

            if (descInContextNode != null)
            {
                rootNode = new InContextNode(rootNode, descInContextNode.ExecutionContext);
            }
        }

        protected virtual LuceneQueryOptimizerState BuildOptimizerState()
        {
            return new ExLuceneQueryOptimizerState();
        }
    }
}