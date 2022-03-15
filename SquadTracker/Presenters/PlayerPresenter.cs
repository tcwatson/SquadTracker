using Blish_HUD.Graphics.UI;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Views;

namespace Torlando.SquadTracker.Presenters
{
    internal class PlayerPresenter : Presenter<PlayerView, Player>
    {
        public PlayerPresenter(PlayerView view, Player model) : base(view, model) { }
    }
}
