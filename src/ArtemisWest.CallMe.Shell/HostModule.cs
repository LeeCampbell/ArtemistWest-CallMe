using ArtemisWest.CallMe.Shell.UI.Search;
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
            _container.RegisterType<Contract.ILocalStore, LocalStore>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISearchModel, SearchModel>(new ContainerControlledLifetimeManager());
            
            //var searchModel = _container.Resolve<Search.SearchModel>();
            //_container.RegisterInstance<Search.ISearchModel>(searchModel);

            var settingsView = _container.Resolve<ProviderSettings.ProviderSettingsView>();
            _regionManager.AddToRegion("ProviderSettingsRegion", settingsView);

            var searchView = _container.Resolve<SearchView>();
            _regionManager.AddToRegion("SearchRegion", searchView);

            var contactView = _container.Resolve<UI.Contact.ContactSearchResultsView>();
            _regionManager.AddToRegion("ContactRegion", contactView);
        }
    }
}