using MaterialDesignThemes.Wpf;
using MiPathOrchestrator.Common;
using MiPathOrchestrator.Utilities;
using MiPathOrchestrator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UiPath;

namespace MiPathOrchestrator.ViewModels
{
    public class JobViewModel : BaseViewModel
    {
        private JobDto _job = null;
        private string _machineName = string.Empty;
        private string _startTime = string.Empty;
        private string _endTime = string.Empty;
        private string _state = string.Empty;
        private string _creationTime = string.Empty;
        private string _key = string.Empty;
        private int _Id = 0;
        private bool _canViewLog = true;
        private ObservableCollection<string> _logs = new ObservableCollection<string>();
        private ICommand _refreshCommand;
        private ICommand _stopCommand;
        private ICommand _viewLogCommand;
        private bool _autoRefresh = false;

        private System.Timers.Timer _timer;

        public JobViewModel(JobDto job, bool autoRefresh = false)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            this.StartTime = job.StartTime == null ? string.Empty : DateTime.Parse(job.StartTime).ToString();
            this.EndTime = job.EndTime == null ? string.Empty : DateTime.Parse(job.EndTime).ToString();
            this.State = job.State;
            this.CreationTime = job.CreationTime == null ? string.Empty : DateTime.Parse(job.CreationTime).ToString();
            this.Key = job.Key;
            this.Id = job.Id;

            _autoRefresh = autoRefresh;
            if (autoRefresh) this.HitRefresh();
            else this.Refresh();
        }

        public JobDto Job
        {
            get { return _job; }
            set { this.MutateVerbose(ref _job, value, RaisePropertyChanged()); }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { this.MutateVerbose(ref _machineName, value, RaisePropertyChanged()); }
        }

        public string StartTime
        {
            get { return _startTime; }
            set { this.MutateVerbose(ref _startTime, value, RaisePropertyChanged()); }
        }

        public string EndTime
        {
            get { return _endTime; }
            set { this.MutateVerbose(ref _endTime, value, RaisePropertyChanged()); }
        }

        public string State
        {
            get { return _state; }
            set { this.MutateVerbose(ref _state, value, RaisePropertyChanged()); }
        }

        public string CreationTime
        {
            get { return _creationTime; }
            set { this.MutateVerbose(ref _creationTime, value, RaisePropertyChanged()); }
        }

        public string Key
        {
            get { return _key; }
            set { this.MutateVerbose(ref _key, value, RaisePropertyChanged()); }
        }

        public int Id
        {
            get { return _Id; }
            set { this.MutateVerbose(ref _Id, value, RaisePropertyChanged()); }
        }

        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set { this.MutateVerbose(ref _logs, value, RaisePropertyChanged()); }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new RelayCommand(
                   x =>
                   {
                       HitRefresh();
                   }));
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new RelayCommand(
                   x =>
                   {
                       Stop();
                   }));
            }
        }

        public ICommand ViewLogCommand
        {
            get
            {
                return _viewLogCommand ?? (_viewLogCommand = new RelayCommand(
                   x =>
                   {
                       ViewLog();
                   }
                   //, o => CanViewLog()
                   ));
            }
        }

        private void HitRefresh()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(RunRefresh);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void RunRefresh(object source, System.Timers.ElapsedEventArgs e)
        {
            Refresh();

            if(this.State == "Successful")
            {
                RunOnUIDespatcher((Action)(() =>
                {
                    _timer.AutoReset = false;
                    _timer.Enabled = false;
                    _timer.Stop();
                }));
            }
        }

        private void Refresh()
        {
            var jobDetail = ApiHelper.Get_Job_Details(this.Id);
            RunOnUIDespatcher((Action)(() =>
            {
                this.State = jobDetail.State;
                this.CreationTime = jobDetail.CreationTime;
                this.StartTime = jobDetail.StartTime;
                this.EndTime = jobDetail.EndTime;
                this.MachineName = jobDetail.Robot != null ? jobDetail.Robot.MachineName : "Unable to retrieve";
            }));
        }

        private void Stop()
        {
            ApiHelper.Stop_a_Job(this.Id);
        }

        private void ViewLog()
        {
            RunOnUIDespatcher((Action)(() =>
            {
                if(_canViewLog)
                {
                    int retry = 0;
                    List<string> logs = new List<string>();
                    do
                    {
                        if (retry == 3) break;
                        logs = ApiHelper.Get_Logs(this.Key);
                        retry++;
                    }
                    while (logs.Count == 0);

                    this.Logs.Clear();
                    this.Logs = new ObservableCollection<string>(logs);

                    //_canViewLog = false;
                }

                var view = new LogView
                {
                    DataContext = this
                };
                var result = DialogHost.Show(view, "RootDialog");
            }));
        }

        private bool CanViewLog()
        {
            if (_autoRefresh == false) return true;
            else return this.State == "Successful" ? true : false;
        }

        private void RunOnUIDespatcher(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }


        //delegate JobDto GetJobDetail(int id);

        //private JobDto Refresh(int id)
        //{
        //    var jobDetail = ApiHelper.Get_Job_Details(id);
        //    return jobDetail;
        //}
    }
}