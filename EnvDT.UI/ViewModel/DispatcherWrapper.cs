using System;
using System.Windows;
using System.Windows.Threading;

namespace EnvDT.UI.ViewModel
{
    public class DispatcherWrapper : IDispatcher
    {
        private Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            _dispatcher = Application.Current.Dispatcher;
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}
