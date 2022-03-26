using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelView : View<SquadPanelPresenter>
    {
        #region Controls
        
        private FlowPanel _squadMembersPanel;
        private FlowPanel _formerSquadMembersPanel;
        private StandardButton _clearFormerSquadButton;
        private Dictionary<string, PlayerButton> _playerButtons = new Dictionary<string, PlayerButton>();

        #region Test
        private StandardButton _addPlayerButton;
        private StandardButton _removeButton;
        #endregion
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
                ShowBorder = true
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

            #region Test
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
            #endregion
        }

        public void DisplayPlayer(Player playerModel, AsyncTexture2D icon, IEnumerable<Role> roles)
        {
            var otherCharacters = playerModel.KnownCharacters.Except(new[] { playerModel.CurrentCharacter }).ToList();

            _playerButtons.Add(playerModel.AccountName, new PlayerButton(roles) 
            { 
                Parent = _squadMembersPanel,
                AccountName = playerModel.AccountName, 
                CharacterName = playerModel.CurrentCharacter.Name,
                Icon = icon,
                BasicTooltipText = OtherCharactersToString(otherCharacters),
            });
        }

        public void SetPlayerIcon(Player playerModel, AsyncTexture2D icon)
        {
            if (!_playerButtons.TryGetValue(playerModel.AccountName, out var button)) return;
            button.Icon = icon;
        }

        public void MovePlayerToFormerMembers(string accountName)
        {
            if (_playerButtons.TryGetValue(accountName, out var button))
            { 
                button.Parent = _formerSquadMembersPanel;
            }
        }

        public void MoveFormerPlayerBackToSquad(Player playerModel, AsyncTexture2D icon)
        {
            if (!_playerButtons.TryGetValue(playerModel.AccountName, out var button)) return;

            button.CharacterName = playerModel.CurrentCharacter.Name;
            button.Icon = icon;

            var otherCharacters = playerModel.KnownCharacters.Except(new[] { playerModel.CurrentCharacter }).ToList();
            button.BasicTooltipText = OtherCharactersToString(otherCharacters);

            button.Parent = _squadMembersPanel;
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
    }
}
