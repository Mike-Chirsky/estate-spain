using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class ProductSearchCriteria
    {
        private IList<Term> _mutableTerms;
        /// <summary>
        /// For set from query
        /// </summary>
        public IList<Term> MutableTerms
        {
            set
            {
                Terms = value == null ? new Term[0] : value.ToArray();
                _mutableTerms = value;
            }
            get
            {
                return _mutableTerms;
            }
        }
    }
}
