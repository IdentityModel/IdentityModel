using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


namespace Thinktecture.IdentityModel.Hawk.Core.Helpers
{
    /// <summary>
    /// Generates a nonce to be used by a .NET client.
    /// </summary>
    public class NonceGenerator
    {
        private static readonly RandomNumberGenerator s_RnGenerator = RandomNumberGenerator.Create();

        /// <summary>
        /// Generates a nonce matching the specified length and returns the same. Default length is 6.
        /// </summary>
        public static string Generate(int length = 6)
        {
            var random = new List<char>();

            do
            {
                byte[] bytes = new byte[length * 8];
                s_RnGenerator.GetBytes(bytes);

                var characters = bytes.Where(b => (b >= 48 && b <= 57) ||       // 0 - 9
                                                    (b >= 97 && b <= 122) ||    // a - z
                                                    (b >= 65 && b <= 90))       // A - Z
                    .Take(length - random.Count)
                    .Select(b => Convert.ToChar(b));

                random.AddRange(characters);
            }
            while (random.Count < length);

            return new string(random.ToArray());
        }
    }
}
