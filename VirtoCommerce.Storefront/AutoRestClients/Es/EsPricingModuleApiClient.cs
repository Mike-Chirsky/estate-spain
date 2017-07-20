using VirtoCommerce.Storefront.AutoRestClients.Es;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi
{
    public partial class PricingModuleApiClient
    {
        private readonly ILocalCacheManager _cacheManager;

        public PricingModuleApiClient(System.Uri baseUri, Microsoft.Rest.ServiceClientCredentials credentials, ILocalCacheManager cacheManager, System.Net.Http.HttpClientHandler rootHandler, params System.Net.Http.DelegatingHandler[] handlers) : this(rootHandler, handlers)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            this.BaseUri = baseUri;
            this.Credentials = credentials;
            if (this.Credentials != null)
            {
                this.Credentials.InitializeServiceClient(this);
            }

            _cacheManager = cacheManager;
            this.PricingModule = new EsPricingModule(new PricingModule(this), _cacheManager);
        }
    }
}