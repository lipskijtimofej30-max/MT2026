using Game.Scripts;
using Game.Scripts.Factory;
using Game.Scripts.Quest;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private AnimalFactory _animalFactory;
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromInstance(_playerController);
        BindGame();
    }

    private void BindGame()
    {
        Container.Bind<GameMath>().To<GameMath>().AsSingle();
        Container.Bind<IAnimalFactory>().FromInstance(_animalFactory).AsSingle();
        Container.Bind<AnimalRegistry>().To<AnimalRegistry>().AsSingle();
    }
} 