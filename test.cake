var buildTarget = Argument("target","Default");

Task("Default").IsDependentOn("Pack");

var relativeVersion = "1.0.0.0";

Task("Pack")
  .Does(() => {

    var versionInfo = XmlPeek("./EventSource.Framework/EventSource.Framework.csproj", "/Project/PropertyGroup/AssemblyVersion");

    var ver = new System.Version(versionInfo);

    var major = ver.Major;
    var minor = ver.Minor;
    var build = ver.Build;
    var rev = ver.Revision;

    var buildnumber = EnvironmentVariable("BUILD_NUMBER");

    if(buildnumber != null && buildnumber != "")
    {
        relativeVersion = System.String.Format("{0}.{1}.{2}.{3}",major,minor,buildnumber,rev);
    }
    else
    {
        rev += 1;

        relativeVersion = System.String.Format("{0}.{1}.{2}.{3}",major,minor,build,rev);
        
        XmlPoke("./EventSource.Framework/EventSource.Framework.csproj", "/Project/PropertyGroup/AssemblyVersion",relativeVersion);
        XmlPoke("./EventSource.Framework/EventSource.Framework.csproj", "/Project/PropertyGroup/AssemblyFileVersion",relativeVersion);
        XmlPoke("./EventSource.Framework/EventSource.Framework.csproj", "/Project/PropertyGroup/Version",relativeVersion);
        
        Information(relativeVersion);
    }

    
});

RunTarget(buildTarget);