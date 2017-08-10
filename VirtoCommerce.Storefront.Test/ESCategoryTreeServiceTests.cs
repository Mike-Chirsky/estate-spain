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

            return apiClient.Object;
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
            var service = new ESCategoryTreeService(CreateMockApiClient(), CreateWorkContext, null, null);
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
        public void ConvertRegions()
        {
            var client = CreateService();

            var tree = client.GetTree().Result;
        }

    }
}
