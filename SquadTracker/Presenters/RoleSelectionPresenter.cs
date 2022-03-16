using Blish_HUD.Graphics.UI;
using System.Collections.ObjectModel;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Views;

namespace Torlando.SquadTracker.Presenters
{
    class RoleSelectionPresenter : Presenter<RoleSelectionView, ObservableCollection<Role>>
    {
        public RoleSelectionPresenter(RoleSelectionView view, ObservableCollection<Role> model) : base(view, model) { }

        //public View<> GetDropdown()
        //{
        //    return new RoleSelectionView();
        //}
    }
}
