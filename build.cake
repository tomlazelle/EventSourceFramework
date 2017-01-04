#tool "nuget:?package=GitVersion.CommandLine"

var buildTarget = Argument("target","Default");
var configuration = Argument("configuration", "Release");

var workingFolder = Directory("./").ToString();
var buildDir = Directory("./EventSource.Framework/bin/") + Directory(configuration);
var sln = "./EventSourceFramework.sln";

var appName = "EventSource.Framework";
var version = "1.0.0.0";

Task("Default").IsDependentOn("Push-Nuget");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);

    if(DirectoryExists("./nuget"))
        {
            CleanDirectory("./nuget");
        }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(sln);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    
      // Use MSBuild
      MSBuild(sln, settings =>
        settings.SetConfiguration(configuration));
    
});


Task("Create-Directory")
.IsDependentOn("Build")
.Does(()=>{

  EnsureDirectoryExists("nuget");
});


// Task("UpdateAssemblyInfo")
// .IsDependentOn("Create-Directory")
//     .Does(() =>
// {
//     GitVersion(new GitVersionSettings {
//         UpdateAssemblyInfo = true
//     });
// });

GitVersion relativeVersion = null;

Task("GetVersionInfo")
.IsDependentOn("Create-Directory")
    .Does(() =>
{
    relativeVersion = GitVersion(new GitVersionSettings {
        UserName = "tomlazelle",
        Password = "T0msl1ck",
        Url = "https://github.com/tomlazelle/EventSourceFramework.git",
        Branch = "master"
        // Commit = EnviromentVariable("MY_COMMIT")
    });
    // Use result for building nuget packages, setting build server version, etc...
});

Task("Pack")
  .IsDependentOn("GetVersionInfo")
  .Does(() => {

var info = ParseAssemblyInfo("./EventSource.Framework/Properties/AssemblyInfo.cs");

    
    Information(info.Title);
    Information(info.Description);
    Information(info.Product);

       var nuGetPackSettings   = new NuGetPackSettings {
                                    Id                      = info.Product,
                                    Version                 = relativeVersion.NuGetVersion,
                                    Title                   = info.Title,
                                    Authors                 = new[] {"Tom La Zelle"},
                                    Description             = info.Description,
                                    Summary                 = info.Description,
                                    ProjectUrl              = new Uri("https://github.com/tomlazelle/EventSourceFramework.git"),
                                    Files                   = new [] {
                                                                        new NuSpecContent {Source = @"EventSource.Framework.*" ,Target = @"lib\net45\"},
                                                                      },
                                    Dependencies = new []{                                        
                                        new NuSpecDependency{
                                            Id="StructureMap",
                                            TargetFramework = "net452"
                                        }
                                    },
                                    BasePath                = "./EventSource.Framework/bin",
                                    OutputDirectory         = "./nuget"
                                };

    NuGetPack(nuGetPackSettings);
});

 //43b006e1-e30a-4a0e-af42-b04ac797ce0a

 Task("Push-Nuget")
 .IsDependentOn("Pack")
 .Does(()=>{
// Get the path to the package.
var package = "./nuget/EventSource.Framework." + relativeVersion.NuGetVersion + ".nupkg";
            
// Push the package.
NuGetPush(package, new NuGetPushSettings {
    Source = "https://www.myget.org/F/tomlazelle/api/v2/package",
    ApiKey = "43b006e1-e30a-4a0e-af42-b04ac797ce0a"
});

 });

RunTarget(buildTarget);