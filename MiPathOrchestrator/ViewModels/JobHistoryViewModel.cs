using MiPathOrchestrator.Common;
using MiPathOrchestrator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiPathOrchestrator.ViewModels
{
    public class JobHistoryViewModel : BaseViewModel
    {
        private TriggerJobViewModel _triggerJob;
        private ObservableCollection<JobViewModel> _jobHistory = new ObservableCollection<JobViewModel>();

        public void ShowHistory(TriggerJobViewModel triggerJob)
        {
            if (triggerJob == null) throw new ArgumentNullException(nameof(triggerJob));

            this.TriggerJob = triggerJob;

            Task.Factory.StartNew(() => this.Get_Job_History());
        }

        public TriggerJobViewModel TriggerJob
        {
            get { return _triggerJob; }
            set { this.MutateVerbose(ref _triggerJob, value, RaisePropertyChanged()); }
        }

        public ObservableCollection<JobViewModel> JobHistory
        {
            get { return _jobHistory; }
            set { this.MutateVerbose(ref _jobHistory, value, RaisePropertyChanged()); }
        }

        private void Get_Job_History()
        {
            if(_triggerJob != null)
            {
                var processKey = _triggerJob.Process.ProcessKey;
                var jobs = ApiHelper.Get_Job_History(processKey);

                ClearHistoryJobs();

                //Get the result Jobs
                foreach (var job in jobs)
                {
                    var _job = new JobViewModel(job);
                    AddHistoryJob(_job);
                }
            }
        }

        private void AddHistoryJob(JobViewModel job)
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _jobHistory.Add(job);
            }));
        }

        private void ClearHistoryJobs()
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _jobHistory.Clear();
            }));
        }

        private void RunOnUIDespatcher(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
