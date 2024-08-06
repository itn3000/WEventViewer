using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tooling;

namespace nukebuild
{
    interface Installer : INukeBuild
    {
        [Parameter]
        public string Configuration => TryGetValue(() => Configuration) ?? "Release";
        public Target BuildInstallerBinary => _ => _
            .Executes(() =>
            {
                var project = RootDirectory / "WEventViewer.Msi" / "WEventViewer.Msi.wixproj";
                DotNetBuild(cfg => cfg.SetProjectFile(project)
                    .SetProcessWorkingDirectory(project.Parent)
                    .SetConfiguration(Configuration));
            });
    }
}
