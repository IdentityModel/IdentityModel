using System.IO;

namespace IdentityModel.UnitTests
{
    static class FileName
    {
        public static string Create(string name)
        {
#if NETCOREAPP1_1 || NETCOREAPP2_0
            var fullName = Path.Combine(System.AppContext.BaseDirectory, "documents", name);
#else
            var fullName = Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "documents", name);
#endif

            return fullName;
        }
    }
}
