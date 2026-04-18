using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class ServiceInstaller : MonoInstaller
    {
        override public void InstallBindings()
        {
            Container.Bind<IPhotoProvider>().To<PhotoProvider>().AsSingle();
        }
    }
}