using System;
using System.ComponentModel;

namespace EnvDT.UITests.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static bool IsPropertyChangedFired(
            this INotifyPropertyChanged notifyPropertyChanged,
            Action action, string propertyName)
        {
            var fired = false;
            notifyPropertyChanged.PropertyChanged += (p, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    fired = true;
                }
            };

            action();

            return fired;
        }
    }
}
