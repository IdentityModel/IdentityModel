var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var packPath            = Directory("./src/IdentityModel");
var buildArtifacts      = Directory("./artifacts/packages");

var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var isWindows           = IsRunningOnWindows();
var netcore             = "netcoreapp1.1";
var netstandard         = "netstandard1.3";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration
    };

    // libraries
	var projects = GetFiles("./src/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - building only for " + netstandard);
        settings.Framework = netstandard;
    }

	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }

    // tests
	projects = GetFiles("./test/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - building only for " + netcore);
        settings.Framework = netcore;
    }

	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    var projects = GetFiles("./test/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - testing only for " + netcore);
        settings.Framework = netcore;
    }

    foreach(var project in projects)
	{
        DotNetCoreTest(project.FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    if (!isWindows)
    {
        Information("Not on Windows - skipping pack");
    }

    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
    };

    // add build suffix for CI builds
    if(isAppVeyor)
    {
        settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
    }

    DotNetCorePack(packPath, settings);
});


Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

RunTarget(target);