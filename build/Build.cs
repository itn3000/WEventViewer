using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild, NativeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });
    [Parameter]
    string Runtime = "win-x64";
    Target Publish => _ => _
        .Executes(() =>
        {
            var outdir = GetPublishOutputDirectory();
            var project = RootDirectory / "src" / "WEventViewer" / "WEventViewer.csproj";
            DotNetPublish(setting => setting.SetConfiguration(Configuration)
                .SetRuntime(Runtime)
                .SetPublishTrimmed(true)
                .SetSelfContained(true)
                .SetOutput(outdir)
                .SetProject(project)
                .SetPublishSingleFile(true)
                );
        });
    Target Archive => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            var outdir = GetPublishOutputDirectory();
            outdir.ZipTo(outdir.Parent / $"WEventViewer-{Runtime}.zip", fileMode: System.IO.FileMode.Create);
        });
    AbsolutePath GetPublishOutputDirectory()
    {
        return RootDirectory / "dist" / "publish" / Configuration / Runtime / "WEventViewer";
    }
}
