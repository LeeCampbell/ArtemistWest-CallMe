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
            _container.RegisterType<ISchedulerProvider, SchedulerProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Contract.ILocalStore, LocalStore>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Services.IActivatedIdentityListener, Services.BluetoothActivatedIdentityListener>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Services.IActiveProfileService, Services.ActiveProfileService>(new ContainerControlledLifetimeManager());
            
            var settingsView = _container.Resolve<ProviderSettings.ProviderSettingsView>();
            _regionManager.AddToRegion("ProviderSettingsRegion", settingsView);

            var contactView = _container.Resolve<UI.Contact.ContactSearchResultsView>();
            _regionManager.AddToRegion("ContactRegion", contactView);
        }
    }
}