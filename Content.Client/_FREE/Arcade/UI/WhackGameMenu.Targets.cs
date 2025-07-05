using System.Linq;
using System.Numerics;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
    private readonly SpriteSystem _spriteSystem;
    private readonly int _targetsPerRow = 3;
    private readonly int _gapSize = 32;
    private readonly int _spriteSize = 32;

    private GridContainer? _targetContainer;
    private List<WhackButton> _targets = new();
    private Dictionary<int, WhackTarget> _lastTargets = new();


    private void InitiateTargets()
    {
        var targetContainer = new GridContainer()
        {
            Columns = _targetsPerRow,
            HorizontalAlignment = HAlignment.Center,
            HorizontalExpand = true,
            VerticalExpand = true
        };

        _targetContainer = targetContainer;
        _gameBox?.AddChild(targetContainer);
    }

    private void PopulateTargets()
    {
        if (_targetContainer == null)
            InitiateTargets();

        _targetContainer?.RemoveAllChildren();
        _targets.Clear();

        if (TargetCount <= 0)
            return;

        var rows = TargetCount / _targetsPerRow;
        var heightAvailable = _windowSize.Y / rows - _gapSize;
        var maxHeight = _spriteSize * MathF.Floor(heightAvailable / _spriteSize);

        for (int i = 0; i < TargetCount; i++)
            _targets.Add(CreateTarget(i, new Vector2(maxHeight, maxHeight)));
    }

    private WhackButton CreateTarget(int position, Vector2 size)
    {
        var target = new WhackButton(_spriteSystem)
        {
            SetSize = size,
            Disabled = true,
            HorizontalExpand = true,
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Top,
        };

        target.OnButtonDown += _ => HitTarget(position);
        _targetContainer?.AddChild(target);
        return target;
    }

    private void HitTarget(int position)
    {
        if (!_targets.TryGetValue(position, out var target))
            return;

        target.HitTarget();
        OnPlayerAction?.Invoke(WhackGamePlayerAction.WhackTarget, position, target.TargetData);
    }

    private void UpdateTargets(Dictionary<int, WhackTarget> activeTargets)
    {
        var newTargets = activeTargets.Where(t => !(_lastTargets.TryGetValue(t.Key, out var v)
            && t.Value.ID == v.ID))
            .ToDictionary();

        for (int i = 0; i < _targets.Count; i++)
        {
            activeTargets.TryGetValue(i, out var targetData);
            var target = _targets[i];
            var isNew = newTargets.TryGetValue(i, out var _);

            if (target.TargetData != targetData)
                UpdateTarget(target, targetData, isNew);
        }

        _lastTargets = activeTargets;
    }

    private void UpdateTarget(WhackButton targetButton, WhackTarget? targetData, bool isNew = false)
    {
        if (targetButton.TargetData == targetData)
            return;

        targetButton.SetTarget(targetData, isNew);
    }
}
