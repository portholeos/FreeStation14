
using Content.Client.FREE.Arcade.Games;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.UserInterface;

namespace Content.Client.FREE.Arcade.UI;

public sealed class WhackGameBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entity = default!;

    private WhackGameMenu? _menu;

    public WhackGameBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        SendAction(WhackGamePlayerAction.RequestData);
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<WhackGameMenu>();

        if (_entity.TryGetComponent<WhackGameArcadeComponent>(Owner, out var game))
            _menu.TargetCount = game.TargetCount;

        _menu.OnPlayerAction += SendAction;
        SendAction(WhackGamePlayerAction.RequestData);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is WhackGameArcadeGameUIState arcadeState)
            _menu?.UpdateState(arcadeState);
    }

    private void SendAction(WhackGamePlayerAction action, int? target = null)
    {
        var actionMessage = new WhackGameArcadePlayerActionMessage(action, target);
        SendMessage(actionMessage);
    }
}
