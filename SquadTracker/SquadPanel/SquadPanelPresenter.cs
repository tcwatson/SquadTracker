using Blish_HUD.Graphics.UI;
using System.Linq;
using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, object>
    {
        private readonly PlayersManager _playersManager;
        private readonly SquadManager _squadManager;
        private readonly PlayerIconsManager _iconsManager;
        private readonly IEnumerable<Role> _roles;

        private readonly Squad _squad;

        public SquadPanelPresenter(
            SquadPanelView view,
            PlayersManager playersManager,
            SquadManager squadManager,
            PlayerIconsManager iconsManager,
            IEnumerable<Role> roles
        ) : base (view, null)
        {
            _playersManager = playersManager;
            _squadManager = squadManager;
            _iconsManager = iconsManager;
            _roles = roles;

            _squad = _squadManager.GetSquad();
        }

        protected override void UpdateView()
        {
            foreach (var member in _squad.CurrentMembers)
            {
                AddPlayer(member, false);
            }

            foreach (var formerMember in _squad.FormerMembers)
            {
                AddPlayer(formerMember, false);
                View.MovePlayerToFormerMembers(formerMember.AccountName);
            }

            _squadManager.PlayerJoinedSquad += AddPlayer;
            _playersManager.CharacterChangedSpecialization += ChangeCharacterSpecialization;
            _squadManager.PlayerLeftSquad += RemovePlayer;
        }

        protected override void Unload()
        {
            // To allow for garbage collection.
            _squadManager.PlayerJoinedSquad -= AddPlayer;
            _playersManager.CharacterChangedSpecialization -= ChangeCharacterSpecialization;
            _squadManager.PlayerLeftSquad -= RemovePlayer;
        }

        private void AddPlayer(Player player, bool isReturning)
        {
            var character = player.CurrentCharacter;
            var icon = _iconsManager.GetSpecializationIcon(character.Profession, character.Specialization);

            if (isReturning)
            {
                View.MoveFormerPlayerBackToSquad(player, icon);
            }
            else
            {
                View.DisplayPlayer(player, icon, _roles);
            }
        }

        private void ChangeCharacterSpecialization(Character character)
        {
            var icon = _iconsManager.GetSpecializationIcon(character.Profession, character.Specialization);
            View.SetPlayerIcon(character.Player, icon);
        }

        private void RemovePlayer(string accountName)
        {
            var player = _squad.FormerMembers.FirstOrDefault(p => p.AccountName == accountName);
            if (player == null) return;

            View.MovePlayerToFormerMembers(accountName);
        }

        public void ClearFormerSquadMembers()
        {
            _squad.ClearFormerSquadMembers();
        }

        #region Test

        public void AddTestPlayer()
        {
            var character = new Character("Frodo", 2, 18);
            var player = new Player("test.1234", character);

            var icon = _iconsManager.GetSpecializationIcon(character.Profession, character.Specialization);
            View.DisplayPlayer(player, icon, _roles);
        }

        public void RemoveTestPlayer()
        {
            View.MovePlayerToFormerMembers("test.1234");
        }

        #endregion
    }
}
