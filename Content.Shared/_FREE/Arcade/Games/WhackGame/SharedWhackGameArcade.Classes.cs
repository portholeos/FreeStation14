using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[DataDefinition, Serializable, NetSerializable]
public partial record WhackTarget
{
    /// <summary>
    ///     The sprite this target uses.
    /// </summary>
    [DataField(required: true)]
    public SpriteSpecifier Sprite;

    /// <summary>
    ///     How many points this target gives (or takes away) when you hit it.
    /// </summary>
    [DataField(required: true)]
    public int Score;

    /// <summary>
    ///     The sprite this target uses when it is hit.
    /// </summary>
    [DataField]
    public SpriteSpecifier? HitSprite = null;

    /// <summary>
    ///     The sound effect that plays when this target is hit.
    /// </summary>
    [DataField]
    public SoundSpecifier? BonkSound;

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
