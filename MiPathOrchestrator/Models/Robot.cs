using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath;

namespace MiPathOrchestrator.Models
{
    public class Robot
    {
        public Robot(RobotDto robot)
        {
            if (robot == null) throw new ArgumentNullException(nameof(robot));

            this.LicenseKey = robot.LicenseKey;
            this.MachineName = robot.MachineName;
            this.Name = robot.Name;
            this.Username = robot.Username;
            this.Description = robot.Description;
            this.Type = robot.Type;
            this.Password = robot.Password;
            this.RobotEnvironments = robot.RobotEnvironments;
            this.Id = robot.Id;
        }

        public string LicenseKey { get; set; }
        public string MachineName { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Password { get; set; }
        public string RobotEnvironments { get; set; }
        public int Id { get; set; }
    }
}
