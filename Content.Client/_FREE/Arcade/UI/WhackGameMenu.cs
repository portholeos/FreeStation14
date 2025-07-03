using System.Numerics;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu : DefaultWindow
{
    private readonly Vector2 _windowSize = new Vector2(450, 300);
    private readonly LocId _windowTitle = "whackgame-menu-title";
    public event Action<WhackGamePlayerAction, int?>? OnPlayerAction;

    public int TargetCount = 0;

    public WhackGameMenu()
    {
        InitializeWindowProperties();
    }

    private void InitializeWindowProperties()
    {
        Title = Loc.GetString(_windowTitle);
        MinSize = SetSize = _windowSize;
    }

    public void PopulateWindow()
    {
        PopulatePanels();
        PopulateLabels();
        PopulateTargets();
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

        OnPlayerAction?.Invoke(WhackGamePlayerAction.StartGame, null);
    }
}

public enum WhackGameState
{
    MainMenu,
    Game,
    GameOver,
}
