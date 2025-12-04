using Gameplay;
using Zenject;

namespace Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateService>().AsSingle().NonLazy();
        }
    }
}