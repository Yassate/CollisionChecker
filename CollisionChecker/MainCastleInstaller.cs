using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CollisionChecker.LogicClasses;

namespace CollisionChecker
{
    class MainCastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ViewModel>());
            container.Register(Component.For<ICollectedDataChecker>().ImplementedBy<CollectedDataChecker>());
            container.Register(Component.For<IFilePathUtilities>().ImplementedBy<FilePathUtilities>());
            container.Register(Component.For<INotifier>().ImplementedBy<Notifier>());
            container.Register(Component.For<IDataReaderFactory>().ImplementedBy<DataReaderFactory>());
            container.Register(Component.For<ICollisionFactory>().ImplementedBy<CollisionFactory>());
            container.Register(Component.For<IRobotFactory>().ImplementedBy<RobotFactory>());

        }
    }
}
