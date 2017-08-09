using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using VirtoCommerce.Storefront.Model.AmmoCrm;
using VirtoCommerce.Storefront.Model.UserForms;

namespace VirtoCommerce.Storefront.Services.Es
{
    public class GoogleSheetService : IGoogleSheetService
    {
        private const string _appName = "Estate-spain-server";
        private const string _spredsheetId = "1mfrbPhC9j-9qdEL9lRvorz-WMoi9gGrfzLD22qclVGI";

        public void WriteMessage(CallbackUserRequest userMessage)
        {

            var service = CreateSheetService();
            var data =  new[] {
                        DateTime.UtcNow.ToString("dd.MM.yyyy"),
                        userMessage.UserName,
                        userMessage.UserPhone,
                        userMessage.UserEmail,
                        userMessage.UserMessage,
                        userMessage.FromUrl,
                        userMessage.ObjectName
            };
            var tableName = "";
            switch (userMessage.FormType)
            {
                case FormTypes.CallBackObject:
                    tableName = "Заказ обратного звонка из объекта";
                    break;
                case FormTypes.CallBackOther:
                    tableName = "Заказ обратного звонка";
                    break;
                case FormTypes.RequestObject:
                    tableName = "Запрос объекта";
                    break;
                case FormTypes.RequestOther:
                    tableName = "Другие запросы";
                    break;
            }
            var request = CreateAppendRequest(data, service, tableName);
            request.Execute();
        }

        public void WriteSubscribe(string email)
        {
            var service = CreateSheetService();
            var request = CreateAppendRequest(new[] { DateTime.UtcNow.ToString("dd.MM.yyyy"), email }, service, "Подписки");
            request.Execute();
        }

        private SpreadsheetsResource.ValuesResource.AppendRequest CreateAppendRequest(string[] data, SheetsService service, string tableName)
        {
            var bodyRequest = new ValueRange
            {
                Values = new[] { data }
            };
            var request = service.Spreadsheets.Values.Append(bodyRequest, _spredsheetId, $"{tableName}!A2");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            return request;
        }

        private SheetsService CreateSheetService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromStream(new FileStream(HostingEnvironment.MapPath("~/App_data/server-key.json"), FileMode.Open))
               .CreateScoped(new[] { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive, SheetsService.Scope.DriveFile }),
                ApplicationName = _appName,
            });
        }
    }
}