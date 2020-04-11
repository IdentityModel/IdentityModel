using System;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    class Program
    {
        private static class Targets
        {
            public const string Build = "build";
            public const string Test = "test";
            public const string Pack = "pack";
            public const string SignBinary = "sign-binary";
            public const string SignPackage = "sign-package";
        }

        static string BinaryToSign = "IdentityModel.dll";

        
        static void Main(string[] args)
        {
            CleanArtifacts();

            Target(Targets.Build, () =>
            {
                Run("dotnet", $"build -c Release");
            });

            Target(Targets.SignBinary, DependsOn(Targets.Build), () =>
            {
                Sign(BinaryToSign, "./src/bin/release");
            });

            Target(Targets.Test, DependsOn(Targets.Build), () =>
            {
                Run("dotnet", $"test -c Release --no-build");
            });

            Target(Targets.Pack, DependsOn(Targets.Build), () =>
            {
                var project = Directory.GetFiles("./src", "*.csproj", SearchOption.TopDirectoryOnly).First();

                Run("dotnet", $"pack {project} -c Release -o ./artifacts --no-build");
            });

            Target(Targets.SignPackage, DependsOn(Targets.Pack), () =>
            {
                Sign("*.nupkg", $"./artifacts");
            });

            Target("default", DependsOn(Targets.Test, Targets.Pack));

            Target("sign", DependsOn(Targets.SignBinary, Targets.Test, Targets.SignPackage));

            RunTargetsAndExit(args);
        }

        private static void Sign(string extension, string directory)
        {
            var signClientConfig = Environment.GetEnvironmentVariable("SignClientConfig");
            var signClientSecret = Environment.GetEnvironmentVariable("SignClientSecret");

            if (string.IsNullOrWhiteSpace(signClientConfig))
            {
                throw new Exception("SignClientConfig environment variable is missing. Aborting.");
            }

            if (string.IsNullOrWhiteSpace(signClientSecret))
            {
                throw new Exception("SignClientSecret environment variable is missing. Aborting.");
            }

            var files = Directory.GetFiles(directory, extension, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Console.WriteLine("  Signing " + file);
                Run("dotnet", $"SignClient sign -c {signClientConfig} -i {file} -r sc-ids@dotnetfoundation.org -s \"{signClientSecret}\" -n 'IdentityServer4'", noEcho: true);
            }
        }

        private static void CleanArtifacts()
        {
            Directory.CreateDirectory($"./artifacts");

            foreach (var file in Directory.GetFiles($"./artifacts"))
            {
                File.Delete(file);
            }
        }
    }
}