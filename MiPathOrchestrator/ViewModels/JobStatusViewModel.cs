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
    public class JobStatusViewModel : BaseViewModel
    {
        private TriggerJobViewModel _triggerJob;
        private string _batchID;
        private DateTime _triggeredTime;
        private ObservableCollection<JobViewModel> _jobs = new ObservableCollection<JobViewModel>();

        public void Trigger(TriggerJobViewModel triggerJob)
        {
            if (triggerJob == null) throw new ArgumentNullException(nameof(triggerJob));

            this.TriggerJob = triggerJob;

            _triggeredTime = DateTime.Now;
            this.BatchID = _triggeredTime.ToString("yyyy-MM-dd-HH-mm-ss");

            Task.Factory.StartNew(() => this.Trigger_a_Job());
        }

        public TriggerJobViewModel TriggerJob
        {
            get { return _triggerJob; }
            set { this.MutateVerbose(ref _triggerJob, value, RaisePropertyChanged()); }
        }

        public ObservableCollection<JobViewModel> Jobs
        {
            get { return _jobs; }
            set { this.MutateVerbose(ref _jobs, value, RaisePropertyChanged()); }
        }

        public string BatchID
        {
            get { return _batchID; }
            set { this.MutateVerbose(ref _batchID, value, RaisePropertyChanged()); }
        }

        private void Trigger_a_Job()
        {
            if (_triggerJob != null)
            {
                var queueName = _triggerJob.Process.RobotInfo.QueueName;
                if(!string.IsNullOrEmpty(queueName))
                {
                    //Add Paramerter to Queue Item
                    var specificObject = GetSpecificObject(_triggerJob);
                    ApiHelper.Add_to_Queue(specificObject, queueName);
                }

                //Thread.Sleep(2000);

                //Trigger a Job
                var jobs = ApiHelper.Run_a_Job(this.TriggerJob.Process.Release);

                ClearJobs();

                //Get the result Jobs
                foreach (var job in jobs)
                {
                    var _job = new JobViewModel(job, true);
                    AddJob(_job);
                }
            }
        }

        private object GetSpecificObject(TriggerJobViewModel triggerJob)
        {
            string userName = MiUtility.GetCurrentUserName();
            Dictionary<string, string>  rDictionary = GetRemotePaths(triggerJob, userName);

            Dictionary<string, string> qDictionary = triggerJob.QueueItemForm.QueueItemControls.Where(q => q.Type != Models.QUEUE_ITEM_TYPE.DEFAULT).ToDictionary(q => q.Key, q => q.Value);
            qDictionary.Add("BatchID", this.BatchID);
            qDictionary.Add("TriggeredBy", userName.ToUpper());
            qDictionary.Add("TriggeredAt", _triggeredTime.ToString());

            Dictionary<string, object> queueDictionay = new Dictionary<string, object>();
            foreach (var key in qDictionary.Keys)
            {
                if(rDictionary.ContainsKey(key))
                {
                    queueDictionay.Add(key, rDictionary[key]);
                }
                else
                {
                    queueDictionay.Add(key, qDictionary[key]);
                }
            }

            var specificObject = MiUtility.GetDynamicObject(queueDictionay);
            return specificObject;
        }

        private Dictionary<string, string> GetRemotePaths(TriggerJobViewModel triggerJob, string userName)
        {
            Dictionary<string, string> remotePaths = new Dictionary<string, string>();

            string sftpPath = MiUtility.GetConfigurationValue("SFTP_PATH");
            var file_or_folder_Controls = triggerJob.QueueItemForm.QueueItemControls.Where(q => (q.Type == Models.QUEUE_ITEM_TYPE.BROWSE_FILE || q.Type == Models.QUEUE_ITEM_TYPE.BROWSE_FOLDER));
            foreach (var queueControl in file_or_folder_Controls)
            {
                string remoteDirectory = Path.Combine(sftpPath, string.Format("{0}\\{1}", userName.ToUpper(), this.BatchID));
                if (!Directory.Exists(remoteDirectory))
                {
                    Directory.CreateDirectory(remoteDirectory);
                }

                if (queueControl.Type == Models.QUEUE_ITEM_TYPE.BROWSE_FILE)
                {
                    string localFilePath = queueControl.Value;
                    string fileName = Path.GetFileName(localFilePath);
                    string remoteFilePath = Path.Combine(remoteDirectory, fileName);

                    File.Copy(localFilePath, remoteFilePath, true);

                    remotePaths.Add(queueControl.Key, remoteFilePath);
                }
                else if (queueControl.Type == Models.QUEUE_ITEM_TYPE.BROWSE_FOLDER)
                {
                    string localFolderPath = queueControl.Value;
                    string folderName = Path.GetDirectoryName(localFolderPath);
                    string remoteFolderPath = Path.Combine(remoteDirectory, folderName);

                    foreach (string dirPath in Directory.GetDirectories(localFolderPath, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(localFolderPath, remoteFolderPath));

                    foreach (string newPath in Directory.GetFiles(localFolderPath, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(localFolderPath, remoteFolderPath), true);

                    remotePaths.Add(queueControl.Key, remoteFolderPath);
                }
            }

            return remotePaths;
        }

        private void AddJob(JobViewModel job)
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _jobs.Add(job);
            }));
        }

        private void ClearJobs()
        {
            RunOnUIDespatcher((Action)(() =>
            {
                _jobs.Clear();
            }));
        }

        private void RunOnUIDespatcher(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
