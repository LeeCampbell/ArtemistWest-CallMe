using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProvider
    {
        string Name { get; }
        Uri Image { get; }
        AuthorizationStatus Status { get; }
        IResourceScope[] AvailableServices { get; }
        ObservableCollection<IResourceScope> SelectedServices { get; }
        ICommand AuthorizeCommand { get; }
    }
}
