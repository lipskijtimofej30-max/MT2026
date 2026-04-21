using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts;

public interface IState
{
    AnimalState StateType { get; }
    UniTask<StateAction> OnEnter(CancellationToken ct);
}
