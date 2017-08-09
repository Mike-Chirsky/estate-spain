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
        public GoogleSheetService()
        {
            
        }
        public void WriteMessage(CallbackUserRequest userMessage)
        {
            var credential = GoogleCredential.FromStream(new FileStream(HostingEnvironment.MapPath("~/App_data/server-key.json"), FileMode.Open))
                .CreateScoped(new[] { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive, SheetsService.Scope.DriveFile });
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Estate-spain-server",
            });
            var spreadsheetId = "1mfrbPhC9j-9qdEL9lRvorz-WMoi9gGrfzLD22qclVGI";
            var request = CreateAppendRequest(userMessage, service, spreadsheetId);
            request.Execute();
        }

        private SpreadsheetsResource.ValuesResource.AppendRequest CreateAppendRequest(CallbackUserRequest data, SheetsService service, string sheetId)
        {
            var bodyRequest = new ValueRange
            {
                Values = new[] { new[] {
                        DateTime.UtcNow.ToString("dd.MM.yyyy"),
                        data.UserName,
                        data.UserPhone,
                        data.UserEmail,
                        data.UserMessage,
                        data.FromUrl,
                        data.ObjectName
                    }
                }
            };
            var tableName = "";
            switch (data.FormType)
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
            var request = service.Spreadsheets.Values.Append(bodyRequest, sheetId, $"{tableName}!A2");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            return request;
        }
    }
}