using System.Threading;
using System.Windows;
using ArtemisWest.CallMe.Shell.UnityExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

//TODO: Create a common project for ObservableEx, Converters, LogEx
//TODO: Add functionality that Logs when an Object is constructed from the Container.
namespace ArtemisWest.CallMe.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        private readonly LoggerFactory _loggerFactory;

        public Bootstrapper()
        {
            Thread.CurrentThread.Name = "UI";
            _loggerFactory = new LoggerFactory();
            var logger = _loggerFactory.GetLogger();
            logger.Info("Starting application");
        }

        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {           
            var logger = _loggerFactory.GetLogger();
            return (Microsoft.Practices.Prism.Logging.ILoggerFacade)logger;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog
                       {
                           ModulePath = "Modules",
                       };
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            
            var hostModuleType = typeof(HostModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = hostModuleType.Name,
                ModuleType = hostModuleType.AssemblyQualifiedName,
            });
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.AddNewExtension<GenericSupportExtension>();
            Container.RegisterInstance<ILoggerFactory>(_loggerFactory);
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }

        protected override Microsoft.Practices.Prism.Regions.RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            return base.ConfigureRegionAdapterMappings();
        }

        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            return base.ConfigureDefaultRegionBehaviors();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
        }

        protected override DependencyObject CreateShell()
        {
            var shell = new MainWindow();

            Application.Current.MainWindow = shell;
            shell.Show();
            return shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
        }
    }
}
