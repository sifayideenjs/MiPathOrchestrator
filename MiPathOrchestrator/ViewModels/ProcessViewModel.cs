using MiPathOrchestrator.Common;
using MiPathOrchestrator.Models;
using MiPathOrchestrator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath;

namespace MiPathOrchestrator.ViewModels
{
    public class ProcessViewModel : BaseViewModel
    {
        private RobotInfo _robotInfo;
        private ReleaseDto _release;
        private ObservableCollection<RobotViewModel> _robots = new ObservableCollection<RobotViewModel>();

        public ProcessViewModel(RobotInfo robotInfo, ReleaseDto release)
        {
            if (robotInfo == null) throw new ArgumentNullException(nameof(robotInfo));
            if (release == null) throw new ArgumentNullException(nameof(release));

            Robots = new ObservableCollection<RobotViewModel>(_robots);

            this.RobotInfo = robotInfo;
            this.CustomProcessName = robotInfo.ProcessName ?? release.ProcessKey;
            this.CustomProcessDescription = robotInfo.ProcessDescription ?? release.Description;

            this.Release = release;
            this.ProcessID = release.Id;
            this.ProcessName = release.Name;
            this.ProcessKey = release.ProcessKey;
            this.ProcessDescription = release.Description;
            this.ProcessEnvironment = release.EnvironmentName;
        }

        public void Show()
        {
            Init();
        }

        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { this.MutateVerbose(ref _robotInfo, value, RaisePropertyChanged()); }
        }

        public ReleaseDto Release
        {
            get { return _release; }
            set { this.MutateVerbose(ref _release, value, RaisePropertyChanged()); }
        }

        public int ProcessID { get; }

        public string ProcessName { get; }

        public string ProcessKey { get; }

        public string ProcessDescription { get; }

        public string ProcessEnvironment { get; }

        public string CustomProcessName { get; }

        public string CustomProcessDescription { get; }

        public ObservableCollection<RobotViewModel> Robots
        {
            get { return _robots; }
            set { this.MutateVerbose(ref _robots, value, RaisePropertyChanged()); }
        }

        private void Init()
        {
            var robots = ApiHelper.Fetch_all_Robots_by_Environment(this.ProcessEnvironment);
            _robots.Clear();
            foreach (var robot in robots)
            {
                var _robot = new RobotViewModel(robot);
                Robots.Add(_robot);
            }
        }
    }
}
