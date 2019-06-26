#addin nuget:?package=Cake.Git&version=0.19.0
#addin "Cake.Powershell"

var target = Argument("target", "Default");
var configuration = Argument("config", "Release");
var packageServerApiKey = Argument("packageServerApiKey", "");
var packageServerSource = Argument("packageServerSource", "https://api.nuget.org/v3/index.json");

GitVersion _versionInfo;
DotNetCoreBuildSettings _dotnetCoreBuildSettings;

IEnumerable<string> _projects = new[]
{
    "./src/InfoTrack.OAuth/InfoTrack.OAuth.csproj",
    "./src/InfoTrack.OAuth.Caching.DotNetCore/InfoTrack.OAuth.Caching.DotNetCore.csproj",
    "./src/InfoTrack.OAuth.Caching.DotNetFramework/InfoTrack.OAuth.Caching.DotNetFramework.csproj",
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
            .Append("/p:IncludeSymbols=false")
    };
});

Task("Clean")
    .Does(() =>
    {
        CleanDirectories("**/bin");
        CleanDirectories("**/obj");
    });

Task("Build")
    .DoesForEach(_projects, (project) =>
    {
        DotNetCoreBuild(project, _dotnetCoreBuildSettings);
    });

Task("Push-Packages")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .WithCriteria(TeamCity.IsRunningOnTeamCity)
    .DoesForEach(_projects, (project) =>
    {
        var projectDirectory = System.IO.Path.GetDirectoryName(project);

        foreach(var package in GetFiles($"{projectDirectory}/**/*.nupkg"))
        {
            NuGetPush(package.FullPath, new NuGetPushSettings { Source = packageServerSource, ApiKey = packageServerApiKey });
        }
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Push-Packages");

RunTarget(target);
