using MiPathOrchestrator.Models;
using MiPathOrchestrator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiPathOrchestrator.Common
{
    public class QueueItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DEFAULT { get; set; }

        public DataTemplate BROWSE_FILE { get; set; }

        public DataTemplate BROWSE_FOLDER { get; set; }

        public DataTemplate TEXT_EDITOR { get; set; }

        public DataTemplate COMBOBOX { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement frameworkElement = container as FrameworkElement;
            if (frameworkElement != null && item != null && item is QueueItemControlViewModel)
            {
                QueueItemControlViewModel queueItemControl = item as QueueItemControlViewModel;
                switch(queueItemControl.Type)
                {
                    case QUEUE_ITEM_TYPE.BROWSE_FILE:
                        {
                            BROWSE_FILE = frameworkElement.FindResource("BROWSE_FILE") as DataTemplate;
                            return BROWSE_FILE;
                        }
                    case QUEUE_ITEM_TYPE.BROWSE_FOLDER:
                        {
                            BROWSE_FOLDER = frameworkElement.FindResource("BROWSE_FOLDER") as DataTemplate;
                            return BROWSE_FOLDER;
                        }
                    case QUEUE_ITEM_TYPE.TEXT_EDITOR:
                        {
                            TEXT_EDITOR = frameworkElement.FindResource("TEXT_EDITOR") as DataTemplate;
                            return TEXT_EDITOR;
                        }
                    case QUEUE_ITEM_TYPE.COMBOBOX:
                        {
                            COMBOBOX = frameworkElement.FindResource("COMBOBOX") as DataTemplate;
                            return COMBOBOX;
                        }
                    default:
                        {
                            DEFAULT = frameworkElement.FindResource("DEFAULT") as DataTemplate;
                            return DEFAULT;
                        }
                }
            }
            else return null;
        }
    }
}
