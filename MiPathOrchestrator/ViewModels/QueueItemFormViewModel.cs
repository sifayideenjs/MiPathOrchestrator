using MiPathOrchestrator.Common;
using MiPathOrchestrator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPathOrchestrator.ViewModels
{
    public class QueueItemFormViewModel : BaseViewModel
    {
        private ObservableCollection<QueueItemControlViewModel> _queueItemControls = new ObservableCollection<QueueItemControlViewModel>();

        public QueueItemFormViewModel(string queueItemsJSON)
        {
            if (string.IsNullOrEmpty(queueItemsJSON)) throw new ArgumentNullException(nameof(queueItemsJSON));

            var queueItems = ParseQueueItems(queueItemsJSON);
            this.QueueItemControls = new ObservableCollection<QueueItemControlViewModel>(queueItems);
        }

        public ObservableCollection<QueueItemControlViewModel> QueueItemControls
        {
            get { return _queueItemControls; }
            set { this.MutateVerbose(ref _queueItemControls, value, RaisePropertyChanged()); }
        }

        private IEnumerable<QueueItemControlViewModel> ParseQueueItems(string queueItemsJSON)
        {
            try
            {
                var queueItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QueueItemControl>>(queueItemsJSON);
                var queueItemsViewModels = queueItems.Select(q => new QueueItemControlViewModel(q));
                return queueItemsViewModels;
            }
            catch(Exception ex)
            {
                //var queuItem_01 = new QueueItemControl() { Key = "inputFilePath", Value = "Apple.xlsx", LabelText = "Input File Path", Type = Models.QUEUE_ITEM_TYPE.BROWSE_FILE, Filter = "Excel files (*.xlsx)|*.xlsx", Options = null };
                //var queuItem_02 = new QueueItemControl() { Key = "inputFilePath", Value = "Fruites\\Apple", LabelText = "Input Folder Path", Type = Models.QUEUE_ITEM_TYPE.BROWSE_FOLDER, Filter = string.Empty, Options = null };
                //var queuItem_03 = new QueueItemControl() { Key = "inputText", Value = "Banana", LabelText = "Input Value", Type = Models.QUEUE_ITEM_TYPE.TEXT_EDITOR, Filter = string.Empty, Options = null };
                //var queuItem_04 = new QueueItemControl() { Key = "inputOption", Value = "Orange", LabelText = "Input Option", Type = Models.QUEUE_ITEM_TYPE.COMBOBOX, Filter = string.Empty, Options = new string[] { "Apple", "Orange", "Banana" } };
                //var queueItems = new List<QueueItemControl>() { queuItem_01, queuItem_02, queuItem_03, queuItem_04 };

                var queuItem = new QueueItemControl() { Key = "defaultText", Value = "", LabelText = ex.Message, Type = Models.QUEUE_ITEM_TYPE.DEFAULT, Filter = string.Empty, Options = null };
                var queueItems = new List<QueueItemControl>() { queuItem };

                //var serializedQueueItems = Newtonsoft.Json.JsonConvert.SerializeObject(queueItems);

                var queueItemsViewModels = queueItems.Select(q => new QueueItemControlViewModel(q));
                return queueItemsViewModels;
            }
        }
    }
}
