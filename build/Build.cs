using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
[GitHubActions("ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    OnPushBranches = new[] { "master" },
    OnPullRequestBranches = new[] { "master" },
    InvokedTargets = new[] { nameof(Push) },
    ImportSecrets = new[] { "NUGET_API_KEY" })]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Push);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json"; //default
    [Parameter] string NugetApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net6.0")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src" / "FlintSoft.WorkTime";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory
            .GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => {
            DotNetTest(s => s
                .SetProjectFile(Solution)
            );
        });

    Target Publish => _ => _
    .DependsOn(Test)
    .Executes(() =>
    {
        DotNetPublish(_ => _.SetOutput(OutputDirectory)
        .SetProject(Solution));
    });

    Target Pack => _ => _
        .DependsOn(Publish)
        .Executes(() => {
                Console.WriteLine("Conf: " + Configuration);
                Console.WriteLine("Version: " + GitVersion.NuGetVersionV2);
                Console.WriteLine("Artifacts: " + ArtifactsDirectory.Name);

                DotNetPack(s => s
                .SetProject(Solution.GetProject("FlintSoft.WorkTime"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetIncludeSymbols(true)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                .SetOutputDirectory(ArtifactsDirectory)
            );

        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            var nugetUrl = Environment.GetEnvironmentVariable("NUGET_API_URL");
            if (string.IsNullOrEmpty(nugetUrl))
            {
                nugetUrl = "https://api.nuget.org/v3/index.json";
            }

            var nugetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
            if (string.IsNullOrEmpty(nugetApiKey))
            {
                throw new Exception("Could not get Nuget Api Key environment variable");
            }

            ArtifactsDirectory
            .GlobFiles("*.nupkg")
            .Where(x => !x.Name.EndsWith("symbols.nupkg"))
            .ForEach(x =>
            {
                DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .SetSource(nugetUrl)
                       .SetApiKey(nugetApiKey)
                   );
            });
        });
}
