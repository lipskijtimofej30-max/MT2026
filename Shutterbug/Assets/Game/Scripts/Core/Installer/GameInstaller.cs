using Game.Data;
using Game.Scripts;
using Game.Scripts.CameraPhoto;
using Game.Scripts.Core;
using Game.Scripts.Factory;
using Game.Scripts.Quest;
using Game.Scripts.UI;
using Game.Service;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("UI")]
    [SerializeField] private CameraCaptureView _cameraCaptureView;
    [SerializeField] private CameraCaptureController _cameraCaptureController;
    [SerializeField] private TabletView _tabletView;
    
    [Header("Player")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerBait _playerBait;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private DeathUI _deathUI;
    
    [Header("Other")]
    [SerializeField] private AnimalFactory _animalFactory;
    public override void InstallBindings()
    {
        Container.Bind<BaitRegistry>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<AnimalDataRegistry>().AsSingle();

        BindPlayer();
        BindGame();
        BindGameState();
        BindUI();
        Container.Bind<DataHandler>().AsSingle().NonLazy();
    }

    private void BindPlayer()
    {
        Container.Bind<PlayerController>().FromInstance(_playerController);
        Container.Bind<PlayerBait>().FromInstance(_playerBait).AsSingle();
        Container.Bind<IPlayerInventory>().To<PlayerInventory>().AsSingle();
        Container.Bind<Transform>().WithId("RespawnPoint").FromInstance(_respawnPoint).AsSingle();
        Container.Bind<DeathUI>().FromInstance(_deathUI).AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerDeathHandler>().AsSingle();
    }

    private void BindGame()
    {
        Container.Bind<GameMath>().To<GameMath>().AsSingle();
        Container.Bind<IAnimalFactory>().FromInstance(_animalFactory).AsSingle();
        Container.Bind<AnimalRegistry>().To<AnimalRegistry>().AsSingle();
    }

    private void BindGameState()
    {
        Container.Bind<ExplorationState>().AsSingle();
        Container.Bind<PhotoModeState>().AsSingle();
        Container.Bind<TabletState>().AsSingle();
        Container.Bind<GameStateMachine>().AsSingle();
    }

    private void BindUI()
    {
        Container.Bind<CameraCaptureView>().FromInstance(_cameraCaptureView).AsSingle();
        Container.Bind<TabletView>().FromInstance(_tabletView).AsSingle();
        Container.Bind<CameraCaptureController>().FromInstance(_cameraCaptureController).AsSingle();
    }
} 