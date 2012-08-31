using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Google.Authorization;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace ArtemisWest.CallMe.Google
{
    public class GoogleProvider : IProvider, INotifyPropertyChanged
    {
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
            _regionManager.Regions["WindowRegion"].Add(_loginView);
            _authorizationModel.RegisterAuthorizationCallback(ShowGoogleLogin);
            _authorizeCommand = new DelegateCommand(RequestAuthorization, () => !Status.IsAuthorized && !Status.IsProcessing);
            _authorizationModel.Status.Subscribe(_ =>
            {
                OnPropertyChanged("Status");
                _authorizeCommand.RaiseCanExecuteChanged();
            });
        }

        public string Name
        {
            get { return GoogleProviderDescription.Instance.Name; }
        }
        public Uri Image
        {
            get { return GoogleProviderDescription.Instance.Image; }
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
            return Observable.Create<string>(
                o =>
                {
                    _loginView.ViewModel.AuthorizationUri = authorizationUri;
                    _regionManager.Regions["WindowRegion"].Activate(_loginView);

                    var isActiveChanged = Observable.FromEventPattern<EventHandler, EventArgs>(
                            h => _loginView.IsActiveChanged += h,
                            h => _loginView.IsActiveChanged -= h);

                    var vmPropertyChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            h => _loginView.ViewModel.PropertyChanged += h,
                            h => _loginView.ViewModel.PropertyChanged -= h);

                    var authorizationCodeChanged = vmPropertyChanged.Where(e => e.EventArgs.PropertyName == "AuthorizationCode")
                        .Select(_ => _loginView.ViewModel.AuthorizationCode)
                        .TakeUntil(isActiveChanged.Where(_ => !_loginView.IsActive))
                        .Take(1)
                        .Do(
                            x => Console.WriteLine("ShowGoogleLogin.OnNext({0})", x),
                            ex => Console.WriteLine("ShowGoogleLogin.OnError({0})", ex),
                            () => Console.WriteLine("ShowGoogleLogin.OnCompleted()")
                            );
                    return Observable.Using(
                            () => Disposable.Create(() => _regionManager.Regions["WindowRegion"].Deactivate(_loginView)),
                            _ => authorizationCodeChanged)
                        .Subscribe(o);
                });
        }

        private void RequestAuthorization()
        {
            _authorizationModel.RequestAccessToken().Subscribe();
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}