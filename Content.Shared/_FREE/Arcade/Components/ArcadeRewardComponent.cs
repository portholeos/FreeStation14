using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Shared.FREE.Arcade.Components;

[RegisterComponent]
public sealed partial class ArcadeRewardComponent : Component
{
    /// <summary>
    /// Table that determines what gets spawned.
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector PossibleRewards = default!;

    /// <summary>
    /// The minimum number of prizes the arcade machine can have.
    /// </summary>
    [DataField("rewardMinAmount")]
    public int RewardMinAmount = 0;

    /// <summary>
    /// The maximum number of prizes the arcade machine can have.
    /// </summary>
    [DataField("rewardMaxAmount")]
    public int RewardMaxAmount = 0;

    /// <summary>
    /// The remaining number of prizes the arcade machine can dispense.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int RewardAmount = 0;
}
