#tool "nuget:?package=GitVersion.CommandLine"

var buildTarget = Argument("target","Default");
var configuration = Argument("configuration", "Release");

var workingFolder = Directory("./").ToString();
var buildDir = Directory("./EventSource.Framework/bin/") + Directory(configuration);
var sln = "./EventSourceFramework.sln";

var appName = "EventSource.Framework";
var version = "1.0.0.0";

Task("Default").IsDependentOn("Pack");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
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
                                    Authors                 = new[] {""},
                                    Description             = info.Description,
                                    Summary                 = info.Description,
                                    ProjectUrl              = new Uri("https://github.com/tomlazelle/EventSourceFramework.git"),
                                    Files                   = new [] {
                                                                        new NuSpecContent {Source = @"*" ,Target = @"lib\net45\"},
                                                                      },
                                    BasePath                = "./bin",
                                    OutputDirectory         = "./nuget"
                                };

    NuGetPack(nuGetPackSettings);
});

 

  

// Task("Generate-Swagger-Json")
//   .IsDependentOn("Pack-Api")
//   .Does(()=>{

//     var root = System.IO.Directory.GetCurrentDirectory();
//     var swagGen = System.IO.Path.GetFullPath(Directory("./tools/WebApiSwaggerGenerator/lib/net45").Path.FullPath) + @"\WebApiSwaggerGenerator.exe";
    
//     StartAndReturnProcess(swagGen,
//       new ProcessSettings{
//         Arguments = "-a" + root + @"\bin\ShippingServices.dll -o" + root + @"\swagger.json"
//     });
// });

// Task("Generate-Api-SDK")
//   .IsDependentOn("Generate-Swagger-Json")
//   .Does(() =>
// {
//     var root = System.IO.Directory.GetCurrentDirectory();
//     var autoRest = System.IO.Path.GetFullPath(Directory("./tools/autorest/tools").Path.FullPath) + @"\AutoRest.exe";

//     StartProcess(autoRest,
//       new ProcessSettings{
//         Arguments= "-OutputDirectory " + root + @"\generated -Namespace BAS.ShippingService.Api.Client -Input " + root + @"\swagger.json -AddCredentials"
//       });
// }).ReportError(exception =>
// {  
//     Information(exception.ToString());
// });

// Task("Build-SDK")
// .IsDependentOn("Generate-Api-SDK")
// .Does(()=>{


//   var parameters = new []{
//     @"/recurse:{workingFolder}\generated\*.cs",
//     @"/out:{workingFolder}\generated\BAS.ShippingService.Api.Client.dll",
//     @"/reference:{workingFolder}\tools\Microsoft.Rest.ClientRuntime\lib\net45\Microsoft.Rest.ClientRuntime.dll",
//     @"/reference:{workingFolder}\tools\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll",
//     @"/reference:System.Net.Http.dll", 
//     @"/target:library"  
//   };

//   var root = System.IO.Directory.GetCurrentDirectory();

//   for (int i = 0; i < parameters.Length; i++)
//   {
//       parameters[i] = parameters[i].Replace("{workingFolder}",root);
//   }

  
//   StartProcess(@"C:\Program Files (x86)\MSBuild\14.0\bin\csc.exe", string.Join(" ",parameters) + " ");

// });

RunTarget(buildTarget);