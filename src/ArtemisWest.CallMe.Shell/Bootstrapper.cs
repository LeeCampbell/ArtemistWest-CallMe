using System.Windows;
using ArtemisWest.CallMe.Shell.UnityExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace ArtemisWest.CallMe.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {
            return base.CreateLogger();
            //return new Log4NetFacade();   //Needs to be configured
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
