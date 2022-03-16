using Blish_HUD.Graphics.UI;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, Squad>
    {
        public SquadPanelPresenter(SquadPanelView view, Squad model) : base (view, model) { }
    }
}
