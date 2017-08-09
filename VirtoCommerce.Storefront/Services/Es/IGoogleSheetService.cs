using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.UserForms;

namespace VirtoCommerce.Storefront.Services.Es
{
    public interface IGoogleSheetService
    {
        void WriteMessage(CallbackUserRequest userMessage);
    }
}
