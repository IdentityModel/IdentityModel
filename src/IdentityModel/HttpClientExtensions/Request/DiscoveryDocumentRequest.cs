using IdentityModel.Client;

namespace IdentityModel.HttpClientExtensions
{
    public class DiscoveryDocumentRequest
    {
        public string Address { get; set; }
        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();
    }
}
