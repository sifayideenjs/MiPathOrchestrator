using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiPathOrchestrator.Common
{
    public static class NavigationCommands
    {
        public static RoutedCommand ShowProcessCommand = new RoutedCommand();
        public static RoutedCommand GoBackCommand = new RoutedCommand();
        public static RoutedCommand TriggerJobCommand = new RoutedCommand();
        public static RoutedCommand JobHistoryCommand = new RoutedCommand();
    }
}
