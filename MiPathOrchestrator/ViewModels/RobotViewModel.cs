using MiPathOrchestrator.Common;
using MiPathOrchestrator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath;

namespace MiPathOrchestrator.ViewModels
{
    public class RobotViewModel : BaseViewModel
    {
        private bool _isSelected = false;

        public RobotViewModel(RobotDto robot)
        {
            if (robot == null) throw new ArgumentNullException(nameof(robot));

            this.ID = robot.Id;
            this.Name = robot.Name;
            this.MachineName = robot.MachineName;
            this.Status = ApiHelper.Get_Robot_Status(robot.Name);
        }

        public int ID { get; }

        public string Name { get; }

        public string MachineName { get; }

        public string Status { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.MutateVerbose(ref _isSelected, value, RaisePropertyChanged()); }
        }
    }
}
