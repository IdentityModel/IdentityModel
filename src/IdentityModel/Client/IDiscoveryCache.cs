using System;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public interface IDiscoveryCache
    {
        TimeSpan CacheDuration { get; set; }

        Task<DiscoveryResponse> GetAsync();
        void Refresh();
    }
}