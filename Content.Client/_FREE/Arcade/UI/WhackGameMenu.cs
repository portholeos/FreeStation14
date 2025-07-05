using System.Numerics;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu : DefaultWindow
{
    private readonly Vector2 _windowSize = new Vector2(450, 300);
    private readonly LocId _windowTitle = "whackgame-menu-title";
    private BoxContainer? _gameBox;
    private float _topBarHeight;
    public event Action<WhackGamePlayerAction, int?, WhackTarget?>? OnPlayerAction;

    public int TargetCount = 0;

    public WhackGameMenu()
    {
        IoCManager.InjectDependencies(this);
        InitializeWindowProperties();
    }

    private void InitializeWindowProperties()
    {
        Title = Loc.GetString(_windowTitle);
        MinSize = SetSize = _windowSize;
        _topBarHeight = WindowHeader.MinSize.Y;
    }

    public void PopulateWindow()
    {
        if (_gameBox == null)
            CreateGameBox();

        PopulateLabels();
        PopulateTargets();
        PopulatePanels();
    }

    private void CreateGameBox()
    {
        var margin = new Thickness(0f, _topBarHeight, 0f, 0f);
        var box = new BoxContainer()
        {
            HorizontalExpand = true,
            VerticalExpand = true,
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Align = BoxContainer.AlignMode.Center,
            HorizontalAlignment = HAlignment.Center,
            Margin = margin,
        };

        _gameBox = box;
        AddChild(box);
    }

    public void UpdateState(WhackGameArcadeGameUIState state)
    {
        SetPanelState(WhackGameState.Game);
        UpdateScore(state.Score);
        UpdateTime(state.TimeLeft);
        UpdateTargets(state.ActiveTargets);

        if (state.EndGame)
            SetPanelState(WhackGameState.GameOver);
    }

    private void SetPanelState(WhackGameState newState)
    {
        if (_panelState == newState)
            return;

        _panelState = newState;
        UpdatePanels();
    }

    private void StartGame()
    {
        if (_panelState == WhackGameState.Game)
            return;

        OnPlayerAction?.Invoke(WhackGamePlayerAction.StartGame, null, null);
    }
}

public enum WhackGameState
{
    MainMenu,
    Game,
    GameOver,
}
