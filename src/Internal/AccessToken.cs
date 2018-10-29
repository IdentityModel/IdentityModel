using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.Internal
{
    /// <summary>
    /// Helper class that represent token and expire date.
    /// </summary>
    internal struct AccessToken
    {
        private readonly DateTime _expirationDate;

        public AccessToken(string accessToken, DateTime expirationDate)
        {
            Token = accessToken;
            _expirationDate = expirationDate;
        }

        /// <summary>
        /// Token value.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Is token expired?
        /// </summary>
        public bool IsExpired => _expirationDate <= DateTime.Now;
    }
}
