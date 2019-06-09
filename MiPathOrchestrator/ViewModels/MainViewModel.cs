using MiPathOrchestrator.Common;
using MiPathOrchestrator.Interfaces;
using MiPathOrchestrator.Models;
using MiPathOrchestrator.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiPathOrchestrator.ViewModels
{
    public class MainViewModel : BaseViewModel, ISlideNavigationSubject
    {
        private readonly SlideNavigator _slideNavigator;
        private int _activeSlideIndex;

        public MainViewModel()
        {
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Common.NavigationCommands.ShowProcessCommand, ShowProcessExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Common.NavigationCommands.GoBackCommand, GoBackExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Common.NavigationCommands.TriggerJobCommand, TriggerJobExecuted, CanTriggerJob));
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Common.NavigationCommands.JobHistoryCommand, JobHistoryExecuted));

            Slides = new object[] { ProcessSetViewModel, TriggerJobViewModel, JobStatusViewModel, JobHistoryViewModel };

            _slideNavigator = new SlideNavigator(this, Slides);
            _slideNavigator.GoTo(0);
        }

        public object[] Slides { get; }

        public ProcessSetViewModel ProcessSetViewModel { get; } = new ProcessSetViewModel();

        public TriggerJobViewModel TriggerJobViewModel { get; } = new TriggerJobViewModel();

        public JobStatusViewModel JobStatusViewModel { get; } = new JobStatusViewModel();

        public JobHistoryViewModel JobHistoryViewModel { get; } = new JobHistoryViewModel();

        public int ActiveSlideIndex
        {
            get { return _activeSlideIndex; }
            set { this.MutateVerbose(ref _activeSlideIndex, value, RaisePropertyChanged()); }
        }

        private int IndexOfSlide<TSlide>()
        {
            int index = Slides.Select((o, i) => new { o, i }).First(a => a.o.GetType() == typeof(TSlide)).i;
            return index;
        }

        private void ShowProcessExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoTo(
                    IndexOfSlide<TriggerJobViewModel>(),
                    () => TriggerJobViewModel.Show((ProcessViewModel)e.Parameter));
        }

        private void TriggerJobExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoTo(
                       IndexOfSlide<JobStatusViewModel>(),
                       () => JobStatusViewModel.Trigger((TriggerJobViewModel)e.Parameter));
        }

        private void CanTriggerJob(object sender, CanExecuteRoutedEventArgs e)
        {
            var triggerJob = (TriggerJobViewModel)e.Parameter;
            if (triggerJob != null && triggerJob.QueueItemForm != null)
            {
                bool isNoParam = triggerJob.QueueItemForm.QueueItemControls.Any(q => q.Type == QUEUE_ITEM_TYPE.DEFAULT);
                if(isNoParam)
                {
                    e.CanExecute = true;
                }
                else
                {
                    if (triggerJob.QueueItemForm.QueueItemControls.Any(q => string.IsNullOrEmpty(q.Value) == true))
                    {
                        e.CanExecute = false;
                    }
                    else
                    {
                        e.CanExecute = true;
                    }
                }
            }
            else e.CanExecute = true;
        }

        private void JobHistoryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoTo(
                       IndexOfSlide<JobHistoryViewModel>(),
                       () => JobHistoryViewModel.ShowHistory((TriggerJobViewModel)e.Parameter));
        }

        private void GoBackExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoBack();
        }

        private void GoForwardExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _slideNavigator.GoForward();
        }
    }
}
