using Game.Scripts.CameraPhoto.PhotoAlbum;
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
            BindPhotoAlbum();
        }

        private void BindQuestService()
        {
            Container.Bind<QuestDatabase>().FromInstance(_questDatabase).AsSingle();
            Container.BindInterfacesAndSelfTo<QuestService>().AsSingle();
        }

        private void BindProgressionService()
        {
            Container.Bind<IProgressionService>().To<ProgressionService>().AsSingle();
            Container.Bind<CameraUpgradesConfig>().FromInstance(_cameraConfig).AsSingle();
        }

        private void BindPhotoAlbum()
        {
            Container.Bind<PhotoRegistry>().AsSingle();
            Container.Bind<PhotoService>().AsSingle();
        }
    }
}