using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class NumericRange:Drop
    {
        public decimal? Lower { get; set; }
        public decimal? Upper { get; set; }
        public bool IncludeLower { get; set; }
        public bool IncludeUpper { get; set; }

        public override string ToString()
        {
            return $"{(IncludeLower ? "[" : "(")}{(Lower.HasValue ? Lower.ToString() : "")} TO {(Upper.HasValue ? Upper.ToString() : "")}{(IncludeUpper ? "]" : ")")}";
        }

        public override int GetHashCode()
        {
            return Lower.GetHashCode() + Upper.GetHashCode() + IncludeLower.GetHashCode() + IncludeUpper.GetHashCode();
        }
    }
}
