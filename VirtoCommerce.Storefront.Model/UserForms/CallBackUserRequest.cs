using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.AmmoCrm;

namespace VirtoCommerce.Storefront.Model.UserForms
{
    public class CallbackUserRequest
    {

        public string UserName { set; get; }
        public string UserEmail { set; get; }
        public string FromUrl { set; get; }
        public string UserPhone { set; get; }
        public string UserMessage { set; get; }
        public string FormName { set; get; }
        public FormTypes FormType { set; get; }
        public string FormTitle { set; get; }
        public string ObjectName { set; get; }
    }
}
