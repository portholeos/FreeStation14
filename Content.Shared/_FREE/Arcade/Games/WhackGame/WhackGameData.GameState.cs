using System.Linq;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

public sealed partial class WhackGameData
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    private readonly SharedAudioSystem _audioSystem = default!;

    private readonly EntityUid _machineID;
    public readonly WhackGameArcadeComponent Comp;

    /// <summary>
    ///     When the game is supposed to end.
    /// </summary>
    [ViewVariables] public readonly TimeSpan EndTime;

    [ViewVariables] public bool GameEnded { get; private set; } = false;

    /// <summary>
    ///     How many points the player has earned.
    /// </summary>
    [ViewVariables] public int Score = 0;

    /// <summary>
    ///     How many points players can obtain this game total.
    ///     This value increases dynamically, beause we don't know how many targets are actually
    ///     going to spawn in the game! This allows us to get a more accurate "performance" rating,
    ///     e.g. "90% score".
    /// </summary>
    [ViewVariables] public int TotalPossibleScore = 0;

    /// <summary>
    ///     This determines the frequency of target spawns, how fast they disappear, and how likely they
    ///     are to be "friendly" targets (which you should not hit). Difficulty increases over time
    ///     as the game continues (I.e. it gets faster).
    /// </summary>
    [ViewVariables] public float Difficulty = 0f;

    [ViewVariables(VVAccess.ReadOnly)] public TimeSpan TimeLeft => EndTime - _timing.CurTime;
    [ViewVariables(VVAccess.ReadOnly)] public int TimeLeftInSeconds => (int)TimeLeft.TotalSeconds;
    [ViewVariables(VVAccess.ReadOnly)] public int PreviousTime = 0;

    public event Action<WhackGameArcadeGameUIState>? OnUIUpdate;

    public WhackGameData(Entity<WhackGameArcadeComponent> entity)
    {
        IoCManager.InjectDependencies(this);
        _audioSystem = _entityManager.System<SharedAudioSystem>();

        _machineID = entity.Owner;
        Comp = entity.Comp;

        Difficulty = Comp.StartingDifficulty;
        EndTime = _timing.CurTime + Comp.GameDuration;

        InitializeTargets();
    }

    /// <summary>
    ///     Starts the game "officially" - in this case, it just plays the new game sound.
    /// </summary>
    public void StartGame()
    {
        PlaySound(Comp.NewGameSound);
    }

    /// <summary>
    ///     Updates the game progression by one frame.
    /// </summary>
    /// <param name="frameTime">How much time has elapsed since the previous frame.</param>
    /// <returns>Whether the game should continue.</returns>
    public bool Update(float frameTime)
    {
        if (GameEnded)
            return false;

        var updateState = false;

        if (TimeLeftInSeconds != PreviousTime)
        {
            PreviousTime = TimeLeftInSeconds;
            updateState = true;
        }

        if (_timing.CurTime > EndTime) // No Need for UI update; EndGame will handle it
            return false;

        Difficulty += Comp.DifficultyRate * frameTime;
        UpdateTargets(ref updateState);

        if (updateState)
            UpdateUI();

        return true;
    }

    public void EndGame()
    {
        GameEnded = true;

        foreach (var (pos, _) in _currentTargets)
            RemoveTarget(pos);

        var sound = GetPerformance() > Comp.WinThreshold ? Comp.WinSound : Comp.GameOverSound;
        PlaySound(sound);
    }

    public float GetPerformance()
    {
        return (float)Score / TotalPossibleScore;
    }

    public void UpdateUI(bool endGame = false)
    {
        var state = GetUIState(endGame);
        OnUIUpdate?.Invoke(state);
    }

    private WhackGameArcadeGameUIState GetUIState(bool endGame = false)
    {
        var targets = _currentTargets.ToDictionary(t => t.Key, t => t.Value.Item1);
        var state = new WhackGameArcadeGameUIState(
            score: Score,
            timeLeft: (int)TimeLeft.TotalSeconds,
            targets: targets,
            endGame: endGame
        );

        return state;
    }

    private void PlaySound(SoundSpecifier? sound)
    {
        if (sound != null)
            _audioSystem.PlayPvs(sound, _machineID, AudioParams.Default.WithVolume(Comp.SoundVolume));
    }
}
