using Game.Scripts;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private PlayerController _playerController;
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromInstance(_playerController);
        BindGame();
    }

    private void BindGame()
    {
        Container.Bind<GameMath>().To<GameMath>().AsSingle();
    }
} 