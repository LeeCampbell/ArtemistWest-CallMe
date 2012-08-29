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
            var searchModel = _container.Resolve<Search.SearchModel>();
            _container.RegisterInstance<Search.ISearchModel>(searchModel);

            var settingsView = _container.Resolve<ProviderSettings.ProviderSettingsView>();
            _regionManager.AddToRegion("ProviderSettingsRegion", settingsView);

            var searchView = _container.Resolve<Search.SearchView>();
            _regionManager.AddToRegion("SearchRegion", searchView);
        }
    }
}