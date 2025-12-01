using UnityEngine;
using Zenject;
using BalatroFeel.Scripts.SaveSystem;

namespace BalatroFeel.Scripts
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SaveRegistry>().AsSingle();
            Container.Bind<SaveFileStorage>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle();
        }
    }
}
