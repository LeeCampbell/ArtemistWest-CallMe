using System;
using System.Collections.ObjectModel;
using ArtemisWest.CallMe.Contract;

namespace ArtemisWest.CallMe.Google.Authorization
{
    public delegate IObservable<string> RequestAuthorizationCode(Uri authorizationUri);

    public interface IAuthorizationModel
    {
        IObservable<AuthorizationStatus> Status { get; }
        IResourceScope[] AvailableServices { get; }
        ObservableCollection<IResourceScope> SelectedServices { get; }

        void RegisterAuthorizationCallback(RequestAuthorizationCode callback);
        IObservable<string> RequestAccessToken();
    }
}