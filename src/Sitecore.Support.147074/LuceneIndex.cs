
namespace Sitecore.Support.ContentSearch.LuceneProvider
{
    using System.Linq;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Maintenance;
    using Sitecore.ContentSearch.Security;
    using Sitecore.ContentSearch.LuceneProvider;

    public class LuceneIndex : Sitecore.ContentSearch.LuceneProvider.LuceneIndex
    {
        public LuceneIndex(string name, string folder, IIndexPropertyStore propertyStore, string @group) : base(name, folder, propertyStore, @group)
        {
        }

        public LuceneIndex(string name, string folder, IIndexPropertyStore propertyStore) : base(name, folder, propertyStore)
        {
        }

        public LuceneIndex(string name) : base(name)
        {
        }

        public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions securityOptions = 0)
        {
            this.EnsureInitialized();
            return new Sitecore.Support.ContentSearch.LuceneProvider.LuceneSearchContext(this, this.Shards.Cast<ILuceneProviderSearchable>(), securityOptions);
        }
    }
}