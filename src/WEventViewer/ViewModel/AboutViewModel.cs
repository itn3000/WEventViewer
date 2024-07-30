using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEventViewer.ViewModel
{
    internal class AboutViewModel
    {
        public AboutViewModel()
        {
            var asmname = typeof(AboutViewModel).Assembly.GetName();
            Name = "Windows EventLog Viewer";
            Version = asmname.Version != null ? asmname.Version.ToString() : string.Empty;
        }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
