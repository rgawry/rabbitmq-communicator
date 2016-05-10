using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Chat
{
    public class Bootstrapper
    {
        public static IWindsorContainer BootstrapContainer()
        {
            return new WindsorContainer().Install(FromAssembly.InThisApplication());
        }
    }
}
