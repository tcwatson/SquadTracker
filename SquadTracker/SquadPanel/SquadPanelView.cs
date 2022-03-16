using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Views;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelView : View<SquadPanelPresenter>
    {
        #region Controls
        private Panel _menuPanel;
        private Menu _menuCategories;
        private MenuItem _squadMembersMenu;
        private MenuItem _squadRolesMenu;
        private FlowPanel _squadMembersPanel;
        private FlowPanel _formerSquadMembersPanel;
        private StandardButton _clearFormerSquadButton;

        #region Test
        private StandardButton _addPlayerButton;
        #endregion
        #endregion

        public SquadPanelView()
        {
        }

        protected override void Build(Container buildPanel)
        {
            _menuPanel = new Panel
            {
                Title = "Squad Tracker Menu",
                ShowBorder = true,
                Size = Panel.MenuStandard.Size,
                Parent = buildPanel
            };
            _menuCategories = new Menu
            {
                Size = _menuPanel.ContentRegion.Size,
                MenuItemHeight = 40,
                Parent = _menuPanel,
                CanSelect = true
            };
            _squadMembersMenu = _menuCategories.AddMenuItem("Squad Members");
            _squadMembersMenu.Select();

            _squadRolesMenu = _menuCategories.AddMenuItem("Squad Roles");
            _squadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = buildPanel,
                Location = new Point(_menuCategories.Right + 10, _menuCategories.Top),
                CanScroll = true,
                Size = new Point(buildPanel.Width - _menuCategories.Width - 5, 530), //
                Title = "Current Squad Members",
                ShowBorder = true
            };
            _formerSquadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = buildPanel,
                Location = new Point(_menuCategories.Right + 10, _squadMembersPanel.Bottom + 10),
                CanScroll = true,
                Size = new Point(buildPanel.Width - _menuCategories.Width - 5, 150),
                Title = "Former Squad Members",
                ShowBorder = true
            };
            _clearFormerSquadButton = new StandardButton
            {
                Parent = buildPanel,
                Text = "Clear",
                Location = new Point(_formerSquadMembersPanel.Right - 135, _formerSquadMembersPanel.Top + 5)
            };
            _clearFormerSquadButton.Click += delegate
            {
                Presenter.ClearFormerSquadMembers();
            };

            #region Test
            _addPlayerButton = new StandardButton
            {
                Parent = buildPanel,
                Text = "Add Player",
                Location = new Point(_squadMembersPanel.Right - 135, _squadMembersPanel.Top + 5)
            };
            _addPlayerButton.Click += delegate
            {
                Presenter.AddPlayer();
            };
            #endregion
        }

        public void SpawnPlayerView(PlayerModel playerModel)
        {
            var playerView = new PlayerView(playerModel);
            playerView.SetPlayerText(playerModel.AccountName);
        }
    }
}
