using Nuke.Common;
using Nuke.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.CompressionTasks;

interface NativeBuild : INukeBuild
{
    [Parameter]
    string Configuration => TryGetValue(() => Configuration) ?? "Release";
    [Parameter]
    string Runtime => TryGetValue(() => Runtime) ?? "win-x64";
    Target NativePublish => _ => _
        .Executes(() =>
        {
            var outdir = GetNativeOutputDirectory();
            var project = RootDirectory / "src" / "WEventViewer" / "WEventViewer.csproj";
            DotNetPublish(setting => setting.SetConfiguration(Configuration)
                .SetRuntime(Runtime)
                .SetPublishTrimmed(true)
                .SetProperty("PublishAot", "true")
                .SetSelfContained(true)
                .SetOutput(outdir)
                .SetProject(project)
                );
        });
    Target NativeArchive => _ => _
        .DependsOn(NativePublish)
        .Executes(() =>
        {
            var outdir = GetNativeOutputDirectory();
            outdir.ZipTo(outdir.Parent / $"WEventViewer-{Runtime}.zip", fileMode: System.IO.FileMode.Create);
        });
    AbsolutePath GetNativeOutputDirectory()
    {
        return RootDirectory / "dist" / "native" / Configuration / Runtime / "WEventViewer";
    }
}