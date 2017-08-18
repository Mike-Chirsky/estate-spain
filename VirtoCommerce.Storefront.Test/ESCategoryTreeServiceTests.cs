using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Services.Es;
using Xunit;
using Moq;
using System.IO;
using System.Threading;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;

namespace VirtoCommerce.Storefront.Test
{
    public class ESCategoryTreeServiceTests
    {
        public ESCategoryTreeServiceTests()
        {
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(new UnityContainer()));
        }

        public ICatalogModuleApiClient CreateMockApiClient()
        {
            var apiClient = new Mock<ICatalogModuleApiClient>();

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                    It.Is<ProductSearchCriteria>(c => c.Outline == "06d09840154341a4b4564aa9b94b06b3"),
                    It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("06d09840154341a4b4564aa9b94b06b3.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                    It.Is<ProductSearchCriteria>(c => c.Outline == "2036643b08794d149e1722adbe0230e8"),
                    It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("2036643b08794d149e1722adbe0230e8.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "654dbd245eb2484fa5ccd8ffd69387da"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("654dbd245eb2484fa5ccd8ffd69387da.json"));


            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "d6cce7c7d9854f609ab3fd5109d79a57"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("d6cce7c7d9854f609ab3fd5109d79a57.json"));


            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "efb222551d45446f89597cb2e5391e3b"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("efb222551d45446f89597cb2e5391e3b.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "e2c000ac94e44f6db47af5ba103c5c5d"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("e2c000ac94e44f6db47af5ba103c5c5d.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "815ddf6797f045f69e8e53605baa00b7"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("815ddf6797f045f69e8e53605baa00b7.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "00d8afcebe4b4f6ab02aab523c441bff"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("00d8afcebe4b4f6ab02aab523c441bff.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "834c195a10c44bd582d32c99f6f8ebe7"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("834c195a10c44bd582d32c99f6f8ebe7.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "39511a105f124f86a82a37056b74e215"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("39511a105f124f86a82a37056b74e215.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "482d6976884d42958863fda2cf91a91a"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("482d6976884d42958863fda2cf91a91a.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "f3d9acf66a6049e7b01b83519a5ed317"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("f3d9acf66a6049e7b01b83519a5ed317.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "4069bf77b3004921aa1e1a96eff1bab7"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("4069bf77b3004921aa1e1a96eff1bab7.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "2a457c6498aa4341ad380edc046a019a"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("2a457c6498aa4341ad380edc046a019a.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "dafbaa8a96da4ea4bdfd67bf707c116b"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("dafbaa8a96da4ea4bdfd67bf707c116b.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "46f93fa133674323a98b9cbc55d7dfb6"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("46f93fa133674323a98b9cbc55d7dfb6.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "417574fdb3914d5bb620a705c31dc5b4"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("417574fdb3914d5bb620a705c31dc5b4.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "b2e5670ffad94288830a8c3b0489e976"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("b2e5670ffad94288830a8c3b0489e976.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "eac5828809f34ab7b59ab1e78199be5f"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("eac5828809f34ab7b59ab1e78199be5f.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "754bca93cdcc49fda013a617b89ed4ee"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("754bca93cdcc49fda013a617b89ed4ee.json"));

            apiClient.Setup(p => p.CatalogModuleSearch.SearchProductsWithHttpMessagesAsync(
                It.Is<ProductSearchCriteria>(c => c.Outline == "8a51d21eae164cafb66622dfd4814c5e"),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoadFromJsonFile<ProductSearchResult>("8a51d21eae164cafb66622dfd4814c5e.json"));

            apiClient.Setup(x => x.CatalogModuleProducts.GetProductByIdsWithHttpMessagesAsync(It.IsAny<IList<string>>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(LoadFromJsonFile<IList<Product>>("alicante_update.json"));

            return apiClient.Object;
        }

        private ILocalCacheManager CreateLocalCacheManager()
        {
            var client =
                new Mock<ILocalCacheManager>();
            return client.Object;
        }

        private Func<ICoreModuleApiClient> CreateCoreApiFactory()
        {
            Func<ICoreModuleApiClient> f = ()=>
            {
                var client =
                    new Mock<ICoreModuleApiClient>();
                client.Setup(x => x.Commerce.GetSeoInfoBySlugWithHttpMessagesAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoadFromJsonFile<IList<AutoRestClients.CoreModuleApi.Models.SeoInfo>>("alicante_seo.json"));
                return client.Object;
            };
            return f;
        }

        private static Microsoft.Rest.HttpOperationResponse<T> LoadFromJsonFile<T>(string fileName)
        {
            var filePath = Path.Combine(@"..\..\JsonMoq", "ESCategoryTreeService", fileName);

            return new Microsoft.Rest.HttpOperationResponse<T> { Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<T>(File.ReadAllText(filePath)) };
        }

        private static Store CreateStore(string storeId, string catalogId, SeoLinksType linksType, params string[] cultureNames)
        {
            var result = new Store
            {
                Id = storeId,
                Catalog = catalogId,
                SeoLinksType = linksType,
                Languages = cultureNames.Select(n => new Language(n)).ToList(),
            };

            result.DefaultLanguage = result.Languages.FirstOrDefault();

            return result;
        }

        public ESCategoryTreeService CreateService()
        {

            // TODO: Create mock chache manager, 
            var service = new ESCategoryTreeService(CreateMockApiClient(), CreateWorkContext, CreateLocalCacheManager(), CreateCoreApiFactory());
            return service;
        }

        public WorkContext CreateWorkContext()
        {
            return new WorkContext
            {
                CurrentLanguage = new Language("ru-RU"),
                CurrentStore = CreateStore("Demo", "Demo", SeoLinksType.Short, "ru-RU"),
                CurrentCurrency = new Currency(new Language("en-US"), "USD")
            };
        }

        [Fact]
        public void BuildVirtualCategoryTree_Test()
        {
            var client = CreateService();
            var tree = client.GetTree().Result;
            Assert.NotEmpty(tree);
        }
        [Fact]
        public void RebuildVirtualTree_Test()
        {
            var client = CreateService();
            client.GetTree().Wait();
            var tree = client.RebuildTree().Result;
            Assert.NotEmpty(tree);
        }
        [Fact]
        public void RebuildElementTree_Test()
        {
            var client = CreateService();
            client.GetTree().Wait();
            client.RebuildElement("alicante").Wait();
        }
        [Fact]
        public void GetCategory_Test()
        {
            var client = CreateService();
            client.GetTree().Wait();
            Assert.NotNull(client.FindByPath("alicante"));
        }
    }
}
