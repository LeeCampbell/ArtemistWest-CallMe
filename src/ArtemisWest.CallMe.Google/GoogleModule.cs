using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ArtemisWest.CallMe.Google
{
    [Module(ModuleName = "GoogleModule")]
    public class GoogleModule : IModule
    {
        private readonly ILogger _logger;

        public GoogleModule(IUnityContainer container, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger();
            _logger.Debug("GoogleModule.ctor()");

            container.RegisterType<Contract.IProvider, GoogleProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<Contract.IIdentityProvider, Contacts.GoogleIdentityProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<Contract.IResourceScope, Contacts.ContactResourceScope>(new ContainerControlledLifetimeManager());
            //container.RegisterType<Contract.IResourceScope, Contacts.CalendarResourceScope>();
            //container.RegisterType<Contract.IResourceScope, Contacts.DocsResourceScope>();
            //container.RegisterType<Contract.IResourceScope, Contacts.EmailResourceScope>();
            
#if OFFLINE
            container.RegisterType<Authorization.IAuthorizationModel, Authorization.OfflineAuthorizationModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<Contract.Contacts.IContactQueryProvider, Contacts.OfflineContactQueryProvider>(new ContainerControlledLifetimeManager());
#else
            container.RegisterType<Authorization.IGoogleLoginView, Authorization.GoogleLoginView>();
            container.RegisterType<Authorization.IAuthorizationModel, Authorization.AuthorizationModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWebRequstService, WebRequstService>(new ContainerControlledLifetimeManager());
            container.RegisterType<Contract.Contacts.IContactQueryProvider, Contacts.GoogleContactQueryProvider>(new ContainerControlledLifetimeManager());
#endif
        }

        public void Initialize()
        {
            //Register the providers. The thing that allows the user to choose google as an auth and selecte the services we request.
            //Show the available providers (and their state)
            //  Clicking on them starts the login process (Modal) so you cant try to do 2 at once

            //When making a service request, it should be passed via the serivce/repo that can communicate to thte Auth model that Auth has lapsed. 
            //  It should then disable the Provider and it's child services
            _logger.Debug("GoogleModule.Initialize()");

        }
    }
}
