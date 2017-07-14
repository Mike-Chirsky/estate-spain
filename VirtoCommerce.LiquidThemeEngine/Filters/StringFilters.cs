using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotLiquid;
using DotLiquid.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// String filters are used to manipulate outputs and variables of the string type.
    /// https://docs.shopify.com/themes/liquid-documentation/filters/string-filters
    /// </summary>
    public class StringFilters
    {
        private static readonly Regex TagSyntax = R.B(R.Q(@"([A-Za-z0-9]+)_([A-Za-z0-9].+)"));
        private static readonly Regex WordReplaceSyntax = R.B(R.Q(@"[^\w\s-]"));
        private static readonly Regex WordReplaceSyntax2 = R.B(R.Q(@"[\s-]+"));

        /// <summary>
        /// Converts a string into CamelCase.
        /// {{ 'coming-soon' | camelcase }}
        /// Result - ComingSoon
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Camelize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string result = "";

            string[] strArray = input.Split('_', '-');
            foreach (string word in strArray)
            {
                result += word.Substring(0, 1).ToUpper() + word.Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Formats a string into a handle.
        /// Input
        ///{{ '100% M & Ms!!!' | handleize }}
        /// Output
        /// 100-m-ms
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Handleize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Handelize();
        }

        /// <summary>
        /// Converts a string into an MD5 hash.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            }

            return hash.ToHex(upperCase: false);
        }

        /// <summary>
        /// Outputs the singular or plural version of a string based on the value of a number. The first parameter is the singular string and the second parameter is the plural string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="singular"></param>
        /// <param name="plural"></param>
        /// <returns></returns>
        public static string Pluralize(int input, string singular, string plural)
        {
            return input == 1 ? singular : plural;
        }


        /// <summary>
        /// Strips tabs, spaces, and newlines (all whitespace) from the left and right side of a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Strip(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Trim();
        }

        /// <summary>
        /// Declension word after number
        /// </summary>
        /// <param name="input">Number</param>
        public static string NumEnding(object input, string wordOne, string wordFour, string wordFive)
        {
            if (Regex.Match(input.ToString(), "1\\d$").Success)
                return $"{input} {wordFive}";
            if (Regex.Match(input.ToString(), "1$").Success)
                return $"{input} {wordOne}";
            if (Regex.Match(input.ToString(), "(2|3|4)$").Success)
                return $"{input} {wordFour}";
            return $"{input} {wordFive}";
            /*
            int number;
            if (!int.TryParse(input.ToString(), out number))
            {
                return $"{input} {wordOne}";
            }
            number %= 100;
            if (number > 11 && number <= 19)
                
            number %= 10;
            switch (number)
            {
                case 1:
                    return $"{input} {wordOne}";
                case 2:
                case 3:
                case 4:
                    return $"{input} {wordFour}";
            }
            return $"{input} {wordFive}";*/
        }
    }

    public class RubyContractResolver : DefaultContractResolver
    {
        #region Methods
        protected override string ResolvePropertyName(string propertyName)
        {
            return Template.NamingConvention.GetMemberName(propertyName);
        }
        #endregion
    }
}
