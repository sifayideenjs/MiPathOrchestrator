using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MiPathOrchestrator.Common
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class StateToVisibilityConverter : IValueConverter
    {
        public string State { get; set; }
        public bool Inverse { get; set; }
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public StateToVisibilityConverter()
        {
            State = "Pending,Running";
            Inverse = false;
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return null;

            string[] states = this.State.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if(this.Inverse)
            {
                return states.Contains(value.ToString()) ? FalseValue : TrueValue;
            }
            else
            {
                return states.Contains(value.ToString()) ? TrueValue : FalseValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue)) return true;
            if (Equals(value, FalseValue)) return false;
            return null;
        }
    }
}
