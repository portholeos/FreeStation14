namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Access(typeof(SharedWhackGameArcadeSystem), Friend = AccessPermissions.Read)]
public abstract partial class SharedWhackGameArcadeComponent : Component
{
    /// <summary>
    ///     List of all possible targets that can spawn.
    /// </summary>
    [DataField(required: true)]
    public List<WhackTarget> Targets = new();
}
