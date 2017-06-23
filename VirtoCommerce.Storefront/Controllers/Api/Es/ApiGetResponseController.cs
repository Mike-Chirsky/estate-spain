using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
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
            var campaignTokenId = ConfigurationManager.AppSettings["GetResponse.CampaignToken"];

            if (string.IsNullOrEmpty(campaignTokenId))
                throw new StorefrontException("No Campaign Token Id provided");

            var client = new RestClient(ServerUrl);
            var subscribeRequest = CreateJsonRequest("contacts", Method.POST);

            var contact = new RequestContact
            {
                email = email,
                dayOfCycle = 0,
                campaign = new RequestCampaign { campaignId = campaignTokenId } 
            };

            subscribeRequest.AddBody(contact);
            IRestResponse subscribeResponse = await client.ExecuteTaskAsync(subscribeRequest);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // POST: /storefrontapi/getresponse/unsubscribe
        [HttpPost]
        public async Task<ActionResult> Unsubscribe(string email)
        {
            var client = new RestClient(ServerUrl);

            var getContactRequest = CreateJsonRequest("contacts", Method.GET);
            getContactRequest.AddQueryParameter("query[email]", email);
            IRestResponse getContactResponse = await client.ExecuteTaskAsync(getContactRequest);

            if (getContactResponse.ResponseStatus == ResponseStatus.Completed)
            {
                var contact = JsonConvert.DeserializeObject<IList<ResponseContact>>(getContactResponse.Content).FirstOrDefault();
                if (!string.IsNullOrEmpty(contact?.contactId))
                {
                    var unsubsrcibeRequest = CreateJsonRequest($"contacts/{contact.contactId}", Method.DELETE);
                    IRestResponse unsubsrcibeResponse = await client.ExecuteTaskAsync(unsubsrcibeRequest);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private IRestRequest CreateJsonRequest(string resource, Method method)
        {
            var apiKey = ConfigurationManager.AppSettings["GetResponse.ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new StorefrontException("No API key provided");

            var request = new RestRequest(resource, method)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("X-Auth-Token", $"api-key {apiKey}");

            return request;
        }
    }


    public class RequestContact
    {
        public int dayOfCycle;
        public string email;

        public RequestCampaign campaign { get; set; }
    }

    public class RequestCampaign
    {
        public string campaignId { get; set; }
    }

    public class ResponseContact
    {
        public string contactId { get; set; }
    }
}
