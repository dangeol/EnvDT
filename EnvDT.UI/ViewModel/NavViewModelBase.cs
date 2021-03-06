﻿using EnvDT.Model.IDataService;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public abstract class NavViewModelBase : ViewModelBase, INavViewModel
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;
        protected readonly ILookupDataService LookupDataService;
        private bool _isDetailViewEnabled = false;
        private IDetailViewModel _detailViewModel;
        private NavItemViewModel _selectedItem;

        public NavViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            ILookupDataService lookupDataService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;
            LookupDataService = lookupDataService;
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }

        public ICommand CreateNewDetailCommand { get; private set; }

        public ObservableCollection<NavItemViewModel> Models { get; private set; }

        public bool IsDetailViewEnabled
        {
            get { return _isDetailViewEnabled; }
            set
            {
                _isDetailViewEnabled = value;
                OnPropertyChanged();
            }
        }

        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            set
            {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }

        public NavItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    EventAggregator.GetEvent<OpenDetailViewEvent>()
                        .Publish(
                            new OpenDetailViewEventArgs
                            {
                                Id = _selectedItem.LookupItemId,
                                ViewModelName = _selectedItem.DetailViewModelName
                            });
                }
            }
        }

        public void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnItemSelected(
                new OpenDetailViewEventArgs
                {
                    ViewModelName = viewModelType.Name
                });
        }

        public abstract void LoadModels();

        protected void SetPropertyValueToNull(object instance, string property)
        {
            PropertyInfo prop = instance.GetType().GetProperty(property);
            prop.SetValue(instance, null, null);
        }

        protected abstract void OnItemSelected(OpenDetailViewEventArgs args);
    }
}
