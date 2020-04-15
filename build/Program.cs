using System;
using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    internal static class Program
    {
        private const string packOutput = "./artifacts";
        private const string envVarMissing = " environment variable is missing. Aborting.";

        private static class Targets
        {
            public const string RestoreTools = "restore-tools";
            public const string CleanBuildOutput = "clean-build-output";
            public const string CleanPackOutput = "clean-pack-output";
            public const string Build = "build";
            public const string Test = "test";
            public const string Pack = "pack";
            public const string SignBinary = "sign-binary";
            public const string SignPackage = "sign-package";
        }

        internal static void Main(string[] args)
        {
            Target(Targets.RestoreTools, () =>
            {
                Run("dotnet", "tool restore");
            });

            Target(Targets.CleanBuildOutput, () =>
            {
                Run("dotnet", "clean -c Release -v m --nologo");
            });

            Target(Targets.Build, DependsOn(Targets.CleanBuildOutput), () =>
            {
                Run("dotnet", "build -c Release");
            });

            Target(Targets.SignBinary, DependsOn(Targets.Build, Targets.RestoreTools), () =>
            {
                Sign("./src/bin/Release", "IdentityModel.dll");
            });

            Target(Targets.Test, DependsOn(Targets.Build), () =>
            {
                Run("dotnet", "test -c Release --no-build");
            });

            Target(Targets.CleanPackOutput, () =>
            {
                if (Directory.Exists(packOutput))
                {
                    Directory.Delete(packOutput, true);
                }
            });

            Target(Targets.Pack, DependsOn(Targets.Build, Targets.CleanPackOutput), () =>
            {
                Run("dotnet", $"pack ./src/IdentityModel.csproj -c Release -o {Directory.CreateDirectory(packOutput).FullName} --no-build");
            });

            Target(Targets.SignPackage, DependsOn(Targets.Pack, Targets.RestoreTools), () =>
            {
                Sign(packOutput, "*.nupkg");
            });

            Target("default", DependsOn(Targets.Test, Targets.Pack));

            Target("sign", DependsOn(Targets.SignBinary, Targets.Test, Targets.SignPackage));

            RunTargetsAndExit(args, ex => ex is SimpleExec.NonZeroExitCodeException || ex.Message.EndsWith(envVarMissing));
        }

        private static void Sign(string path, string searchTerm)
        {
            var signClientConfig = Environment.GetEnvironmentVariable("SignClientConfig");
            var signClientSecret = Environment.GetEnvironmentVariable("SignClientSecret");

            if (string.IsNullOrWhiteSpace(signClientConfig))
            {
                throw new Exception($"SignClientConfig{envVarMissing}");
            }

            if (string.IsNullOrWhiteSpace(signClientSecret))
            {
                throw new Exception($"SignClientSecret{envVarMissing}");
            }

            foreach (var file in Directory.GetFiles(path, searchTerm, SearchOption.AllDirectories))
            {
                Console.WriteLine($"  Signing {file}");
                Run("dotnet", $"SignClient sign -c {signClientConfig} -i {file} -r sc-ids@dotnetfoundation.org -s \"{signClientSecret}\" -n 'IdentityServer4'", noEcho: true);
            }
        }
    }
}
