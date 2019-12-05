using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Confighub.Microsoft.Extensions.Configuration.Extensions
{
    internal static class HelperExtensions
    {
        public static Tuple<string, object, string> ExtractKeyValue(this IDictionary<string, string> data, string key)
        {
            if (Regex.IsMatch(key, @"\:\@\d+$"))
            {
                var listKey = key.Substring(0, key.IndexOf(":@"));
                var listItems = data.Where(kvp => kvp.Key.StartsWith(listKey + ":@")).OrderBy(x => Convert.ToInt32(x.Key.Substring(x.Key.IndexOf(":@") + 2)));
                var items = new List<string>();
                foreach (var item in listItems)
                {
                    var itemPosition = Convert.ToInt32(item.Key.Substring(item.Key.IndexOf(":@") + 2));
                    items.Insert(itemPosition, item.Value);
                }
                return new Tuple<string, object, string>(listKey.Replace(":", "."), JsonConvert.SerializeObject(items), "List");
            }

            if (int.TryParse(data[key], out int intValue))
            {
                return new Tuple<string, object, string>(key.Replace(":", "."), intValue, "Integer");
            }

            if (bool.TryParse(data[key], out bool boolValue))
            {
                return new Tuple<string, object, string>(key.Replace(":", "."), boolValue, "Boolean");
            }

            if (long.TryParse(data[key], out long longValue))
            {
                return new Tuple<string, object, string>(key.Replace(":", "."), longValue, "Long");
            }

            if (double.TryParse(data[key], out double doubleValue))
            {
                return new Tuple<string, object, string>(key.Replace(":", "."), doubleValue, "Double");
            }

            if (float.TryParse(data[key], out float floatValue))
            {
                return new Tuple<string, object, string>(key.Replace(":", "."), floatValue, "Float");
            }

            return new Tuple<string, object, string>(key.Replace(":", "."), data[key], "Text");
        }
    }
}
