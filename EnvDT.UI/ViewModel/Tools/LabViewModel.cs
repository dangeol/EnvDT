using EnvDT.Model.IDataService;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class LabViewModel : NavViewModelBase, ILabViewModel
    {
        private ILookupDataService _lookupDataService;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;

        private Func<ILabDetailViewModel> _labDetailVmCreator;

        public LabViewModel(ILookupDataService lookupDataService, IEventAggregator eventAggregator, 
            Func<ILabDetailViewModel> labDetailVmCreator, IMessageDialogService messageDialogService)
            : base(eventAggregator)
        {
            _lookupDataService = lookupDataService;
            _eventAggregator = eventAggregator;
            _labDetailVmCreator = labDetailVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            _eventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Laboratories = new ObservableCollection<NavItemViewModel>();
            LoadModels();
        }

        public ObservableCollection<NavItemViewModel> Laboratories { get; private set; }

        public ICommand CloseDetailViewCommand { get; }

        public Guid? LaboratoryId { get; set; }

        public override void LoadModels()
        {
            Laboratories.Clear();
            var navItemViewModelNull = new NavItemViewModelNull();
            Laboratories.Add(navItemViewModelNull);
            SelectedItem = navItemViewModelNull;
            foreach (var lab in _lookupDataService.GetAllLaboratoriesLookup())
            {
                Laboratories.Add(new NavItemViewModel(
                    lab.LookupItemId, lab.DisplayMember, 
                    nameof(LabDetailViewModel),
                    _eventAggregator));
            }
        }

        protected override void OnItemSelected(OpenDetailViewEventArgs args)
        {
            if (args.Id != Guid.Empty && args.ViewModelName == nameof(LabDetailViewModel))
            {
                CreateAndLoadLabDetailViewModel(args);
            }
        }

        private void CreateAndLoadLabDetailViewModel(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            { 
                var result = _messageDialogService.ShowYesNoDialog(
                    Translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_ConfirmNavigate"],
                    Translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_ConfirmNavigate"]);

                if (result == MessageDialogResult.No)
                {
                    return;
                }
            }

            DetailViewModel = _labDetailVmCreator();
            DetailViewModel.Load(args.Id);
            IsDetailViewEnabled = true;
        }

        private void OnDetailSaved(DetailSavedEventArgs args)
        {
            var displayMember = args.DisplayMember;

            switch(args.ViewModelName)
            {
                case nameof(LabDetailViewModel):
                    var labItem = Laboratories.SingleOrDefault(p => p.LookupItemId == args.Id);
                    if (labItem != null)
                    {
                        labItem.DisplayMember = displayMember;
                    }
                    else
                    {
                        labItem = new NavItemViewModel(args.Id, displayMember, 
                            nameof(LabDetailViewModel),
                            _eventAggregator);
                        Laboratories.Add(labItem);
                    }
                    break;
            }
        }

        private void OnDetailDeleted(DetailDeletedEventArgs args)
        {
            switch(args.ViewModelName)
            {
                case nameof(LabDetailViewModel):
                    var labItem = Laboratories.SingleOrDefault(p => p.LookupItemId == args.Id);
                    IsDetailViewEnabled = false;
                    if (labItem != null)
                    {
                        Laboratories.Remove(labItem);
                    }
                    SetPropertyValueToNull(this, "DetailViewModel");
                    break;
            }
        }
    }
}
