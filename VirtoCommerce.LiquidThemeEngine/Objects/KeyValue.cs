using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public sealed class KeyValue<TKey, TValue>:Drop
    {
        public TKey Key { private set; get; }
        public TValue Value { private set; get; }

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
             
    }
}
