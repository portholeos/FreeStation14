using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;

    private readonly LocId _mainMenuTitle = "whackgame-menu-mainmenu-title";
    private readonly LocId _startButtonText = "whackgame-menu-mainmenu-start-game";
    private readonly LocId _finalScoreText = "whackgame-menu-gameover-score-text";
    private readonly LocId _finalScoreValue = "whackgame-menu-gameover-score-value";
    private readonly LocId _backToMenuText = "whackgame-menu-gameover-back-to-menu-button";
    private const string TitleFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";
    private const int TitleFontSize = 20;

    private PanelContainer? _menuPanel;
    private PanelContainer? _gameOverPanel;
    private Label? _finalScoreLabel;
    private WhackGameState _panelState = WhackGameState.MainMenu;

    private void PopulatePanels()
    {
        _menuPanel ??= CreateOverlayPanel();
        _gameOverPanel ??= CreateOverlayPanel();
        PopulateMenuPanel();
        PopulateGameOverPanel();
        UpdatePanels();
    }

    private void PopulateMenuPanel()
    {
        if (_menuPanel == null)
            return;

        _menuPanel.RemoveAllChildren();
        var titleMargin = new Thickness(0, 0, 0, 20);
        var font = _resourceCache.GetFont(TitleFontPath, TitleFontSize);
        var titleLabel = new Label()
        {
            Text = Loc.GetString(_mainMenuTitle),
            Margin = titleMargin,
            Align = Label.AlignMode.Center,
            FontOverride = font,
        };

        var startButton = new Button() { Text = Loc.GetString(_startButtonText), };
        startButton.OnPressed += _ => StartGame();

        var box = new BoxContainer()
        {
            HorizontalExpand = true,
            HorizontalAlignment = HAlignment.Center,
            Align = BoxContainer.AlignMode.Center,
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Children = { titleLabel, startButton }
        };

        _menuPanel.AddChild(box);
    }

    private void PopulateGameOverPanel()
    {
        if (_gameOverPanel == null)
            return;

        _gameOverPanel.RemoveAllChildren();
        var finalScoreTextLabel = new Label()
        {
            Text = Loc.GetString(_finalScoreText),
            Align = Label.AlignMode.Center,
        };

        var finalScoreValueLabel = new Label()
        {
            Align = Label.AlignMode.Center,
        };

        var backToMenu = new Button() { Text = Loc.GetString(_backToMenuText), };
        backToMenu.OnPressed += _ => SetPanelState(WhackGameState.MainMenu);

        var box = new BoxContainer()
        {
            HorizontalExpand = true,
            VerticalExpand = true,
            HorizontalAlignment = HAlignment.Center,
            Align = BoxContainer.AlignMode.Center,
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Children = { finalScoreTextLabel, finalScoreValueLabel, backToMenu }
        };

        _finalScoreLabel = finalScoreValueLabel;
        _gameOverPanel.AddChild(box);
    }

    private PanelContainer CreateOverlayPanel()
    {
        var margin = new Thickness(0f, _topBarHeight, 0f, 0f);
        var styleBox = new StyleBoxFlat();
        styleBox.BackgroundColor = Color.Black.WithAlpha(0.6f);

        var panel = new PanelContainer()
        {
            MouseFilter = MouseFilterMode.Stop,
            PanelOverride = styleBox,
            Margin = margin,
        };

        AddChild(panel);
        return panel;
    }

    private void UpdatePanels()
    {
        if (_menuPanel != null)
        {
            _menuPanel.Visible = _panelState == WhackGameState.MainMenu;
        }

        if (_gameOverPanel != null)
        {
            _gameOverPanel.Visible = _panelState == WhackGameState.GameOver;
        }
    }
}
