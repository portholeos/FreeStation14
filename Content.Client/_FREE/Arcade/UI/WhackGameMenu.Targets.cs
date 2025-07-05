using Content.Client.Stylesheets;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    private readonly int _targetsPerRow = 3;

    private GridContainer? _targetContainer;
    private List<WhackButton> _targets = new();

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

        for (int i = 0; i < TargetCount; i++)
            _targets.Add(CreateTarget(i));
    }

    private WhackButton CreateTarget(int position)
    {
        var target = new WhackButton()
        {
            MinWidth = _windowSize.X / _targetsPerRow - 10,
            MinHeight = 80,
            Disabled = true,
            HorizontalExpand = true
        };

        target.OnButtonDown += _ => HitTarget(position);
        _targetContainer?.AddChild(target);
        return target;
    }

    private void HitTarget(int position)
    {
        if (!_targets.TryGetValue(position, out var target))
            return;

        UpdateTarget(target, null);
        OnPlayerAction?.Invoke(WhackGamePlayerAction.WhackTarget, position, target.TargetData);
    }

    private void UpdateTargets(Dictionary<int, WhackTarget> activeTargets)
    {
        for (int i = 0; i < _targets.Count; i++)
        {
            activeTargets.TryGetValue(i, out var targetData);
            var target = _targets[i];
            UpdateTarget(target, targetData);
        }
    }

    private void UpdateTarget(WhackButton targetButton, WhackTarget? targetData)
    {
        var newValue = targetData == null;
        if (newValue == targetButton.Disabled)
            return;

        targetButton.Disabled = newValue;

        if (targetData?.Friendly == true)
            targetButton.AddStyleClass(StyleNano.StyleClassButtonColorGreen);
        else
            targetButton.RemoveStyleClass(StyleNano.StyleClassButtonColorGreen);
    }

    private sealed class WhackButton : Button
    {
        public WhackTarget? TargetData = null;

        public WhackButton() : base() { }
    }
}
