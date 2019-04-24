using System;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build
{
    class Program
    {
        private const bool RequireTests = true;

        private const string ArtifactsDir = "artifacts";
        private const string Build = "build";
        private const string Test = "test";
        private const string Pack = "pack";
        
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            var sign = app.Option<(bool hasValue, int theValue)>("--sign", "Sign binaries and nuget package", CommandOptionType.SingleOrNoValue);

            CleanArtifacts();

            app.OnExecute(() =>
            {
                Target(Build, () => 
                {
                    var solution = Directory.GetFiles(".", "*.sln", SearchOption.TopDirectoryOnly).First();

                    Run("dotnet", $"build {solution} -c Release");

                    if (sign.HasValue())
                    {
                        Sign("*.dll", "./src/bin/release");
                    }
                });

                Target(Test, DependsOn(Build), () => 
                {
                    try
                    {
                        var tests = Directory.GetFiles("./test", "*.csproj", SearchOption.AllDirectories);

                        foreach (var test in tests)
                        {
                            Run("dotnet", $"test {test} -c Release --no-build");
                        }    
                    }
                    catch (System.IO.DirectoryNotFoundException ex)
                    {
                        if (RequireTests)
                        {
                            throw new Exception($"No tests found: {ex.Message}");
                        };
                    }
                });
                
                Target(Pack, DependsOn(Build), () => 
                {
                    var project = Directory.GetFiles("./src", "*.csproj", SearchOption.TopDirectoryOnly).First();

                    Run("dotnet", $"pack {project} -c Release -o ../{ArtifactsDir} --no-build");
                    
                    if (sign.HasValue())
                    {
                        Sign("*.nupkg", $"./{ArtifactsDir}");
                    }
                });


                Target("default", DependsOn(Test, Pack));
                RunTargetsAndExit(app.RemainingArguments);
            });

            app.Execute(args);
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
                Run("../../tools/signclient", $"sign -c {signClientConfig} -i {file} -r sc-ids@dotnetfoundation.org -s \"{signClientSecret}\" -n 'IdentityServer4'", noEcho: true);
            }
        }

        private static void CleanArtifacts()
        {
            Directory.CreateDirectory($"./{ArtifactsDir}");

            foreach (var file in Directory.GetFiles($"./{ArtifactsDir}"))
            {
                File.Delete(file);
            }
        }
    }
}