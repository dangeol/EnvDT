using Autofac;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EnvDT.UI.Factory
{
    // Taken from the following sources:
    // https://www.youtube.com/watch?v=IM_3JuVJAQk&t=57s
    // https://github.com/tomislaveric/Xamplate/blob/develop/Xamplate/Services/ViewFactory.cs
    public class ViewFactory : IViewFactory
    {
        private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();
        private IComponentContext _componentContext;

        public ViewFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : UserControl
        {
            _map[typeof(TViewModel)] = typeof(TView);
        }

        public UserControl Resolve<TViewModel>()
            where TViewModel : class, IViewModel
        {
            TViewModel viewModel = _componentContext.Resolve<TViewModel>();
            var viewType = _map[typeof(TViewModel)];
            var view = _componentContext.Resolve(viewType) as UserControl;

            view.DataContext = viewModel;
            return view;
        }
    }
}
