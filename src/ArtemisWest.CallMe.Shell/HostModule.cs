using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace ArtemisWest.CallMe.Shell
{
    public sealed class HostModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public HostModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            var settingsView = _container.Resolve<ProviderSettings.ProviderSettingsView>();
            _regionManager.AddToRegion("ProviderSettingsRegion", settingsView);
        }
    }
}