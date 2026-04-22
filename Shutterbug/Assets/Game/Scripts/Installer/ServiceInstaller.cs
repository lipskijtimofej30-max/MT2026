using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class ServiceInstaller : MonoInstaller
    {
        [SerializeField] private CameraUpgradesConfig _cameraConfig;
        override public void InstallBindings()
        {
            Container.Bind<IPhotoProvider>().To<PhotoProvider>().AsSingle();
            
            Container.Bind<PhotoEvaluator>().To<PhotoEvaluator>().AsSingle();
            
            BindProgressionService();
        }

        private void BindProgressionService()
        {
            Container.Bind<IProgressionService>().To<ProgressionService>().AsSingle();
            Container.Bind<CameraUpgradesConfig>().FromInstance(_cameraConfig).AsSingle();
        }
    }
}