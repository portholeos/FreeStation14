using Robust.Shared.Serialization;

namespace Content.Shared.FREE.Arcade;

/// <summary>
///     Called on the arcade machine entity when a game ends for any reason.
/// </summary>
/// <param name="player">The entity playing the arcade game.</param>
/// <param name="result">The result of the game.</param>
public sealed class ArcadeGameEndedEvent(EntityUid? player,
    ArcadeGameResult result = ArcadeGameResult.Forfeit)
    : EntityEventArgs
{
    public EntityUid? Player = player;
    public ArcadeGameResult Result = result;
}

/// <summary>
///     Called on the player entity when they finish an arcade game for any reason.
/// </summary>
/// <param name="result">The result of the game.</param>
public sealed class FinishedArcadeGameEvent(ArcadeGameResult result) : EntityEventArgs
{
    public ArcadeGameResult Result = result;
}

[Serializable, NetSerializable]
public enum ArcadeGameResult
{
    Win,
    Draw,
    Forfeit,
    Fail,
}
