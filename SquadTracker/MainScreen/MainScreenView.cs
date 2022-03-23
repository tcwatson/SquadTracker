using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;

namespace Torlando.SquadTracker.MainScreen
{
    internal class MainScreenView : View<MainScreenPresenter>
    {
        #region Controls
        private ViewContainer _menuPanel;
        private Menu _menuCategories;
        private MenuItem _squadMembersMenu;
        private MenuItem _squadRolesMenu;
        private ViewContainer _viewContainer;

        #region Test
        private StandardButton _addPlayerButton;
        private StandardButton _removeButton;
        #endregion
        #endregion

        public MainScreenView()
        {
        }

        protected override void Build(Container buildPanel)
        {
            _menuPanel = new ViewContainer
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
            _viewContainer = new ViewContainer
            {
                Parent = buildPanel,
                Location = new Point(_menuCategories.Right + 10, _menuCategories.Top),
                Width = buildPanel.ContentRegion.Width - _menuPanel.Width - 10,
                Height = buildPanel.ContentRegion.Height
            };
            _squadMembersMenu = _menuCategories.AddMenuItem("Squad Members");
            _squadMembersMenu.ItemSelected += (o, e) => ShowView("Squad Members");
            _squadMembersMenu.Select();

            _squadRolesMenu = _menuCategories.AddMenuItem("Squad Roles");
            _squadRolesMenu.ItemSelected += (o, e) => ShowView("Squad Roles");
        }

        private void ShowView(string viewName)
        {
            _viewContainer.Show(Presenter.SelectView(viewName));
        }
    }
}
