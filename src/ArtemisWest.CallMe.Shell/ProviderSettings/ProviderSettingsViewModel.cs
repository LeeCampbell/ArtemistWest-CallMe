using System.Collections.Generic;

namespace ArtemisWest.CallMe.Shell.ProviderSettings
{
    public sealed class ProviderSettingsViewModel
    {
        private readonly IEnumerable<Contract.IProvider> _providers;
        
        public ProviderSettingsViewModel(IEnumerable<Contract.IProvider> providers)
        {
            _providers = providers;
        }
        
        public IEnumerable<Contract.IProvider> Providers
        {
            get { return _providers; }
        }
    }
}
