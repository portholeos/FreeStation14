using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.FREE.Arcade.Games.WhackGame;

[Serializable, NetSerializable]
public record WhackTarget
{
    /// <summary>
    ///     The sprite this target uses.
    /// </summary>
    [DataField]
    public required SpriteSpecifier Sprite;

    /// <summary>
    ///     How many points this target gives (or takes away) when you hit it.
    /// </summary>
    [DataField]
    public required int Score;

    /// <summary>
    ///     The sprite this target uses when it is hit.
    /// </summary>
    [DataField]
    public SpriteSpecifier? HitSprite;

    /// <summary>
    ///     Whether this target is considered a "friend" (i.e. you should not hit it.)
    /// </summary>
    [DataField]
    public bool Friendly = false;
}
