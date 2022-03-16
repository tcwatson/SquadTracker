using System;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Presenters;

namespace Torlando.SquadTracker.Views
{
    class PlayerView : View<PlayerPresenter>
    {
        #region Controls
        private DetailsButton _detailsButton;

        #endregion

        public PlayerView(PlayerModel player)
        {
            Presenter = new PlayerPresenter(this, player);
        }

        protected override void Build(Container buildPanel)
        {
            _detailsButton = new DetailsButton
            {
                Parent = buildPanel,
                //Text = $"{_player.CharacterName} ({_player.AccountName})",
                IconSize = DetailsIconSize.Small,
                ShowVignette = true,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                //Icon = _iconGetter(_player.Profession, _player.CurrentSpecialization),
                Size = new Point(354, 90)
            };
            //_ = new RoleSelectionView(buildPanel);

            //base.Build(buildPanel); // do we need base.Build() ?
        }

        public void SetPlayerText(string playerText)
        {
            _detailsButton.Text = playerText;
        }

        public void SetPlayerIcon(AsyncTexture2D icon)
        {
            _detailsButton.Icon = icon;
        }
    }
}