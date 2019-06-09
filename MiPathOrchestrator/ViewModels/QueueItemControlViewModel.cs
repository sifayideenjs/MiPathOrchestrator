using Microsoft.Win32;
using MiPathOrchestrator.Common;
using MiPathOrchestrator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MiPathOrchestrator.ViewModels
{
    public class QueueItemControlViewModel : BaseViewModel
    {
        private string _value = string.Empty;
        private ICommand _browseFileCommand;
        private ICommand _browseFolderCommand;

        public QueueItemControlViewModel(QueueItemControl queueItemControl)
        {
            if (queueItemControl == null) throw new ArgumentNullException(nameof(queueItemControl));

            this.Key = queueItemControl.Key;
            this.Value = queueItemControl.Value;
            this.Type = queueItemControl.Type;
            this.LabelText = queueItemControl.LabelText;
            this.Filter = queueItemControl.Filter;
            this.Options = queueItemControl.Options;
        }

        public string Value
        {
            get { return _value; }
            set { this.MutateVerbose(ref _value, value, RaisePropertyChanged()); }
        }

        public string Key { get; set; }

        public QUEUE_ITEM_TYPE Type { get; set; }

        public string LabelText { get; set; }

        public string Filter { get; set; }

        public string[] Options { get; set; }

        public ICommand BrowseFileCommand
        {
            get
            {
                return _browseFileCommand ?? (_browseFileCommand = new RelayCommand(
                   x =>
                   {
                       ShowBrowseFileDialog();
                   }));
            }
        }

        public ICommand BrowseFolderCommand
        {
            get
            {
                return _browseFolderCommand ?? (_browseFolderCommand = new RelayCommand(
                   x =>
                   {
                       ShowBrowseFolderDialog();
                   }));
            }
        }

        private void ShowBrowseFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = string.IsNullOrEmpty(this.Filter) ? "All files (*.*)|*.*" : this.Filter;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                this.Value = openFileDialog.FileName;
            }
        }

        private void ShowBrowseFolderDialog()
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dlg.ShowNewFolderButton = true;
                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.Value = dlg.SelectedPath;
                }
            }
        }
    }
}
