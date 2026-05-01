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
    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private AnimalFactory _animalFactory;
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromInstance(_playerController);
        Container.BindInterfacesAndSelfTo<AnimalDataRegistry>().AsSingle();

        BindGame();
        BindGameState();
        BindUI();
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