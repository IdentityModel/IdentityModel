using System;
using System.IO;
using System.Linq;
using Bullseye;
using McMaster.Extensions.CommandLineUtils;
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
        }

        static string BinaryToSign = "IdentityModel.dll";

        
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption();
            var sign = app.Option<(bool hasValue, int theValue)>("--sign", "Sign binaries and nuget package", CommandOptionType.SingleOrNoValue);

            // translate from Bullseye to McMaster.Extensions.CommandLineUtils
            var targets = app.Argument("targets", "The targets to run or list.", true);
            foreach (var option in Options.Definitions)
            {
                app.Option((option.ShortName != null ? $"{option.ShortName}|" : "") + option.LongName, option.Description, CommandOptionType.NoValue);
            }

            CleanArtifacts();

            app.OnExecute(() =>
            {
                // translate from McMaster.Extensions.CommandLineUtils to Bullseye
                var targets = app.Arguments[0].Values;
                var options = new Options(Options.Definitions.Select(d => (d.LongName, app.Options.Single(o => "--" + o.LongName == d.LongName).HasValue())));

                Target(Targets.Build, () => 
                {
                    Run("dotnet", $"build -c Release");

                    if (sign.HasValue())
                    {
                        Sign(BinaryToSign, "./src/bin/release");
                    }
                });

                Target(Targets.Test, DependsOn(Targets.Build), () => 
                {
                    Run("dotnet", $"test -c Release --no-build");
                });
                
                Target(Targets.Pack, DependsOn(Targets.Test), () => 
                {
                    var project = Directory.GetFiles("./src", "*.csproj", SearchOption.TopDirectoryOnly).First();

                    Run("dotnet", $"pack {project} -c Release -o ./artifacts --no-build");
                    
                    if (sign.HasValue())
                    {
                        Sign("*.nupkg", $"./artifacts");
                    }
                });


                Target("default", DependsOn(Targets.Test, Targets.Pack));
                RunTargetsAndExit(targets, options);
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