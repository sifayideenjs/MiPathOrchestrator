using MiPathOrchestrator.Common;
using MiPathOrchestrator.Models;
using MiPathOrchestrator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiPathOrchestrator.ViewModels
{
    public class ProcessSetViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ProcessViewModel> _processes = new ObservableCollection<ProcessViewModel>();
        private bool _isBusy;
        private ICommand _showAllCommand;

        public ProcessSetViewModel()
        {
            Processes = new ReadOnlyObservableCollection<ProcessViewModel>(_processes);
            Task.Factory.StartNew(() => Initialize());
        }

        public ReadOnlyObservableCollection<ProcessViewModel> Processes { get; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.MutateVerbose(ref _isBusy, value, RaisePropertyChanged()); }
        }

        public ICommand ShowAllCommand
        {
            get
            {
                return _showAllCommand ?? (_showAllCommand = new RelayCommand(
                   x =>
                   {
                       Task.Factory.StartNew(() => ShowAllRobots());
                   }));
            }
        }

        private void Initialize()
        {
            IsBusy = true;

            var releases = ApiHelper.Fetch_all_Processes();

            var robotInfos = MiUtility.GetRobotInfos();
            foreach (var robotInfo in robotInfos)
            {
                var release = releases.SingleOrDefault(r => r.Id == int.Parse(robotInfo.ProcessID));
                if (release != null)
                {
                    var process = new ProcessViewModel(robotInfo, release);
                    AddProcess(process);
                    Thread.Sleep(100);
                }
            }

            IsBusy = false;
        }

        private void ShowAllRobots()
        {
            IsBusy = true;

            ClearProcess();
            var releases = ApiHelper.Fetch_all_Processes();
            RobotInfo processInfo = new RobotInfo() { ProcessName = null, QueueItemJSON = "Some JSON String" };
            foreach (var release in releases)
            {
                var process = new ProcessViewModel(processInfo, release);
                AddProcess(process);
                Thread.Sleep(100);
            }

            IsBusy = false;
        }

        private void AddProcess(ProcessViewModel process)
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _processes.Add(process);
            }));
        }

        private void ClearProcess()
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _processes.Clear();
            }));
        }

        private void RunOnUIDespatcher(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
