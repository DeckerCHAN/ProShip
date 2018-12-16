using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LibProShip.Domain;

namespace LibProShip.Domain2.FileSystem
{
    public class Installer: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInit,IFileMonitor>().ImplementedBy<FileMonitor>().LifestyleSingleton());
        }
    }
}