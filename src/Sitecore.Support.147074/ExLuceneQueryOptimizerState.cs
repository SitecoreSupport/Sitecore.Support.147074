
namespace Sitecore.Support.ContentSearch.Linq.Lucene
{
    using Sitecore.ContentSearch.Linq.Nodes;

    public class ExLuceneQueryOptimizerState : Sitecore.ContentSearch.Linq.Lucene.LuceneQueryOptimizerState
    {
        public InContextNode CurrentInContextRoot { get; set; }
    }
}