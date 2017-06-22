using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using RestSharp;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiGetResponseController : StorefrontControllerBase
    {
        private const string ServerUrl = "https://api.getresponse.com/v3";

        public ApiGetResponseController(WorkContext context, IStorefrontUrlBuilder urlBuilder)
            : base(context, urlBuilder)
        {
        }

        // POST: /storefrontapi/getresponse/subscribe
        [HttpPost]
        public async Task<ActionResult> Subscribe(string email)
        {
            var apiKey = ConfigurationManager.AppSettings["GetResponse.ApiKey"];
            if(string.IsNullOrEmpty(apiKey))
                throw new StorefrontException("No API key provided");

            var campaignTokenId = ConfigurationManager.AppSettings["GetResponse.CampaignToken"];
            if (string.IsNullOrEmpty(apiKey))
                throw new StorefrontException("No Campaign Token Id provided");

            var client = new RestClient(ServerUrl);
            var request = new RestRequest("contacts", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("X-Auth-Token", $"api-key {apiKey}");

            var contact = new GetResponseContact
            {
                email = email,
                dayOfCycle = 0,
                campaign = new GetResponseCampaign { campaignId = campaignTokenId } 
            };

            request.AddBody(contact);
            IRestResponse response = await client.ExecuteTaskAsync(request);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }

    public class ResponseContact
    {
        public string httpStatus { get; set; }
        public string code { get; set; }
        public string codeDescription { get; set; }
        public string message { get; set; }
        public string moreInfo { get; set; }
        public object context { get; set; }
        public string uuid { get; set; }
    }

    public class GetResponseContact
    {
        public int dayOfCycle;
        public string email;

        public GetResponseCampaign campaign { get; set; }
    }

    public class GetResponseCampaign
    {
        public string campaignId { get; set; }
    }
}
