using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Serializable, NetSerializable]
public record WhackTarget
{
    /// <summary>
    ///     The sprite this target uses.
    /// </summary>
    [DataField]
    public required SpriteSpecifier Sprite;

    /// <summary>
    ///     How many points this target gives (or takes away) when you hit it.
    /// </summary>
    [DataField]
    public required int Score;

    /// <summary>
    ///     The sprite this target uses when it is hit.
    /// </summary>
    [DataField]
    public SpriteSpecifier? HitSprite;

    /// <summary>
    ///     Whether this target is considered a "friend" (i.e. you should not hit it.)
    /// </summary>
    [DataField]
    public bool Friendly = false;
}

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
