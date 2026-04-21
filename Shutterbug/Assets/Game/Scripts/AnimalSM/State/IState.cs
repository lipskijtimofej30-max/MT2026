using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts;

public interface IState
{
    UniTask<StateAction> OnEnter(CancellationToken ct);
}
