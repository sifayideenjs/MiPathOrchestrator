using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPathOrchestrator.Models
{
    public enum QUEUE_ITEM_TYPE
    {
        DEFAULT = 0,
        BROWSE_FILE,
        BROWSE_FOLDER,
        TEXT_EDITOR,
        COMBOBOX
    };

    public class QueueItemControl
    {
        public QueueItemControl() { this.Type = QUEUE_ITEM_TYPE.DEFAULT; }

        public string Key { get; set; }

        public string Value { get; set; }

        public QUEUE_ITEM_TYPE Type { get; set; }

        public string LabelText { get; set; }

        public string Filter { get; set; }

        public string[] Options { get; set; }
    }
}