using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public class DiscoveryDocumentRequest
    {
        public string Address { get; set; }
        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();
    }
}
