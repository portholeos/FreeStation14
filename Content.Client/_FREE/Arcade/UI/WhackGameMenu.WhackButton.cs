using System.Numerics;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.FREE.Arcade.Games.WhackGame;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.FREE.Arcade.UI;

public sealed partial class WhackGameMenu
{
    private sealed class WhackButton : Button
    {
        private readonly IGameTiming _gameTiming;
        private readonly SpriteSystem _spriteSystem;
        private readonly LocId _scoreText = "whackgame-menu-score-indicator";
        private readonly TimeSpan _hitDuration = TimeSpan.FromSeconds(0.8f);
        private readonly TimeSpan _scoreDuration = TimeSpan.FromSeconds(1.2f);
        private const string ScoreFontPath = "/Fonts/NotoSans/NotoSans-Bold.ttf";
        private const int ScoreFontSize = 16;

        private TextureRect _targetTexture;
        private TimeSpan? _hitEndTime;
        private TimeSpan? _scoreEndTime;
        private SpriteSpecifier? _fallbackSprite;

        public WhackTarget? TargetData = null;
        public bool Hit = false;

        public WhackButton(SpriteSystem spriteSystem, SpriteSpecifier? fallbackSprite = null) : base()
        {
            _gameTiming = IoCManager.Resolve<IGameTiming>();
            _spriteSystem = spriteSystem;
            _fallbackSprite = fallbackSprite;
            var resourceCache = IoCManager.Resolve<IResourceCache>();
            var font = resourceCache.GetFont(ScoreFontPath, ScoreFontSize);

            _targetTexture = new TextureRect()
            {
                HorizontalExpand = true,
                VerticalExpand = true,
            };

            if (_fallbackSprite != null)
                _targetTexture.Texture = _spriteSystem.Frame0(_fallbackSprite);

            Label.Align = Label.AlignMode.Center;
            Label.VAlign = Label.VAlignMode.Top;
            Label.FontOverride = font;
            Label.FontColorOverride = Color.Yellow;
            Label.RemoveStyleClass(StyleClassButton);
            Label.Orphan();

            AddChild(_targetTexture);
            AddChild(Label);

            RemoveStyleClass(StyleClassButton);
            UpdateAppearance();
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            base.FrameUpdate(args);

            if (Hit && _hitEndTime != null && _gameTiming.CurTime > _hitEndTime)
            {
                Hit = false;
                UpdateTexture();
            }

            if (_scoreEndTime != null && _gameTiming.CurTime > _scoreEndTime)
            {
                _scoreEndTime = null;
                UpdateScoreLabel();
            }
        }

        public void HitTarget()
        {
            Hit = true;
            _hitEndTime = _gameTiming.CurTime + _hitDuration;
            _scoreEndTime = _gameTiming.CurTime + _scoreDuration;

            UpdateButtonAppearance();
            UpdateTexture();
        }

        public void SetTarget(WhackTarget? targetData, bool isNew = false)
        {
            if (Hit && isNew)
                Hit = false;

            TargetData = targetData;
            UpdateAppearance();
        }

        public void UpdateAppearance()
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
            var spriteToUse = _fallbackSprite;
            if (TargetData != null)
            {
                if (Hit)
                    spriteToUse = TargetData.HitSprite ?? spriteToUse;
                else
                    spriteToUse = TargetData.Sprite;
            }

            // Note that if there is no hit sprite, the target falls back to base sprite.
            // This is intentional!
            if (spriteToUse == null)
            {
                _targetTexture.Texture = null;
                return;
            }

            var texture = _spriteSystem.Frame0(spriteToUse);
            var scale = MathF.Floor(Size.Y / texture.Width);
            _targetTexture.TextureScale = new Vector2(scale, scale);
            _targetTexture.Texture = texture;
        }

        /// <remarks>
        ///     The UX of this can be inconsistent or slow depending on server latency.
        ///     If it's really bad you might want to consider disabling validateOnServer in the
        ///     component.
        /// </remarks>
        private void UpdateScoreLabel()
        {
            Label.Visible = Hit && _scoreEndTime != null && _gameTiming.CurTime <= _scoreEndTime;
            if (TargetData == null)
                return;

            var text = Loc.GetString(_scoreText, ("score", TargetData.Score));

            Label.SetWidth = Size.X;
            Label.Margin = new Thickness(0, -Size.Y / 2, 0, 0);
            Label.Text = text;
        }
    }
}
