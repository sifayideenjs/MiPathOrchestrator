using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiPathOrchestrator.Common;
using MiPathOrchestrator.Models;
using MiPathOrchestrator.Utilities;

namespace MiPathOrchestrator.ViewModels
{
    public class TriggerJobViewModel : BaseViewModel
    {
        private ProcessViewModel _process = null;
        private ObservableCollection<RobotViewModel> _robots = new ObservableCollection<RobotViewModel>();
        private QueueItemFormViewModel _queueItemForm = null;

        public void Show(ProcessViewModel process)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));

            this.Process = process;
            this.Initialize(process);
        }

        public ProcessViewModel Process
        {
            get { return _process; }
            set { this.MutateVerbose(ref _process, value, RaisePropertyChanged()); }
        }

        public ObservableCollection<RobotViewModel> Robots
        {
            get { return _robots; }
            set { this.MutateVerbose(ref _robots, value, RaisePropertyChanged()); }
        }

        public QueueItemFormViewModel QueueItemForm
        {
            get { return _queueItemForm; }
            set { this.MutateVerbose(ref _queueItemForm, value, RaisePropertyChanged()); }
        }

        private void Initialize(ProcessViewModel process)
        {
            var robots = ApiHelper.Fetch_all_Robots_by_Environment(process.ProcessEnvironment);

            _robots.Clear();

            foreach (var robot in robots)
            {
                var _robot = new RobotViewModel(robot);
                this.Robots.Add(_robot);
            }

            this.QueueItemForm = new QueueItemFormViewModel(process.RobotInfo.QueueItemJSON);
        }
    }
}
