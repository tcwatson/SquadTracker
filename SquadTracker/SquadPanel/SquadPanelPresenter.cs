using Blish_HUD.Graphics.UI;
using System.Linq;
using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, Squad>
    {
        private readonly PlayersManager _playersManager;
        private readonly PlayerIconsManager _iconsManager;
        private readonly IEnumerable<Role> _roles;

        public SquadPanelPresenter(
            SquadPanelView view, 
            Squad model,
            PlayersManager playersManager,
            PlayerIconsManager iconsManager,
            IEnumerable<Role> roles
        ) : base (view, model) 
        {
            _playersManager = playersManager;
            _iconsManager = iconsManager;
            _roles = roles;
        }

        protected override void UpdateView()
        {
            var players = _playersManager.GetPlayers();
            foreach (var player in players.Where(p => p.IsInSquad))
            {
                AddPlayer(player);
            }

            _playersManager.PlayerJoinedInstance += AddPlayer;
            _playersManager.PlayerLeftInstance += RemovePlayer;
        }

        protected override void Unload()
        {
            // To allow for garbage collection.
            _playersManager.PlayerJoinedInstance -= AddPlayer;
            _playersManager.PlayerLeftInstance -= RemovePlayer;
        }

        private void AddPlayer(Player player)
        {
            var character = player.CurrentCharacter;
            var icon = _iconsManager.GetSpecializationIcon(character.Profession, character.Specialization);

            if (Model.FormerSquadMembers.Contains(player))
            {
                Model.CurrentSquadMembers.Add(player);
                Model.FormerSquadMembers.Remove(player);

                View.MoveFormerPlayerBackToSquad(player, icon);
            }
            else
            {
                Model.CurrentSquadMembers.Add(player);

                View.DisplayPlayer(player, icon, _roles);
            }
        }

        private void RemovePlayer(string accountName)
        {
            var player = Model.CurrentSquadMembers.FirstOrDefault(p => p.AccountName == accountName);
            if (player == null) return;

            Model.CurrentSquadMembers.Remove(player);
            Model.FormerSquadMembers.Add(player);

            View.MovePlayerToFormerMembers(accountName);
        }

        public void ClearFormerSquadMembers()
        {
            Model.ClearFormerSquadMembers();
        }

        #region Test

        public void AddTestPlayer()
        {
            var character = new Character("Frodo", 2, 18);
            var player = new Player("test.1234", character);

            var icon = _iconsManager.GetSpecializationIcon(character.Profession, character.Specialization);
            Model.CurrentSquadMembers.Add(player);
            View.DisplayPlayer(player, icon, _roles);
        }

        public void RemoveTestPlayer()
        {
            View.MovePlayerToFormerMembers("test.1234");
        }

        #endregion
    }
}
