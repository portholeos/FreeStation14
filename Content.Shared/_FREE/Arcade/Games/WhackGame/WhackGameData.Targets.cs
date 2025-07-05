using System.Linq;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Shared.Random;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

public sealed partial class WhackGameData
{
    [Dependency] private readonly IRobustRandom _random = default!;

    /// <summary>
    ///     How frequently targets spawn. Scales inversely with <see cref="Difficulty"/>.
    /// </summary>
    [ViewVariables] public TimeSpan TargetFrequency;

    /// <summary>
    ///     How long targets stay visible before disappearing. Scales inversely with <see cref="Difficulty"/>.
    /// </summary>
    [ViewVariables] public TimeSpan TargetDuration;

    /// <summary>
    ///     How likely "friendly" targets should spawn. Friendly targets should not be hit -
    ///     they serve to interrupt players' muscle memory!
    ///     Scales with <see cref="Difficulty"/>
    /// </summary>
    [ViewVariables] public float FriendSpawnChance = 0.0f;

    /// <summary>
    ///     How many targets can spawn at a time.
    ///     Scales with <see cref="Difficulty"/>
    /// </summary>
    [ViewVariables] public int RandomTargetSpawnMax = 1;

    private IReadOnlyList<WhackTarget> FriendlyTargets => Comp.Targets.Where(t => t.Friendly).ToList();
    private IReadOnlyList<WhackTarget> EnemyTargets => Comp.Targets.Where(t => !t.Friendly).ToList();

    [ViewVariables(VVAccess.ReadOnly)] public TimeSpan NextTargetSpawn = TimeSpan.Zero;
    private List<int> _availablePositions = new();
    private Dictionary<int, (WhackTarget, TimeSpan)> _currentTargets = new();

    private void InitializeTargets()
    {
        _availablePositions = Enumerable.Range(0, Comp.TargetCount).ToList();
        UpdateTargetDifficulty();

        // Give it a few seconds before spawning the first one
        NextTargetSpawn = _timing.CurTime + TargetFrequency;
    }

    public void HitTarget(int position)
    {
        if (GameEnded)
            return;
        if (!_currentTargets.TryGetValue(position, out var targetData))
            return;

        PlaySound(Comp.BonkSound);
        PlaySound(targetData.Item1.BonkSound);

        Score += targetData.Item1.Score;
        RemoveTarget(position);
        UpdateUI();
    }

    /// <summary>
    ///     Adds new targets when the time is right, then removes expired ones.
    ///     This order of operations means that, for example, a target cannot appear in a position
    ///     in the same "cycle" that it has been freed up.
    /// </summary>
    private void UpdateTargets(ref bool updateState)
    {
        UpdateTargetDifficulty();

        if (_timing.CurTime >= NextTargetSpawn)
        {
            SpawnTargetWave();
            updateState = true;
        }

        var targetsToRemove = _currentTargets
            .Where(t => _timing.CurTime > t.Value.Item2)
            .Select(t => t.Key);

        if (targetsToRemove.Count() > 0)
        {
            foreach (var position in targetsToRemove)
                RemoveTarget(position);
            updateState = true;
        }
    }

    private void UpdateTargetDifficulty()
    {
        var difficultyScale = Math.Clamp(Difficulty / Comp.DifficultyCurveEnd, 0f, 1.0f);
        var inverseDifficulty = 1f - difficultyScale;

        var bonusTargetSpawn = (int)(Difficulty / Comp.SpawnCountThreshold);
        RandomTargetSpawnMax = Math.Clamp(1 + bonusTargetSpawn, 1, Comp.MaxTargetSpawns);

        // Frequency and Duration scale inversely with difficulty.
        // This scale is linear from difficulty (0, 1.0) to (100, 0.0), which then gets bounded by minimumm.
        var scaledFrequency = Comp.StartingTargetFrequency * inverseDifficulty;
        var clampedFrequency = Math.Max(scaledFrequency, Comp.MinTargetFrequency);
        TargetFrequency = TimeSpan.FromSeconds(clampedFrequency);

        var scaledDuration = Comp.StartingTargetDuration * inverseDifficulty;
        var clampedDuration = Math.Max(scaledDuration, Comp.MinTargetDuration);
        TargetDuration = TimeSpan.FromSeconds(clampedDuration);

        // Friend chance goes from (FriendChanceIncreaseStart, 0) to (FriendChanceIncreaseEnd, FriendChanceTarget).
        // Clamped between MinFriendChance and MaxFriendChance. Chances are expressed as floats between 0 and 1.
        var friendTargetTime = Comp.FriendChanceIncreaseEnd - Comp.FriendChanceIncreaseStart;
        var fT = Math.Max(Comp.MinFriendChance, Difficulty - Comp.FriendChanceIncreaseStart) / friendTargetTime;
        FriendSpawnChance = Math.Clamp(fT * Comp.FriendChanceTarget, Comp.MinFriendChance, Comp.MaxFriendChance);
    }

    private void SpawnTargetWave()
    {
        for (var i = 0; i < _random.Next(1, RandomTargetSpawnMax); i++)
            SpawnTarget();

        NextTargetSpawn = _timing.CurTime + TargetFrequency;
    }

    private void SpawnTarget()
    {
        var target = GetNextTarget();
        var position = GetNextPosition();
        if (position == -1)
            return;

        var expireTime = _timing.CurTime + TargetDuration;

        if (target.Score > 0)
            TotalPossibleScore += target.Score;

        var targetData = (target, expireTime);

        _currentTargets.Add(position, targetData);
    }

    private int GetNextPosition()
    {
        if (_availablePositions.Count == 0)
            return -1;

        var position = _availablePositions[0];
        _availablePositions.RemoveAt(0);
        return position;
    }

    private void RemoveTarget(int position)
    {
        _currentTargets.Remove(position);
        _availablePositions.Add(position);
        _random.Shuffle(_availablePositions);
    }

    private WhackTarget GetNextTarget()
    {
        var targets = NextTargetIsFriendly() ? FriendlyTargets : EnemyTargets;
        return _random.Pick(targets);
    }

    private bool NextTargetIsFriendly()
    {
        return FriendSpawnChance > 0.0f
            && FriendlyTargets.Any()
            && (FriendSpawnChance == 1.0f || _random.NextFloat() <= FriendSpawnChance);
    }
}
