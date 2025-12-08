using Audio;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplaySettings _gameplaySettings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CameraSwitch>().AsSingle();
            Container.Bind<GameplaySettings>().FromInstance(_gameplaySettings).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerInteraction>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameStateMusicController>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("GameStateMusic")
                .AsSingle()
                .NonLazy();
        }
    }
}
