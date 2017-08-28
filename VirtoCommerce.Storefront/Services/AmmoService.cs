using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model.AmmoCrm;
using VirtoCommerce.Storefront.Model.AmmoCrm.Services;

namespace VirtoCommerce.Storefront.Services
{
    public class AmmoService : IAmmoService
    {
        private CookieContainer _cookies;
        private string _subdomain;
        private string _userName;
        private string _apiKey;
        public async Task<bool> Auth(string userName, string apiKey, string subdomain)
        {
            _subdomain = subdomain;
            _userName = userName;
            _apiKey = apiKey;
            _cookies = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = _cookies
            };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://" + _subdomain + ".amocrm.ru");
                var response = await client.PostAsJsonAsync("/private/api/auth.php?type=json", new { USER_LOGIN = userName, USER_HASH = apiKey });
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> CreateUnsorted(AmmoUnsortedModel unsorted)
        {
            if (_cookies == null)
                throw new Exception("Call Auth before call ExistContact method");
            
           
            var handler = new HttpClientHandler()
            {
                CookieContainer = _cookies
            };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://" + _subdomain + ".amocrm.ru");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var objectFields = GetValues(JsonConvert.SerializeObject(CreateUnsortedRequest(unsorted)));
                var requestString = string.Join("&", objectFields.Select(x => $"{NormilazeToForm(x.Name)}={x.Value}"));
                var requestContent = new StringContent(Uri.EscapeUriString(requestString), Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await client.PostAsync("/api/unsorted/add/?type=json&api_key=" + _apiKey + "&login=" + _userName, requestContent);
                var content = new System.IO.StreamReader((await result.Content.ReadAsStreamAsync())).ReadToEnd();
                return result.IsSuccessStatusCode;
            }
        }


