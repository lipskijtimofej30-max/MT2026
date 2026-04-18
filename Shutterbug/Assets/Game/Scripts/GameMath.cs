using Game.Scripts;
using UnityEngine;
using Zenject;

public class GameMath 
{
    private PlayerController _player;

    [Inject]
    private void Construct(PlayerController playerController)
    {
        _player = playerController;
    }

    public float DistanceToPlayer(Transform transform)
    {
        return Vector3.Distance(transform.position, _player.gameObject.transform.position);
    }

    public Vector3 GetDirectionToPlayer(Transform transform)
    {
        return (transform.position - _player.transform.position).normalized;
    }
}