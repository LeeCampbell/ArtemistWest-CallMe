using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProvider : IProviderDescription, INotifyPropertyChanged
    {
        AuthorizationStatus Status { get; }
        IResourceScope[] AvailableServices { get; }
        ObservableCollection<IResourceScope> SelectedServices { get; }
        ICommand AuthorizeCommand { get; }
    }
}
