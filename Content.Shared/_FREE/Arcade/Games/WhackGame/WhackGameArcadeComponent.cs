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

    [ViewVariables, Access(typeof(SharedWhackGameArcadeSystem), Friend = AccessPermissions.ReadWriteExecute)]
    public WhackGameData? Game = null;

    /// <summary>
    ///     List of all possible targets that can spawn.
    /// </summary>
    [DataField(required: true)]
    public List<WhackTarget> Targets = new();

    // How long a game session lasts.
    [DataField]
    public TimeSpan GameDuration = TimeSpan.FromSeconds(120);

    /// <summary>
    ///     Target Frequency is how frequently targets spawn.
    /// </summary>
    [DataField]
    public float StartingTargetFrequency = 4.0f;

    /// <summary>
    ///     Lowest possible spawning frequency, in seconds.
    /// </summary>
    [DataField]
    public float MinTargetFrequency = 0.4f;

    /// <summary>
    ///     Target Duration is how long a target lasts before disappearing.
    /// </summary>
    [DataField]
    public float StartingTargetDuration = 2.5f;

    /// <summary>
    ///     Lowest possible duration a target can have, in seconds.
    /// </summary>
    [DataField]
    public float MinTargetDuration = 0.5f;

    /// <summary>
    ///     Minimum difficulty needed for "friendly" targets to start spawning.
    /// </summary>
    [DataField]
    public float FriendChanceIncreaseStart = 20f;

    /// <summary>
    ///     Maximum difficulty for friend target chance to stop increasing.
    /// </summary>
    [DataField]
    public float FriendChanceIncreaseEnd = 120f;

    /// <summary>
    ///     The chance that a target should be a friend target when <see cref="FriendChanceIncreaseEnd"/> is reached.
    /// </summary>
    [DataField]
    public float FriendChanceTarget = 0.4f;

    /// <summary>
    ///     Minimum possible "friendly" spawn chance a target can have.
    /// </summary>
    [DataField]
    public float MinFriendChance = 0.0f;

    /// <summary>
    ///     The maximum possible friend spawn chance.
    /// </summary>
    [DataField]
    public float MaxFriendChance = 0.5f;

    /// <summary>
    ///     Every time Difficulty increases by this threshold, another maximum target spawn is added.
    ///     I.e. If threshold is 10, and the difficulty is 18, then you get 1 + 1 = 2 max targets.
    ///     Once you reach 20 difficulty, it would increase to 1 + 2 = 3 max targets.
    /// </summary>
    [DataField]
    public float SpawnCountThreshold = 30.0f;

    /// <summary>
    ///     Maximum number of targets that can spawn at a time.
    /// </summary>
    [DataField]
    public int MaxTargetSpawns = 4;

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
}
