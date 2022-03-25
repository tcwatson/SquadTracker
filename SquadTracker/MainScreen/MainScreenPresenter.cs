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
        private readonly PlayerIconsManager _iconsManager;
        private readonly ICollection<Role> _roles;

        public MainScreenPresenter(MainScreenView view, PlayerIconsManager iconsManager, ICollection<Role> roles) : base (view, 0)
        {
            _iconsManager = iconsManager;
            _roles = roles;
        }

        public IView SelectView(string name)
        {
            return name switch
            {
                "Squad Members" => this.CreateSquadView(),
                "Squad Roles" => this.CreateRolesView(),
                _ => this.CreateSquadView(),
            };
        }

        private IView CreateSquadView()
        {
            var view = new SquadPanelView();
            var presenter = new SquadPanelPresenter(view, new Squad(), _iconsManager, _roles);
            return view.WithPresenter(presenter);
        }

        private IView CreateRolesView()
        {
            var view = new RolesView();
            var presenter = new RolesPresenter(view, _roles);
            return view.WithPresenter(presenter);
        }
    }
}
