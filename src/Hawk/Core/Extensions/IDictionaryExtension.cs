using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Hawk.Core.Extensions
{
    public static class IDictionaryExtension
    {
        public static string FirstOrDefault(this IDictionary<string, string[]> headers, string headerName)
        {
            string value = null;

            string[] values = null;
            if (headers.TryGetValue(headerName, out values))
                value = values[0];

            return value;

        }
    }
}
