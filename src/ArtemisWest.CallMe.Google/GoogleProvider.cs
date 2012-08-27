using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Google.Authorization;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace ArtemisWest.CallMe.Google
{
    public class GoogleProvider : IProvider
    {
        private static readonly Uri _image = new Uri("pack://application:,,,/ArtemisWest.CallMe.Google;component/Google_64x64.png");
        private readonly IAuthorizationModel _authorizationModel;
        private readonly IRegionManager _regionManager;
        private readonly IGoogleLoginView _loginView;
        private readonly DelegateCommand _authorizeCommand;

        //TODO: Inject a Func<IGoogleLoginView> instead
        public GoogleProvider(IAuthorizationModel authorizationModel, IRegionManager regionManager, IGoogleLoginView loginView)
        {
            Console.WriteLine("GoogleProvider()");
            _authorizationModel = authorizationModel;
            _regionManager = regionManager;
            _loginView = loginView;
            _authorizationModel.RegisterAuthorizationCallback(ShowGoogleLogin);
            _authorizeCommand = new DelegateCommand(RequestAuthorization, () => !Status.IsAuthorized && !Status.IsProcessing);
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
            get { return _authorizationModel.Status.First(); }
        }
        public IResourceScope[] AvailableServices
        {
            get { return _authorizationModel.AvailableServices; }
        }
        public ObservableCollection<IResourceScope> SelectedServices
        {
            get { return _authorizationModel.SelectedServices; }
        }
        public ICommand AuthorizeCommand
        {
            get { return _authorizeCommand; }
        }

        private IObservable<string> ShowGoogleLogin(Uri authorizationUri)
        {
            //throw new NotImplementedException();
            //TODO: Ensure I am on the dispatcher.
            //TODO: Block until the Window closes
            //TODO: Disposal should close the window too

            return Observable.Create<string>(
                o =>
                {
                    _loginView.ViewModel.AuthorizationUri = authorizationUri;
                    //_regionManager.AddToRegion("WindowRegion", _loginView);
                    _regionManager.Regions["WindowRegion"].Add(_loginView);
                    _regionManager.Regions["WindowRegion"].Activate(_loginView);
                    return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            h => _loginView.ViewModel.PropertyChanged += h,
                            h => _loginView.ViewModel.PropertyChanged -= h)
                        .Where(e => e.EventArgs.PropertyName == "AuthorizationCode")
                        .Select(_ => _loginView.ViewModel.AuthorizationCode)
                        .Subscribe(o);
                });
        }

        private void RequestAuthorization()
        {
            _authorizationModel.RequestAccessToken().Subscribe();
        }
    }
}