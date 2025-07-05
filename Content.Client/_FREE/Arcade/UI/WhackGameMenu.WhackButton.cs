using System.Numerics;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    private sealed class WhackButton : Button
    {
        private readonly IGameTiming _gameTiming;
        private readonly SpriteSystem _spriteSystem;
        private readonly LocId _scoreText = "whackgame-menu-score-indicator";
        private readonly TimeSpan _hitDuration = TimeSpan.FromSeconds(0.8f);
        private readonly TimeSpan _scoreDuration = TimeSpan.FromSeconds(2.0f);
        private const string ScoreFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";
        private const int ScoreFontSize = 16;

        private TextureRect _targetTexture;
        private Label _scoreLabel;
        private TimeSpan? _hitEndTime;
        private TimeSpan? _scoreEndTime;

        public WhackTarget? TargetData = null;
        public bool Hit = false;


        public WhackButton(SpriteSystem spriteSystem) : base()
        {
            _gameTiming = IoCManager.Resolve<IGameTiming>();
            _spriteSystem = spriteSystem;
            var resourceCache = IoCManager.Resolve<IResourceCache>();
            var font = resourceCache.GetFont(ScoreFontPath, ScoreFontSize);

            _targetTexture = new TextureRect()
            {
                HorizontalExpand = true,
                VerticalExpand = true,
            };

            _scoreLabel = new Label()
            {
                Align = Label.AlignMode.Center,
                VAlign = Label.VAlignMode.Top,
                FontOverride = font,
            };

            AddChild(_targetTexture);
            AddChild(_scoreLabel);
            RemoveStyleClass(StyleClassButton);
            UpdateAppearance();
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            base.FrameUpdate(args);

            if (Hit && _hitEndTime != null && _gameTiming.CurTime > _hitEndTime)
            {
                Hit = false;
                _targetTexture.Texture = null;
            }

            if (_scoreEndTime != null && _gameTiming.CurTime > _scoreEndTime)
            {
                _scoreEndTime = null;
                _scoreLabel.Visible = false;
            }
        }

        public void HitTarget()
        {
            Hit = true;
            UpdateAppearance();

            _hitEndTime = _gameTiming.CurTime + _hitDuration;
            _scoreEndTime = _gameTiming.CurTime + _scoreDuration;

            SetTarget(null);
        }

        public void SetTarget(WhackTarget? targetData)
        {
            if (targetData != null)
                Hit = false;

            TargetData = targetData;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            UpdateButtonAppearance();
            UpdateTexture();
            UpdateScoreLabel();
        }

        private void UpdateButtonAppearance()
        {
            Disabled = TargetData == null;

            if (TargetData?.Friendly == true)
                AddStyleClass(StyleNano.StyleClassButtonColorGreen);
            else
                RemoveStyleClass(StyleNano.StyleClassButtonColorGreen);
        }

        private void UpdateTexture()
        {
            if (TargetData == null)
            {
                if (!Hit)
                    _targetTexture.Texture = null;
                return;
            }

            var spriteToUse = TargetData.Sprite;
            if (Hit)
                spriteToUse = TargetData.HitSprite;

            // Note that if there is no hit sprite, the target becomes invisible.
            // This is intentional!
            if (spriteToUse == null)
                return;

            var texture = _spriteSystem.Frame0(spriteToUse);
            var scale = MathF.Floor(Size.Y / texture.Width);
            _targetTexture.TextureScale = new Vector2(scale, scale);
            _targetTexture.Texture = texture;
        }

        private void UpdateScoreLabel()
        {
            if (TargetData == null)
                return;

            var text = Loc.GetString(_scoreText, ("score", TargetData.Score));

            _scoreLabel.SetWidth = Size.X;
            _scoreLabel.Text = text;
            _scoreLabel.Visible = _scoreEndTime != null && _gameTiming.CurTime <= _scoreEndTime;
        }
    }
}
