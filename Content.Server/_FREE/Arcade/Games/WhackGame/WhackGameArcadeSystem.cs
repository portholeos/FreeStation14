using Content.Server.FREE.Arcade.Systems;
using Content.Server.Power.Components;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Content.Shared.Power;
using Robust.Server.GameObjects;

namespace Content.Server.FREE.Arcade.Games.WhackGame;

public sealed partial class WhackGameArcadeSystem : SharedWhackGameArcadeSystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly ArcadeSystem _arcade = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WhackGameArcadeComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<WhackGameArcadeComponent, WhackGameArcadePlayerActionMessage>(OnPlayerAction);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<WhackGameArcadeComponent>();
        var gamesToEnd = new List<Entity<WhackGameArcadeComponent>>();

        while (query.MoveNext(out var eid, out var comp))
        {
            var keepGoing = comp.Game?.Update(frameTime);

            if (keepGoing == false)
                gamesToEnd.Add(new Entity<WhackGameArcadeComponent>(eid, comp));
        }

        foreach (var ent in gamesToEnd)
            EndGame(ent);
    }

    private void EndGame(Entity<WhackGameArcadeComponent> ent)
    {
        var game = ent.Comp.Game;
        if (game == null)
            return;

        game.EndGame();
        var performance = game.GetPerformance();

        if (performance >= 0.7f) // C's get degrees
            _arcade.WinGame(null, ent.Owner);

        game.UpdateUI(true);
        ent.Comp.Game = null;
    }

    private void OnPlayerAction(Entity<WhackGameArcadeComponent> ent, ref WhackGameArcadePlayerActionMessage args)
    {
        if (!IsPowered(ent))
            return;

        switch (args.Action)
        {
            case WhackGamePlayerAction.StartGame:
                {
                    if (ent.Comp.Game != null)
                        break;

                    var newGame = new WhackGame(ent.Comp);
                    ent.Comp.Game = newGame;
                    newGame.OnUIUpdate += s => UpdateState(ent, s);
                    newGame.UpdateUI();
                    break;
                }
            case WhackGamePlayerAction.WhackTarget:
                {
                    if (ent.Comp.Game == null
                        || args.Target == null
                        || args.Target < 0
                        || args.Target >= ent.Comp.TargetCount)
                        break;

                    ent.Comp.Game.HitTarget(args.Target.Value);
                    break;
                }
            case WhackGamePlayerAction.RequestData:
                {
                    ent.Comp.Game?.UpdateUI();
                    break;
                }
        }
    }

    private void OnPowerChanged(Entity<WhackGameArcadeComponent> ent, ref PowerChangedEvent args)
    {
        if (IsPowered(ent))
            return;

        _uiSystem.CloseUi(ent.Owner, WhackGameArcadeUiKey.Key);
    }

    private void UpdateState(Entity<WhackGameArcadeComponent> ent, WhackGameArcadeGameUIState state)
    {
        if (!TryComp<UserInterfaceComponent>(ent.Owner, out var uiState))
            return;

        var uiEnt = new Entity<UserInterfaceComponent?>(ent.Owner, uiState);
        _uiSystem.SetUiState(uiEnt, WhackGameArcadeUiKey.Key, state);
    }

    private bool IsPowered(Entity<WhackGameArcadeComponent> ent)
    {
        return !TryComp<ApcPowerReceiverComponent>(ent.Owner, out var power)
            || power.Powered;
    }
}
