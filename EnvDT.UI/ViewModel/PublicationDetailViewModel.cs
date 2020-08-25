using EnvDT.Model;
using EnvDT.UI.Data;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    class PublicationDetailViewModel : ViewModelBase, IPublicationDetailViewModel
    {
        private IPublicationDataService _dataService;
        private IEventAggregator _eventAggregator;

        public PublicationDetailViewModel(IPublicationDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPublicationDetailViewEvent>()
                .Subscribe(OnOpenPublicationDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private void OnSaveExecute()
        {
            _dataService.Save(Publication);
            _eventAggregator.GetEvent<AfterPublicationSavedEvent>().Publish(
                new AfterPublicationSavedEventArgs
                {
                    PublicationId = Publication.PublicationId,
                    DisplayMember = $"{Publication.Abbreviation} {Publication.Year}"
                });
        }

        private bool OnSaveCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnOpenPublicationDetailView(Guid publicationId)
        {
            Load(publicationId);
        }

        public void Load(Guid publicationId)
        {
            Publication = _dataService.GetById(publicationId);
        }

        private Publication _publication;
        public Publication Publication
        {
            get { return _publication; }
            private set
            {
                _publication = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
    }
}