        private Dictionary<string, object> CreateUnsortedLead(AmmoUnsortedModel model)
        {
            var lead = new Dictionary<string, object>();
            var formTypeInfo = GetFormType(model.FormType);
            if (formTypeInfo == null)
                return null;

            var leadName = "";
            if (!string.IsNullOrEmpty(model.UserName))
            {
                leadName = $"{formTypeInfo.Item2} от {model.UserName}";
            }
            else
            {
                leadName = $"{formTypeInfo.Item2}";
            }
            lead.Add("name", leadName);
            lead.Add("custom_fields", new List<object>());
            if (!string.IsNullOrEmpty(model.UserName))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956589,
                    values = new[] { new { value = model.UserName } }
                });
            }
            if (!string.IsNullOrEmpty(model.UserPhone))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956635,
                    values = new[] { new { value = model.UserPhone } }
                });
            }
            if (!string.IsNullOrEmpty(model.UserEmail))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956637,
                    values = new[] { new { value = model.UserEmail } }
                });
            }
            if (!string.IsNullOrEmpty(model.UserMessage))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956645,
                    values = new[] { new { value = model.UserMessage } }
                });
            }
            

            if (!string.IsNullOrEmpty(model.ObjectName))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956641,
                    values = new[] { new { value = model.ObjectName } }
                });
            }
            if (!string.IsNullOrEmpty(model.FromUrl))
            {
                ((List<object>)lead["custom_fields"]).Add(new
                {
                    id = 1956639,
                    values = new[] { new { value = model.FromUrl } }
                });
            }
            ((List<object>)lead["custom_fields"]).Add(new
            {
                id = 1956615,
                values = new[] { new { @enum = formTypeInfo.Item1, value = formTypeInfo.Item2 } }
            });
            ((List<object>)lead["custom_fields"]).Add(new
            {
                id = 1956657,
                values = new[] { new { value = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") } }
            });
            return lead;

        }

        public Tuple<int, string> GetFormType(FormTypes type)
        {
            switch (type)
            {
                case FormTypes.CallBackObject:
                    return new Tuple<int, string>(4542817, "Запрос звонка по объекту с сайта");
                case FormTypes.CallBackOther:
                    return new Tuple<int, string>(4542821, "Общий запрос звонка с сайта");
                case FormTypes.RequestObject:
                    return new Tuple<int, string>(4542819, "Запрос по объекту с сайта");
                case FormTypes.RequestOther:
                    return new Tuple<int, string>(4542823, "Общее обращение в компанию с сайта");
            }
            return null;
        }

        private object CreateUnsortedContact(AmmoUnsortedModel model)
        {
            var contact = new Dictionary<string, object>();
            contact.Add("name", string.IsNullOrEmpty(model.UserName) ? model.FromUrl : model.UserName);
            contact.Add("custom_fields", new List<object>());
            if (!string.IsNullOrEmpty(model.UserPhone))
            {
                ((List<object>)contact["custom_fields"]).Add(new
                {
                    id = 1347104,
                    values = new[] { new { @enum = 3233982, value = model.UserPhone } }
                });
            }
            if (!string.IsNullOrEmpty(model.UserEmail))
            {
                ((List<object>)contact["custom_fields"]).Add(new
                {
                    id = 1347106,
                    values = new[] { new { @enum = 3233988, value = model.UserEmail } }
                });
            }
            return contact;

        }

        private Dictionary<string, object> CreateUnsortedFormData(AmmoUnsortedModel model)
        {
            var fields = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(model.UserName))
            {
                fields.Add("1956589_1", CreateFormField(model.UserName, "1956589", "Имя:"));
            }
            if (!string.IsNullOrEmpty(model.UserEmail))
            {
                fields.Add("1956637_1", CreateFormField(model.UserEmail, "1956637", "Email:"));
            }
            if (!string.IsNullOrEmpty(model.UserPhone))
            {
                fields.Add("1956635_1", CreateFormField(model.UserPhone, "1956635","Телефон:"));
            }
            if (!string.IsNullOrEmpty(model.UserMessage))
            {
                fields.Add("1956645_1", CreateFormField(model.UserMessage, "1956645", "Сообщение:"));
            }
            return fields;
        }

        private Dictionary<string, object> CreateFormField(string value, string id, string name)
        {
            var field = new Dictionary<string, object>();
            field.Add("type", "text");
            field.Add("id", id);
            field.Add("element_type", 1);
            field.Add("value", value);
            field.Add("name", name);
            return field;
        }

        private object CreateUnsortedRequest(AmmoUnsortedModel model)
        {

            return new
            {
                request = new
                {
                    unsorted = new
                    {
                        category = "forms",
                        add = new[] {
                            new {
                                source= model.FromUrl,
                                data = new  { leads = new []{
                                                        CreateUnsortedLead(model)
                                              },
                                              contacts = new []{
                                                    CreateUnsortedContact(model)
                                              }

                                },
                                source_data = new
                                {
                                    data = CreateUnsortedFormData(model),
                                    form_id = 318,
                                    form_type = 1,
                                    origin = new {
                                        ip = "192.168.1.1",
                                        referer = ""
                                    },
                                    date = GetUnixTime(DateTime.UtcNow),
                                    from = model.FromUrl,
                                    form_name = model.FormName
                                }
                            },
                        },
                    }
                }
            };

        }

        private string NormilazeToForm(string str)
        {
            var resultString = new StringBuilder();
            var paths = str.Split('.');
            var isFirst = true;
            foreach (var path in paths)
            {
                var indexQ = path.IndexOf("[");
                if (isFirst)
                {
                    resultString.Append(path);
                    isFirst = false;
                }
                else if (indexQ > -1)
                {
                    resultString.Append($"[{path.Substring(0, indexQ)}]{path.Substring(indexQ, path.Length - indexQ)}");
                }
                else
                {
                    resultString.Append($"[{path}]");
                }
            }
            return resultString.ToString();
        }

        private int GetUnixTime(DateTime dt)
        {
            return (int)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }
       

        public static NameValuePair[] GetValues(string json)
        {
            var jobject = JObject.Parse(json);
            var allTokens = jobject.SelectTokens("..*").ToArray();

            var jvalues = allTokens
                .Select(t => t as JValue)
                .Where(v => v != null && v.Value != null)
                .ToArray();

            var parameters = new List<NameValuePair>();

            foreach (var jvalue in jvalues)
            {
                var value = jvalue.Value;

                if (value is DateTime)
                    value = JsonConvert.ToString(value).Trim('\"');
                else
                    value = string.Format(CultureInfo.InvariantCulture, "{0}", value);

                parameters.Add(new NameValuePair(jvalue.Path, value.ToString()));
            }

            return parameters.ToArray();
        }
        
        public async Task<bool> CreatePrimaryTreatment(AmmoUnsortedModel leadModel)
        {
            if (_cookies == null)
                throw new Exception("Call Auth before call ExistContact method");


            var handler = new HttpClientHandler()
            {
                CookieContainer = _cookies
            };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://" + _subdomain + ".amocrm.ru");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.PostAsJsonAsync("/private/api/v2/json/leads/set", new
                {
                    request = new
                    {
                        leads = new
                        {
                            add = new[] {
                                        CreateLead(leadModel)
                                    }
                        }
                    }
                });

                var content = new System.IO.StreamReader((await result.Content.ReadAsStreamAsync())).ReadToEnd();
                return result.StatusCode == HttpStatusCode.OK;
            }
        }

        private Dictionary<string, object> CreateLead(AmmoUnsortedModel model)
        {
             var lead = CreateUnsortedLead(model);
             lead.Add("status_id", 12546192);
             lead.Add("pipeline_id", 320355);
             lead.Add("responsible_user_id", 1104147);
             lead.Add("created_user_id", 1104147);
             lead.Add("date_create", GetUnixTime(DateTime.UtcNow));
             return lead;
        }
    }
}
