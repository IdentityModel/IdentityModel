var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var packPath            = Directory("./src/IdentityModel");
var sourcePath          = Directory("./src");
var testsPath           = Directory("test");
var buildArtifacts      = Directory("./artifacts/packages");

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	var projects = GetFiles("./**/project.json");

	foreach(var project in projects)
	{
        var settings = new DotNetCoreBuildSettings 
        {
            Configuration = configuration
            // Runtime = IsRunningOnWindows() ? null : "unix-x64"
        };

	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

Task("RunTests")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var projects = GetFiles("./test/**/project.json");

    foreach(var project in projects)
	{
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration
        };

        DotNetCoreTest(project.GetDirectory().FullPath, settings);
    }
});

Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
    };

    // add build suffix for CI builds
    if(!isLocalBuild)
    {
        settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
    }

    DotNetCorePack(packPath, settings);
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    DotNetCoreRestore(sourcePath, settings);
    //DotNetCoreRestore(testsPath, settings);
});

Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("RunTests")
  .IsDependentOn("Pack");

RunTarget(target);