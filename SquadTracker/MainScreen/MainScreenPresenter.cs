using System.Collections.Generic;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Torlando.SquadTracker.RolesScreen;
using Torlando.SquadTracker.SquadPanel;

namespace Torlando.SquadTracker.MainScreen
{
    internal class MainScreenPresenter : Presenter<MainScreenView, int>
    {

        private SquadPanelPresenter _squadPanelPresenter;
        private SquadPanelView _squadPanelView;

        private RolesPresenter _rolesPresenter;
        private RolesView _rolesView;

        public MainScreenPresenter(MainScreenView view, ContentsManager contentsManager, SettingEntry<bool> areColorIconsEnabled, ICollection<Role> roles) : base (view, 0)
        {
            _squadPanelView = new SquadPanelView();
            _squadPanelPresenter = new SquadPanelPresenter(_squadPanelView, new Squad(), contentsManager, areColorIconsEnabled);

            _rolesView = new RolesView();
            _rolesPresenter = new RolesPresenter(_rolesView, roles);
        }

        public IView SelectView(string name)
        {
            return name switch
            {
                "Squad Members" => _squadPanelView.WithPresenter(_squadPanelPresenter),
                "Squad Roles" => _rolesView.WithPresenter(_rolesPresenter),
                _ => _squadPanelView.WithPresenter(_squadPanelPresenter),
            };
        }
        
    }
}
