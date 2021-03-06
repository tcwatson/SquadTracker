using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using Torlando.SquadTracker.RolesScreen;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelView : View<SquadPanelPresenter>
    {
        #region Controls
        
        private FlowPanel _squadMembersPanel;
        private FlowPanel _formerSquadMembersPanel;
        private StandardButton _clearFormerSquadButton;
        private Dictionary<string, PlayerDisplay> _playerDisplays = new Dictionary<string, PlayerDisplay>();

        #if DEBUG
        private StandardButton _addPlayerButton;
        private StandardButton _removeButton;
        #endif
        #endregion

        public SquadPanelView()
        {
        }

        protected override void Build(Container buildPanel)
        {
            _squadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = buildPanel,
                Location = new Point(buildPanel.ContentRegion.Left, buildPanel.ContentRegion.Top),
                CanScroll = true,
                Size = new Point(buildPanel.ContentRegion.Width, 530), //
                Title = "Current Squad Members",
                ShowBorder = true,
                BasicTooltipText = "You loaded Blish HUD after starting Guild Wars 2. Please change maps to refresh."
            };
            _formerSquadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = buildPanel,
                Location = new Point(buildPanel.ContentRegion.Left, _squadMembersPanel.Bottom + 10),
                CanScroll = true,
                Size = new Point(_squadMembersPanel.Width, 150),
                Title = "Former Squad Members",
                ShowBorder = true
            };
            _clearFormerSquadButton = new StandardButton
            {
                Parent = buildPanel,
                Text = "Clear",
                Location = new Point(_formerSquadMembersPanel.Right - 135, _formerSquadMembersPanel.Top + 5)
            };
            _clearFormerSquadButton.Click += delegate
            {
                Presenter.ClearFormerSquadMembers();
            };

#if DEBUG
            _addPlayerButton = new StandardButton
            {
                Parent = buildPanel,
                Text = "Add Player",
                Location = new Point(_squadMembersPanel.ContentRegion.Right - 135, _squadMembersPanel.Top + 5)
            };
            _addPlayerButton.Click += delegate
            {
                Presenter.AddTestPlayer();
            };

            _removeButton = new StandardButton
            {
                Parent = buildPanel,
                Text = "Remove",
                Location = new Point(_addPlayerButton.Location.X - 135, _squadMembersPanel.Top + 5)
            };
            _removeButton.Click += delegate
            {
                Presenter.RemoveTestPlayer();
            };
#endif
        }

        public void DisplayPlayer(Player playerModel, AsyncTexture2D icon, IEnumerable<Role> roles, List<string> assignedRoles)
        {
            var otherCharacters = playerModel.KnownCharacters.Except(new[] { playerModel.CurrentCharacter }).ToList();

            var playerDisplay = new PlayerDisplay(roles)
            {
                Parent = _squadMembersPanel,
                AccountName = playerModel.AccountName,
                CharacterName = playerModel.CurrentCharacter.Name,
                Icon = icon,
                BasicTooltipText = OtherCharactersToString(otherCharacters),
            };
 
            playerDisplay.Role1Dropdown.ValueChanged += (o, e) => UpdateSelectedRoles(playerModel, e, 0);
            playerDisplay.Role2Dropdown.ValueChanged += (o, e) => UpdateSelectedRoles(playerModel, e, 1);

            playerDisplay.Role1Dropdown.SelectedItem = assignedRoles[0];
            playerDisplay.Role2Dropdown.SelectedItem = assignedRoles[1];
            _playerDisplays.Add(playerModel.AccountName, playerDisplay);

            _squadMembersPanel.BasicTooltipText = "";
        }

        private void UpdateSelectedRoles(Player playerModel, ValueChangedEventArgs e, int index)
        {
            var role = e.CurrentValue;
            var accountName = playerModel.AccountName;
            Presenter.UpdateSelectedRoles(accountName, role, index);
        }

        public void SetPlayerIcon(Player playerModel, AsyncTexture2D icon)
        {
            if (!_playerDisplays.TryGetValue(playerModel.AccountName, out var display)) return;
            display.Icon = icon;
        }

        public void MovePlayerToFormerMembers(string accountName)
        {
            if (_playerDisplays.TryGetValue(accountName, out var display))
            { 
                display.Parent = _formerSquadMembersPanel;
            }
        }

        public void MoveFormerPlayerBackToSquad(Player playerModel, AsyncTexture2D icon)
        {
            if (!_playerDisplays.TryGetValue(playerModel.AccountName, out var display)) return;

            display.CharacterName = playerModel.CurrentCharacter.Name;
            display.Icon = icon;

            var otherCharacters = playerModel.KnownCharacters.Except(new[] { playerModel.CurrentCharacter }).ToList();
            display.BasicTooltipText = OtherCharactersToString(otherCharacters);

            display.Parent = _squadMembersPanel;
        }

        private static string OtherCharactersToString(IReadOnlyCollection<Character> characters)
        {
            if (characters.Count == 0) return string.Empty;

            var charactersList = string.Join("\n",
                characters
                    .OrderBy(character => character.Name)
                    .Select(character =>
                        $"- {character.Name} ({Specialization.GetEliteName(character.Specialization, character.Profession)})"
                    )
            );

            return $"Other characters:\n{charactersList}";
        }

        public void RemoveFormerMember(string accountName)
        {
            if (!_playerDisplays.TryGetValue(accountName, out var display)) return;

            _playerDisplays.Remove(accountName);
            display.Parent = null;
        }
    }
}
