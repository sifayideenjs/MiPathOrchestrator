using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath;

namespace MiPathOrchestrator.Models
{
    public class Process
    {
        public Process(ReleaseDto release)
        {
            if (release == null) throw new ArgumentNullException(nameof(release));

            this.ProcessID = release.Id;
            this.ProcessName = release.Name;
            this.ProcessKey = release.ProcessKey;
            this.ProcessDescription = release.Description;
            this.ProcessEnvironment = release.EnvironmentName;
        }

        public int ProcessID { get; }

        public string ProcessName { get; }

        public string ProcessKey { get; }

        public string ProcessDescription { get; }

        public string ProcessEnvironment { get; }
    }
}
