using Blish_HUD.Graphics.UI;
using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, Squad>
    {
        private readonly PlayerIconsManager _iconsManager;
        private readonly IEnumerable<Role> _roles;

        public SquadPanelPresenter(
            SquadPanelView view, 
            Squad model, 
            PlayerIconsManager iconsManager,
            IEnumerable<Role> roles
        ) : base (view, model) 
        {
            _iconsManager = iconsManager;
            _roles = roles;
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
                CharacterName = "Frodo",
                Profession = 2,
                CurrentSpecialization = 18
            };
            var icon = _iconsManager.GetSpecializationIcon(player.Profession, player.CurrentSpecialization);
            Model.AddPlayer(player);
            View.SpawnPlayerButton(player, icon, _roles);
        }

        public void RemovePlayer()
        {
            View.RemovePlayerFromSquad("test.1234");
        }
        #endregion
    }
}
