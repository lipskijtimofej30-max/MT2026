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
        var dist = Vector3.Distance(transform.position, _player.transform.position);
        return dist;
    }

    public Vector3 GetDirectionToPlayer(Transform transform)
    {
        return (transform.position - _player.transform.position).normalized;
    }
}
