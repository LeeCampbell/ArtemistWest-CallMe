using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProvider : IProviderDescription
    {
        AuthorizationStatus Status { get; }
        IResourceScope[] AvailableServices { get; }
        ObservableCollection<IResourceScope> SelectedServices { get; }
        ICommand AuthorizeCommand { get; }
    }
}
