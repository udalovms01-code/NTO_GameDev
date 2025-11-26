using UnityEngine;
using Zenject;

namespace BalatroFeel.Scripts
{
    public class GlobalInstaller : MonoInstaller
    {

        //[SerializeField] private GameContext _gameContext;
        
        public override void InstallBindings()
        {
            //Container.BindInterfacesAndSelfTo<>().AsSingle(); 
            //Container.Bind<GameContext>().FromInstance(_gameContext);
            
            
        }
    }
}