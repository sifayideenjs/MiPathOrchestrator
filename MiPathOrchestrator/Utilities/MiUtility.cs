using MiPathOrchestrator.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPathOrchestrator.Utilities
{
    public static class MiUtility
    {
        public static dynamic GetDynamicObject(Dictionary<string, object> properties)
        {
            return new MiDynamicObject(properties);
        }

        public static IEnumerable<RobotInfo> GetRobotInfos()
        {
            string robotInfoDBPath = GetConfigurationValue("ROBOT_DB");
            var robotInfos = ExcelUtility.ConvertToList<RobotInfo>(robotInfoDBPath);

            string userName = GetCurrentUserName();
            return robotInfos.Where(r => r.UserID.ToUpper() == userName);
        }

        public static string GetCurrentUserName()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            userName = userName.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).Last();

            return userName.ToUpper();
        }

        public static RobotInfo GetRobotInfoByID(int processID)
        {
            var robotInfos = GetRobotInfos();
            return robotInfos.SingleOrDefault(r => r.ProcessID == processID.ToString());
        }

        public static string GetConfigurationValue(string key)
        {
            string value = string.Empty;
            bool isAvailable = ConfigurationManager.AppSettings.AllKeys.Contains(key);
            if (isAvailable)
            {
                value = ConfigurationManager.AppSettings[key];
            }
            else
            {
                throw new ArgumentNullException(nameof(key));
            }

            return value;
        }
    }
}
