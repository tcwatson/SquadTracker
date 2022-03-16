using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelView : View
    {
        #region Controls
        private Panel _menuPanel;
        private Menu _menuCategories;
        private MenuItem _squadMembersMenu;
        private MenuItem _squadRolesMenu;
        #endregion
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
        }
    }
}
