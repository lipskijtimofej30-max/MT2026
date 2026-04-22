using Game.Scripts.Quest;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class ServiceInstaller : MonoInstaller
    {
        [SerializeField] private CameraUpgradesConfig _cameraConfig;
        [SerializeField] private QuestDatabase _questDatabase;
        override public void InstallBindings()
        {
            Container.Bind<IPhotoProvider>().To<PhotoProvider>().AsSingle();
            Container.Bind<PhotoEvaluator>().To<PhotoEvaluator>().AsSingle();
            
            Container.Bind<IAnimalInPhotoProvider>().To<AnimalInPhotoProvider>().AsSingle();
            BindQuestService();

            BindProgressionService();
        }

        private void BindQuestService()
        {
            Container.Bind<QuestDatabase>().FromInstance(_questDatabase).AsSingle();
            Container.Bind<QuestService>().To<QuestService>().AsSingle();
        }

        private void BindProgressionService()
        {
            Container.Bind<IProgressionService>().To<ProgressionService>().AsSingle();
            Container.Bind<CameraUpgradesConfig>().FromInstance(_cameraConfig).AsSingle();
        }
    }
}