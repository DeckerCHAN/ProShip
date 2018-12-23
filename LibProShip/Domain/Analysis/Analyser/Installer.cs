using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LibProShip.Domain.Analysis.Analyser
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(ISphereAnalyser).IsAssignableFrom(x));

            foreach (var type in types)
            {
                container.Register(Component.For<ISphereAnalyser>().ImplementedBy(type).LifestyleTransient());
            }
        }
    }
}