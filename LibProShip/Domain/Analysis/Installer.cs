using System.Linq;
using System.Reflection;
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
            
            var types = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(IAnalyser).IsAssignableFrom(x));

            foreach (var type in types)
            {
                container.Register(Component.For<IAnalyser>().ImplementedBy(type).LifestyleTransient());
            }
        }
    }
}