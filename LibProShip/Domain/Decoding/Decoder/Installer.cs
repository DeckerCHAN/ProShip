using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LibProShip.Domain.Decoding.Decoder
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var decoderTypes = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(IDecoder).IsAssignableFrom(x));

            foreach (var decoderType in decoderTypes)
                container.Register(Component.For<IDecoder>().ImplementedBy(decoderType).LifestyleTransient());
        }
    }
}