#addin nuget:?package=Cake.Git&version=0.19.0

var target = Argument("target", "Default");
var configuration = Argument("config", "Release");
var nugetApiKey = Argument("nugetApiKey", "");

GitVersion _versionInfo;
DotNetCoreBuildSettings _dotnetCoreBuildSettings;

IEnumerable<string> _projects = new[]
{
    "./src/InfoTrack.Authentication/InfoTrack.Authentication.csproj",
    "./src/InfoTrack.Authentication.Caching.DotNetCore/InfoTrack.Authentication.Caching.DotNetCore.csproj",
    "./src/InfoTrack.Authentication.Caching.DotNetFramework/InfoTrack.Authentication.Caching.DotNetFramework.csproj",
};

Setup((setupContext) => {
    _versionInfo = GitVersion(new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json
    });

    TeamCity.SetBuildNumber(_versionInfo.NuGetVersion);

    _dotnetCoreBuildSettings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = (args) => args
            .Append($"/p:AssemblyVersion={_versionInfo.AssemblySemVer}")
            .Append($"/p:PackageVersion={_versionInfo.NuGetVersion}")
            .Append($"/p:FileVersion={_versionInfo.Major}.{_versionInfo.Minor}.{_versionInfo.Patch}")
            .Append($"/p:InformationalVersion={_versionInfo.InformationalVersion}")
            .Append($"/p:IncludeSymbols=true")
    };
});

Task("Build")
    .DoesForEach(_projects, (project) =>
    {
        DotNetCoreBuild(project, _dotnetCoreBuildSettings);
    });

Task("Push-Packages")
    .IsDependentOn("Build")
    .WithCriteria(TeamCity.IsRunningOnTeamCity)
    .Does(() =>
    {
        var packages = GetFiles("**/*.symbols.nupkg");

        NuGetPush(packages, new NuGetPushSettings {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = nugetApiKey
        });
    });

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Push-Packages");

RunTarget(target);
