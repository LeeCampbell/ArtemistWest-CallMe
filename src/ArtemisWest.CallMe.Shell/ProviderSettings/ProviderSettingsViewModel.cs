using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ArtemisWest.CallMe.Shell.ProviderSettings
{
    public sealed class ProviderSettingsViewModel : INotifyPropertyChanged
    {
        private readonly IEnumerable<Contract.IProvider> _providers;
        private bool _isAnyAuthenticated;

        public ProviderSettingsViewModel(IEnumerable<Contract.IProvider> providers)
        {
            _providers = providers.ToArray();
            foreach (var provider in _providers)
            {
                provider.PropertyChanged += Provider_PropertyChanged;
            }
            IsAnyAuthenticated = Providers.Any(p => p.Status.IsAuthorized);
        }
        
        public IEnumerable<Contract.IProvider> Providers
        {
            get { return _providers; }
        }

        public bool IsAnyAuthenticated
        {
            get { return _isAnyAuthenticated; }
            set
            {
                if (_isAnyAuthenticated != value)
                {
                    _isAnyAuthenticated = value;
                    OnPropertyChanged("");
                }
            }
        }

        void Provider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                IsAnyAuthenticated = Providers.Any(p => p.Status.IsAuthorized);
            }
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
