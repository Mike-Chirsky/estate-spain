using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.AmmoCrm;
using VirtoCommerce.Storefront.Model.AmmoCrm.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.UserForms;
using VirtoCommerce.Storefront.Services.Es;

namespace VirtoCommerce.Storefront.Controllers.Api.Es
{
    public class ApiUserFormsController : StorefrontControllerBase
    {
        private readonly IAmmoService _ammoService;
        private readonly IGoogleSheetService _googleSheetService;
        private readonly string _ammoUserName;
        private readonly string _ammoApiKey;
        private readonly string _ammoSubdomain;
        
        public ApiUserFormsController(WorkContext context, IStorefrontUrlBuilder urlBuilder, IAmmoService ammoService, IGoogleSheetService googleSheetService) : base(context, urlBuilder)
        {
            _ammoService = ammoService;
            _googleSheetService = googleSheetService;
            _ammoUserName = ConfigurationManager.AppSettings["AmmoUser"];
            _ammoSubdomain = ConfigurationManager.AppSettings["AmmoSubdomain"];
            _ammoApiKey = ConfigurationManager.AppSettings["AmmoApiKey"];
        }

        // POST: storefrontapi/forms
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> UserForm(CallbackUserRequest request)
        {
            var errorSendAmmo = false;
            var errorSendGoogle = false;
            try
            {
                if (await _ammoService.Auth(_ammoUserName, _ammoApiKey, _ammoSubdomain))
                {
                    await _ammoService.CreatePrimaryTreatment(new AmmoUnsortedModel
                    {
                        FormName = request.FormName,
                        FormType = request.FormType,
                        FormTitle = request.FormTitle,
                        FromUrl = request.FromUrl,
                        UserEmail = request.UserEmail,
                        UserMessage = request.UserMessage,
                        UserName = request.UserName,
                        UserPhone = request.UserPhone,
                        ObjectName = request.ObjectName
                    });
                }
                else
                {
                    errorSendAmmo = true;
                }
            }
            catch
            {
                errorSendAmmo = true;
            }
            try
            {
                _googleSheetService.WriteMessage(request);
            }
            catch
            {
                errorSendGoogle = true;
            }
            if (errorSendGoogle && errorSendAmmo)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return Json("ok");
        }
    }
}
