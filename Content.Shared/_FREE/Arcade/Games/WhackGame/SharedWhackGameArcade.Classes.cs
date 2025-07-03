using Robust.Shared.Serialization;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Serializable, NetSerializable]
public sealed class WhackGameArcadePlayerActionMessage(WhackGamePlayerAction action,
    int? target = null) : BoundUserInterfaceMessage
{
    public readonly WhackGamePlayerAction Action = action;
    public readonly int? Target = target;
}

[Serializable, NetSerializable]
public enum WhackGameArcadeUiKey
{
    Key,
}

[Serializable, NetSerializable]
public enum WhackGamePlayerAction
{
    WhackTarget,
    StartGame,
    RequestData,
}

/// <summary>
///     Updates the current state of the game.
/// </summary>
[Serializable, NetSerializable]
public sealed class WhackGameArcadeGameUIState : BoundUserInterfaceState
{
    public readonly int Score;
    public readonly int TimeLeft;
    public readonly Dictionary<int, WhackTarget> ActiveTargets;
    public readonly bool EndGame = false;

    public WhackGameArcadeGameUIState(int score,
        int timeLeft,
        Dictionary<int, WhackTarget> targets,
        bool endGame = false)
    {
        Score = score;
        TimeLeft = timeLeft;
        ActiveTargets = targets;
        EndGame = endGame;
    }
}
