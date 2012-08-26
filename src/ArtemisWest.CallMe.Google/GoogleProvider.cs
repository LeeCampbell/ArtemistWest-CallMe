using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ArtemisWest.CallMe.Contract;
using Microsoft.Practices.Prism.Commands;

namespace ArtemisWest.CallMe.Google
{
    public class GoogleProvider : IProvider
    {
        private static readonly Uri _image = new Uri("pack://application:,,,/ArtemisWest.CallMe.Google;component/Google_64x64.png");
        private static readonly IResourceScope[] _availableServices = new IResourceScope[] { };
        private readonly ObservableCollection<IResourceScope> _selectedServices = new ObservableCollection<IResourceScope>();
        private AuthorizationStatus _status = AuthorizationStatus.NotAuthorized;

        //private DelegateCommand _authorizeCommand = new DelegateCommand();
        private DelegateCommand _authorizeCommand = null;

        public GoogleProvider()
        {
            Console.WriteLine("GoogleProvider()");
        }

        public string Name
        {
            get { return "Google"; }
        }

        public Uri Image
        {
            get { return _image; }
        }

        public AuthorizationStatus Status
        {
            get { return _status; }
        }

        public IResourceScope[] AvailableServices
        {
            get { return _availableServices; }
        }

        public ObservableCollection<IResourceScope> SelectedServices
        {
            get { return _selectedServices; }
        }

        public ICommand AuthorizeCommand
        {
            get { return _authorizeCommand; }
        }
    }
}