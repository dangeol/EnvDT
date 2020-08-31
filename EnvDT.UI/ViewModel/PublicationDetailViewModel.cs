using EnvDT.Model;
using EnvDT.UI.Data.Repositories;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    class PublicationDetailViewModel : ViewModelBase, IPublicationDetailViewModel
    {
        private IPublicationRepository _publicationRepository;
        private IEventAggregator _eventAggregator;
        private Publication _publication;

        public PublicationDetailViewModel(IPublicationRepository publicationRepository,
            IEventAggregator eventAggregator)
        {
            _publicationRepository = publicationRepository;
            _eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private void OnSaveExecute()
        {
            _publicationRepository.Save();
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

        public void Load(Guid publicationId)
        {
            Publication = _publicationRepository.GetById(publicationId);
        }

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
