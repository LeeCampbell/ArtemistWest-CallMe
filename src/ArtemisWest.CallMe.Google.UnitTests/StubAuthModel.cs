using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Google.Authorization;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    public class StubAuthModel : IAuthorizationModel
    {
        private IObservable<AuthorizationStatus> _status;
        private IResourceScope[] _availableServices;
        private ObservableCollection<IResourceScope> _selectedServices;

        public IObservable<AuthorizationStatus> Status
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

        public void RegisterAuthorizationCallback(RequestAuthorizationCode callback)
        {
            throw new NotImplementedException();
        }

        public IObservable<string> RequestAccessToken()
        {
            return Observable.Return("SomeToken");
        }
    }
}