using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Torlando.SquadTracker.Models;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, Squad>
    {
        private readonly ContentsManager _contentsManager;

        public SquadPanelPresenter(SquadPanelView view, Squad model, ContentsManager contentsManager) : base (view, model) 
        {
            _contentsManager = contentsManager;
        }
        
        public void ClearFormerSquadMembers()
        {
            Model.ClearFormerSquadMembers();
        }


        #region Test
        public void AddPlayer()
        {
            var player = new PlayerModel
            {
                AccountName = "test.1234",
                CharacterName = "Frodo"
            };
            Model.AddPlayer(player);
            View.SpawnPlayerView(player);
        }
        #endregion
    }
}
