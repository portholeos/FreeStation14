using Content.Shared.FREE.Arcade;
using Content.Shared.FREE.Arcade.Systems;
using JetBrains.Annotations;

namespace Content.Server.FREE.Arcade.Systems;

public sealed partial class ArcadeSystem : SharedArcadeSystem
{
    /// <summary>
    ///     Ends the arcade game on a success.
    /// </summary>
    /// <param name="player">The entity playing the game.</param>
    /// <param name="machine">The arcade machine entity.</param>
    [PublicAPI]
    public void WinGame(EntityUid? player, EntityUid machine)
    {
        FinishGame(player, machine, ArcadeGameResult.Win);
    }

    /// <summary>
    ///     Ends the arcade game on a loss.
    /// </summary>
    /// <param name="player">The entity playing the game.</param>
    /// <param name="machine">The arcade machine entity.</param>
    [PublicAPI]
    public void LoseGame(EntityUid? player, EntityUid machine)
    {
        FinishGame(player, machine, ArcadeGameResult.Fail);
    }

    /// <summary>
    ///     Ends the arcade game without finishing it.
    /// </summary>
    /// <param name="player">The entity playing the game.</param>
    /// <param name="machine">The arcade machine entity.</param>
    [PublicAPI]
    public void LeaveGame(EntityUid? player, EntityUid machine)
    {
        FinishGame(player, machine, ArcadeGameResult.Forfeit);
    }

    /// <summary>
    ///     Ends the arcade game on a draw (game finished, neither win nor lose).
    /// </summary>
    /// <param name="player">The entity playing the game.</param>
    /// <param name="machine">The arcade machine entity.</param>
    [PublicAPI]
    public void DrawGame(EntityUid? player, EntityUid machine)
    {
        FinishGame(player, machine, ArcadeGameResult.Draw);
    }

    private void FinishGame(EntityUid? player, EntityUid machine, ArcadeGameResult result)
    {
        var endedEvent = new ArcadeGameEndedEvent(player, result);
        var finishEvent = new FinishedArcadeGameEvent(result);

        RaiseLocalEvent(machine, endedEvent);
        if (player != null)
            RaiseLocalEvent(player.Value, finishEvent);
    }
}
