using Game.Signals;
using Zenject;

namespace Game.Core
{
    public class SignalInstaller : MonoInstaller
    {
        override public void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<PlayerCrouchedSignal>().OptionalSubscriber();
            Container.DeclareSignal<LevelChangeSignal>().OptionalSubscriber();
        }
    }
}