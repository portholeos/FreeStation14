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
        private readonly TimeSpan _hitDuration = TimeSpan.FromSeconds(0.7f);
        private readonly TimeSpan _scoreDuration = TimeSpan.FromSeconds(1.2f);
        private const string ScoreFontPath = "/Fonts/NotoSans/NotoSans-Bold.ttf";
        private const int ScoreFontSize = 16;

        private TextureRect _targetTexture;
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

            Label.Align = Label.AlignMode.Center;
            Label.VAlign = Label.VAlignMode.Top;
            Label.FontOverride = font;
            Label.FontColorOverride = Color.Yellow;
            Label.RemoveStyleClass(StyleClassButton);

            AddChild(_targetTexture);
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
                Label.Visible = false;
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

            Logger.GetSawmill("WhackButton")
                .Debug($"Button: {GetHashCode()} Hit: {Hit} SpriteToUse: {spriteToUse.GetHashCode()} Time: {_gameTiming.CurTime}");
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
