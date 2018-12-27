using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LibProShip.Domain.Analysis
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IAnalysisManager>().ImplementedBy<AnalysisManager>().LifestyleSingleton());
        }
    }
}