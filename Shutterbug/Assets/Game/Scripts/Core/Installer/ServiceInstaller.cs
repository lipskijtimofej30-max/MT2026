using Game.Scripts.CameraPhoto.PhotoAlbum;
using Game.Scripts.Data;
using Game.Scripts.Quest;
using Game.Scripts.Service;
using Game.Service;
using Game.Service.Currency;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class ServiceInstaller : MonoInstaller
    {
        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private StatUpgradesDatabase _statUpgradesDatabase;
        [SerializeField] private QuestDatabase _questDatabase;
        [SerializeField] private PhotoRewardConfig _rewardConfig;
        override public void InstallBindings()
        {
            Container.Bind<IPhotoProvider>().To<PhotoProvider>().AsSingle();
            Container.Bind<PhotoEvaluator>().To<PhotoEvaluator>().AsSingle();
            
            Container.Bind<IAnimalInPhotoProvider>().To<AnimalInPhotoProvider>().AsSingle();
            
            Container.Bind<ICurrencyService>().To<CurrencyService>().AsSingle();
            Container.Bind<ShopService>().AsSingle();
            Container.Bind<ItemDatabase>().FromInstance(_itemDatabase).AsSingle();
            
            BindQuestService();
            BindProgressionService();
            BindPhotoAlbum();
            BindRewardService();
        }

        private void BindQuestService()
        {
            Container.Bind<QuestDatabase>().FromInstance(_questDatabase).AsSingle();
            Container.BindInterfacesAndSelfTo<QuestService>().AsSingle();
        }

        private void BindRewardService()
        {
            Container.Bind<PhotoRewardConfig>().FromInstance(_rewardConfig).AsSingle();
            Container.Bind<PhotoRewardService>().To<PhotoRewardService>().AsSingle().NonLazy();

        }
        private void BindProgressionService()
        {
            Container.Bind<IProgressionService>().To<ProgressionService>().AsSingle();
            Container.Bind<StatUpgradesDatabase>().FromInstance(_statUpgradesDatabase).AsSingle();
        }

        private void BindPhotoAlbum()
        {
            Container.Bind<PhotoRegistry>().AsSingle();
            Container.Bind<PhotoService>().AsSingle();
            Container.Bind<IPhotoRecordProvider>().To<PhotoRecordProvider>().AsSingle();
        }
    }
}