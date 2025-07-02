using Robust.Shared.Serialization;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Serializable, NetSerializable]
public sealed class WhackGameArcadePlayerActionMessage(WhackGamePlayerAction action,
    int? target = null) : BoundUserInterfaceMessage
{
    public readonly WhackGamePlayerAction Action = action;
    public readonly int? Target = target;
}

[Serializable, NetSerializable]
public enum WhackGameArcadeUiKey
{
    Key,
}

[Serializable, NetSerializable]
public enum WhackGamePlayerAction
{
    WhackTarget,
    StartGame,
    ReturnToMenu,
}

[Serializable, NetSerializable]
public enum WhackGameState
{
    MainMenu,
    Game,
    GameOver,
}

[Serializable, NetSerializable, Virtual]
public class WhackGameArcadeUpdateMessage : BoundUserInterfaceMessage
{
}
