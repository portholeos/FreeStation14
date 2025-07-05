using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Access(typeof(SharedWhackGameArcadeSystem), Friend = AccessPermissions.Read)]
[NetworkedComponent, AutoGenerateComponentState]
[RegisterComponent]
public sealed partial class WhackGameArcadeComponent : Component
{
    /// <summary>
    ///     How many possible target positions there are to hit.
    /// </summary>
    [AutoNetworkedField, DataField]
    public int TargetCount = 6;

    [ViewVariables]
    [Access(typeof(SharedWhackGameArcadeSystem), Friend = AccessPermissions.ReadWriteExecute)]
    public WhackGameData? Game = null;

    /// <summary>
    ///     List of all possible targets that can spawn.
    /// </summary>
    [DataField(required: true)]
    [Access(typeof(WhackGameData), Friend = AccessPermissions.ReadWriteExecute)]
    public List<WhackTarget> Targets = new();

    /// <summary>
    ///     Whether or not "game target hit" logic should be validated on the server.
    ///     Even if this is disabled, the server will still check if the target
    ///     data is valid, so clients can't just claim they hit a Target That Gives You
    ///     10000 Points
    /// </summary>
    /// <remarks>
    ///     Note that if this is enabled, players may be screwed over by lag, as the server
    ///     may receive client messages *after* targets get hidden. Disabling this
    ///     basically eliminates the lag issue, so it may be desirable if you're willing
    ///     to run the risk at people exploiting at fucking Whack-a-Mole
    /// </remarks>
    [DataField]
    public bool ValidateOnServer = true;

    /// <summary>
    ///     The sound effect that plays when the game starts.
    /// </summary>
    [DataField]
    public SoundSpecifier? NewGameSound;

    /// <summary>
    ///     The sound effect that plays when the player hits an enemy.
    /// </summary>
    /// <remarks>
    ///     This happens in addition to the "hit" sound that each individual Target can have.
    /// </remarks>
    [DataField]
    public SoundSpecifier? BonkSound;

    /// <summary>
    ///     The sound effect that plays when the game is "won", and a prize is dispensed.
    /// </summary>
    [DataField]
    public SoundSpecifier? WinSound;

    /// <summary>
    ///     The sound effect that plays when the game finishes without fulfilling the win condition.
    /// </summary>
    /// <remarks>
    ///     There is not an actual "lose" state in the game, so this sound is more like the default
    ///     Game Finished sound, and the win sound is a "You won a prize!" sound.
    /// </remarks>
    [DataField]
    public SoundSpecifier? GameOverSound;

    /// <summary>
    ///     The volume of all arcade sounds should play, in dB.
    /// </summary>
    [DataField]
    public float SoundVolume = -4f;

    /// <summary>
    ///     A float ranging from 0.0 to 1.0, representing the "performance" % you need to win the game.
    /// </summary>
    /// <remarks>
    ///     Performance is represented as points earned divided by the maximum "possible" number of points
    ///     you can get. When I was testing it, I averaged 70%-75% score when I was "tryharding" and the
    ///     win threshold probably shouldn't be much higher than that unless you adjust the difficulty of
    ///     the game in general, or want the game to be much harder to win.
    /// </remarks>
    [DataField]
    public float WinThreshold = 0.65f; // C's get degrees

    // How long a game session lasts.
    [DataField]
    public TimeSpan GameDuration = TimeSpan.FromSeconds(90);

    /// <summary>
    ///     Target Frequency is how frequently targets spawn.
    /// </summary>
    [DataField]
    public float StartingTargetFrequency = 4.0f;

    /// <summary>
    ///     Lowest possible spawning frequency, in seconds.
    /// </summary>
    [DataField]
    public float MinTargetFrequency = 0.8f;

    /// <summary>
    ///     Target Duration is how long a target lasts before disappearing.
    /// </summary>
    [DataField]
    public float StartingTargetDuration = 3.0f;

    /// <summary>
    ///     Lowest possible duration a target can have, in seconds.
    /// </summary>
    [DataField]
    public float MinTargetDuration = 1.0f;

    /// <summary>
    ///     Minimum difficulty needed for "friendly" targets to start spawning.
    /// </summary>
    [DataField]
    public float FriendChanceIncreaseStart = 20f;

    /// <summary>
    ///     Maximum difficulty for friend target chance to stop increasing.
    /// </summary>
    [DataField]
    public float FriendChanceIncreaseEnd = 90f;

    /// <summary>
    ///     The chance that a target should be a friend target when <see cref="FriendChanceIncreaseEnd"/> is reached.
    /// </summary>
    [DataField]
    public float FriendChanceTarget = 0.3f;

    /// <summary>
    ///     Minimum possible "friend" spawn chance a target can have.
    /// </summary>
    [DataField]
    public float MinFriendChance = 0.0f;

    /// <summary>
    ///     The maximum possible friend spawn chance.
    /// </summary>
    [DataField]
    public float MaxFriendChance = 0.4f;

    /// <summary>
    ///     Every time Difficulty increases by this threshold, another maximum target spawn is added.
    ///     I.e. If threshold is 10, and the difficulty is 18, then you get 1 + 1 = 2 max targets.
    ///     Once you reach 20 difficulty, it would increase to 1 + 2 = 3 max targets.
    /// </summary>
    [DataField]
    public float SpawnCountThreshold = 18.0f;

    /// <summary>
    ///     Maximum number of targets that can spawn at a time.
    /// </summary>
    [DataField]
    public int MaxTargetSpawns = 5;

    /// <summary>
    ///     "Difficulty" determines how fast targets spawn and disappear.
    /// </summary>
    [DataField]
    public float StartingDifficulty = 0f;

    /// <summary>
    ///     How fast Difficulty rises in one second.
    /// </summary>
    [DataField]
    public float DifficultyRate = 1.0f;

    /// <summary>
    ///     The difficulty at which Frequency/Duration are multiplied by zero.
    /// </summary>
    [DataField]
    public float DifficultyCurveEnd = 90f;
}
