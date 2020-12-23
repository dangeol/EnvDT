using System;

namespace EnvDT.UI.ViewModel
{
    public interface IDispatcher
    {
        public void Invoke(Action action);
        public void BeginInvoke(Action action);
    }
}
