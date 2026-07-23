using UnityEngine;

public interface IGameMode
{
    void Initialize();

    void StartRound();

    void EndRound();

    void Cleanup();
}