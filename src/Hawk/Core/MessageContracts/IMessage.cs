using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Hawk.Core.MessageContracts
{
    /// <summary>
    /// Represents the elements of an HTTP message common to both request and response.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Message body
        /// </summary>
        Task<string> ReadBodyAsStringAsync();

        /// <summary>
        /// Content type of the message body, if any.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Message headers
        /// </summary>
        IDictionary<string, string[]> Headers { get; }
    }
}
