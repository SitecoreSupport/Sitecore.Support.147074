
namespace Sitecore.Support.ContentSearch.LuceneProvider
{
    using System.Linq;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.LuceneProvider;
    using Sitecore.ContentSearch.Maintenance;
    using Sitecore.ContentSearch.Security;

    public class SwitchOnRebuildLuceneIndex: Sitecore.ContentSearch.LuceneProvider.SwitchOnRebuildLuceneIndex
    {
        public SwitchOnRebuildLuceneIndex(string name, string folder, IIndexPropertyStore propertyStore) : base(name, folder, propertyStore)
        {
        }

        public SwitchOnRebuildLuceneIndex(string name) : base(name)
        {
        }

        public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions securityOptions = 0)
        {
            this.EnsureInitialized();
            return new Sitecore.Support.ContentSearch.LuceneProvider.LuceneSearchContext(this, this.Shards.Cast<ILuceneProviderSearchable>(), securityOptions);
        }
    }
}