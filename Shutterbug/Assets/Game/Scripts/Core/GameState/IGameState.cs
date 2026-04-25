
public interface IGameState 
{
    GameMode GameMode { get; }
    void Enter();
    void Exit();
    void Update();
}

public enum GameMode
{
    Exploration,
    Tablet,
    Photo
}
