using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EnvDT.UI.Service
{
    // Taken without modification from https://stackoverflow.com/a/37805849
    public class IndexToDescriptionConverter : IMultiValueConverter
    {       
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)values[0];
            if (index < 0)
            {
                return null;
            }
            ObservableCollection<string> PublColDescriptions = (ObservableCollection<string>)values[1];
            return PublColDescriptions.ElementAt(index - 1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
