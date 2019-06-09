using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath;

namespace MiPathOrchestrator.Utilities
{
    internal class ApiHelper
    {
        private static string Get_Token()
        {
            var loginModel = new
            {
                tenancyName = string.Empty,
                usernameOrEmailAddress = "username",
                password = "password"
            };

            string token = OrchestratorAPIHelper.GetToken(loginModel);
            return token;
        }

        public static ReleaseDto[] Fetch_all_Processes()
        {
            string token = Get_Token();
            var releases = OrchestratorAPIHelper.GetListOfReleases(token);
            return releases;
        }

        public static RobotDto[] Fetch_all_Robots()
        {
            string token = Get_Token();
            var robots = OrchestratorAPIHelper.GetListOfRobots(token);
            return robots;
        }

        public static RobotDto[] Fetch_all_Robots_by_Environment(string environmentName)
        {
            string token = Get_Token();
            var robots = OrchestratorAPIHelper.GetListOfRobots(token);
            var matchRobots = OrchestratorAPIHelper.GetRobots(robots, environmentName);
            return matchRobots;
        }

        public static string Get_Robot_Status(string robotName)
        {
            string token = Get_Token();
            var sessions = OrchestratorAPIHelper.GetListOfSessions(token);
            var session = OrchestratorAPIHelper.GetSession_byRobotName(sessions, robotName);
            if (session != null) return session.State;
            else return "Unavailable";
        }

        public static void Add_to_Queue(object specificContent, string queueName)
        {
            string token = Get_Token();
            OrchestratorAPIHelper.AddQueue(token, queueName, specificContent);
        }

        public static JobDto[] Run_a_Job(ReleaseDto release)
        {
            JobDto[] jobs = null;
            try
            {
                string token = Get_Token();
                string processName = release.ProcessKey;
                string environmentName = release.EnvironmentName;

                var robots = OrchestratorAPIHelper.GetListOfRobots(token);
                var matchRobots = OrchestratorAPIHelper.GetRobots(robots, environmentName);

                List<int> robotIds = new List<int>();
                foreach (var robot in matchRobots)
                {
                    robotIds.Add(robot.Id);
                }

                jobs = OrchestratorAPIHelper.ExecuteRobot(token, release.Key, robotIds.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }

            return jobs;
        }

        public static void Stop_a_Job(int jobId)
        {
            string token = Get_Token();

            // 1 (meaning Soft Stop) or 2 (meaning Kill).

            OrchestratorAPIHelper.StopRobot(token, jobId, "2");
        }

        public static JobDto Get_Job_Details(int jobId)
        {
            string token = Get_Token();
            var job = OrchestratorAPIHelper.GetJobDetails(token, jobId);
            return job;
        }

        public static JobDto[] Get_Job_History(string processKey)
        {
            string token = Get_Token();
            var jobs = OrchestratorAPIHelper.GetJobHistory(token, processKey);
            return jobs;
        }

        public static List<string> Get_Logs(string jobKey)
        {
            List<string> logStrings = new List<string>();
            string token = Get_Token();
            var logs = OrchestratorAPIHelper.GetLogs(token, jobKey);
            foreach (var log in logs)
            {
                logStrings.Add(string.Format("{0} : {1}", DateTime.Parse(log.TimeStamp).ToString(), log.Message));
            }

            return logStrings;
        }
    }
}
