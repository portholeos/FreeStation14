using Robust.Shared.GameStates;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Access(typeof(SharedWhackGameArcadeSystem), Friend = AccessPermissions.Read)]
[NetworkedComponent, AutoGenerateComponentState]
public abstract partial class SharedWhackGameArcadeComponent : Component
{
    /// <summary>
    ///     How many possible target positions there are to hit.
    /// </summary>
    [AutoNetworkedField, DataField]
    public int TargetCount = 6;
}
