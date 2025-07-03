using Robust.Client.UserInterface.Controls;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    private readonly LocId _scoreText = "whackgame-menu-score";
    private readonly LocId _timeText = "whackgame-menu-time";
    private BoxContainer? _labelContainer;
    private Label? _scoreLabel;
    private Label? _timeLabel;

    private void PopulateLabels()
    {
        if (_labelContainer == null)
        {
            _labelContainer = new() { HorizontalExpand = true };
            AddChild(_labelContainer);
        }

        _labelContainer.RemoveAllChildren();

        if (_scoreLabel == null)
        {
            _scoreLabel = CreateLabel();
            UpdateScore(0);
        }

        if (_timeLabel == null)
        {
            _timeLabel = CreateLabel();
            UpdateTime(0);
        }
    }

    private Label CreateLabel()
    {
        var panel = new Label() { HorizontalExpand = true, };
        _labelContainer?.AddChild(panel);
        return panel;
    }

    private void UpdateScore(int score)
    {
        if (_scoreLabel != null)
            _scoreLabel.Text = Loc.GetString(_scoreText, ("score", score));

        if (_finalScoreLabel != null)
            _finalScoreLabel.Text = Loc.GetString(_finalScoreValue, ("score", score));
    }

    private void UpdateTime(int time)
    {
        if (_timeLabel == null)
            return;

        _timeLabel.Text = Loc.GetString(_timeText, ("time", time));
    }
}
