using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPathOrchestrator.Models
{
    public class RobotInfo
    {
        public string ID { get; set; }
        public string ProcessID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescription { get; set; }
        public string QueueName { get; set; }
        public string QueueItemJSON { get; set; }
        public string UserID { get; set; }
    }
}
