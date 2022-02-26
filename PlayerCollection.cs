using Blish_HUD.ArcDps.Common;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Torlando.SquadTracker
{
    public class PlayerCollection
    {
        private ObservableCollection<PlayerDisplay> _playerDisplays;
        private IDictionary<string, Player> _players;
        private ConcurrentDictionary<string, CommonFields.Player> _arcPlayersInSquad;

        private Panel _activePlayerPanel;
        private Panel _formerPlayerPanel;

        public PlayerCollection(ConcurrentDictionary<string, CommonFields.Player> arcPlayersInSquad, Panel activePlayerPanel, Panel formerPlayerPanel)
        {
            _playerDisplays = new ObservableCollection<PlayerDisplay>();
            _players = new ConcurrentDictionary<string, Player>();
            _arcPlayersInSquad = arcPlayersInSquad;
            _activePlayerPanel = activePlayerPanel;
            _formerPlayerPanel = formerPlayerPanel;
        }

        public void AddPlayer(CommonFields.Player arcPlayer, Func<uint, uint, AsyncTexture2D> iconGetter, ObservableCollection<string> availableRoles)
        {
            if (_players.TryGetValue(arcPlayer.AccountName, out var existingPlayer))
            {
                var playerDisplay = GetPlayer(arcPlayer);

                // Move from former players if player rejoined
                if (playerDisplay?.IsFormerSquadMember ?? false)
                {
                    playerDisplay.MoveFormerSquadMemberToActivePanel();
                }
                if (arcPlayer.CharacterName != existingPlayer.CharacterName)
                {
                    var newCharacter = new Player(arcPlayer, existingPlayer);
                    playerDisplay.UpdateCharacter(newCharacter);
                    _players[arcPlayer.AccountName] = newCharacter;
                }
                //Don't add duplicate player
                return;

            }

            var player = new Player(arcPlayer);
            _players.Add(player.AccountName, player);
            _playerDisplays.Add(new PlayerDisplay(_activePlayerPanel, _formerPlayerPanel, player, iconGetter, availableRoles));
        }

        public void UpdatePlayerSpecialization(string characterName, uint newSpec)
        {
            Player player = null;
            var hasPlayer = _players.Values.ToList().Any(x => x.TryGetPlayedCharacterByName(characterName, out player));
            if (!hasPlayer) return;
            if (player?.CurrentSpecialization == newSpec) return;

            player.CurrentSpecialization = newSpec;
        }

        public void RemovePlayerFromActivePanel(CommonFields.Player arcPlayer)
        {
            //if (arcPlayer.Self && !_players.Any(x => x.IsSelf)) return; //Don't remove yourself, unless you changed characters
            var playerToRemove = GetPlayer(arcPlayer);
            playerToRemove.RemovePlayerFromActivePanel();
        }

        public void ClearFormerPlayers()
        {
            foreach (var player in _playerDisplays)
            {
                if (player.IsFormerSquadMember)
                    player.DisposeDetailsButton(); //todo - test this
            }
        }

        private CommonFields.Player GetArcPlayer(string characterName)
        {
            return _arcPlayersInSquad.First(x => x.Value.CharacterName.Equals(characterName)).Value;
        }

        private PlayerDisplay GetPlayer(CommonFields.Player arcPlayer)
        {
            return _playerDisplays.First(x => x.AccountName.Equals(arcPlayer.AccountName));
        }
    }

    public class PlayerDisplay
    {
        #region Data
        private Player _player;
        private ObservableCollection<string> _availableRoles;
        #endregion

        #region UI
        private DetailsButton _detailsButton;
        private Dropdown _dropdown1;
        private Dropdown _dropdown2;
        private Panel _activePlayerPanel;
        private Panel _formerPlayerPanel;
        private Func<uint, uint, AsyncTexture2D> _iconGetter;
        private const string _placeholderRoleName = "Select a role...";
        #endregion

        public string CharacterName => _player.CharacterName;
        public bool IsFormerSquadMember => _detailsButton.Parent?.Equals(_formerPlayerPanel) ?? false;
        public bool HasChangedCharacters => _player.HasChangedCharacters;
        public bool IsSelf => _player.IsSelf;
        public string AccountName => _player.AccountName;
        public PlayerDisplay(Panel activePlayerPanel,
            Panel formerPlayerPanel, Player player, Func<uint, uint, AsyncTexture2D> iconGetter, ObservableCollection<string> availableRoles)
        {
            _activePlayerPanel = activePlayerPanel;
            _formerPlayerPanel = formerPlayerPanel;
            _player = player;
            _iconGetter = iconGetter;
            _availableRoles = availableRoles;
            
            CreateDetailsButtonAndDropDowns();

            _availableRoles.CollectionChanged += UpdateDropdowns;
            player.PropertyChanged += UpdateDetailsButton;
        }

        public void RemovePlayerFromActivePanel()
        {
            _detailsButton.Parent = _formerPlayerPanel;
        }

        public void MoveFormerSquadMemberToActivePanel()
        {
            _detailsButton.Parent = _activePlayerPanel;
        }

        public void MakeInvisible()
        {
            _detailsButton.Visible = false;
        }

        public void DisposeDetailsButton()
        {
            _detailsButton.Dispose();
            _player.PropertyChanged -= UpdateDetailsButton;
        }

        /// <summary>
        /// Updates the text on the DetailsButton for the PlayerDisplay when a player changes characters
        /// </summary>
        public void UpdateCharacter(Player player)
        {
            _player = player;
            UpdateDetailsButtonWithNewCharacter();
        }

        private void CreateDetailsButtonAndDropDowns()
        {
            CreateDetailsButtonOnly();
            _dropdown1 = CreateDropdown();
            _dropdown2 = CreateDropdown();
        }
        private void CreateDetailsButtonOnly()
        {
            _detailsButton = new DetailsButton
            {
                Parent = _activePlayerPanel,
                Text = $"{_player.CharacterName} ({_player.AccountName})",
                IconSize = DetailsIconSize.Small,
                ShowVignette = true,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                Icon = _iconGetter(_player.Profession, _player.CurrentSpecialization),
                Size = new Point(354, 90)
            };
        }

        private void UpdateDetailsButton(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Player.CurrentSpecialization)) return;
            if (IsFormerSquadMember) return;
            if (_player.Profession == 0) return;
            _detailsButton.Icon = _iconGetter(_player.Profession, _player.CurrentSpecialization);
        }

        private void UpdateDetailsButtonWithNewCharacter()
        {
            _detailsButton.Text = $"{_player.CharacterName} ({_player.AccountName})";
            _detailsButton.Icon = _iconGetter(_player.Profession, _player.CurrentSpecialization);
            _detailsButton.BasicTooltipText = GetPreviousCharactersToolTipText();
        }

        private Dropdown CreateDropdown()
        {
            var dropdown = new Dropdown
            {
                Parent = _detailsButton,
                Width = 150
            };
            dropdown.Items.Add(_placeholderRoleName);
            foreach (var role in _availableRoles)
            {
                dropdown.Items.Add(role);
            }
            return dropdown;
        }

        private void UpdateDropdowns(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.ToString().Equals("Remove"))
            {
                foreach (var removedItem in e.OldItems)
                {
                    _dropdown1 = RemoveItemFromDropdown(_dropdown1, removedItem.ToString());
                    _dropdown2 = RemoveItemFromDropdown(_dropdown2, removedItem.ToString());
                }
            }
            else
            {
                foreach (var newItem in e.NewItems)
                {
                    _dropdown1 = AddItemToDropddown(_dropdown1, newItem.ToString());
                    _dropdown2 = AddItemToDropddown(_dropdown2, newItem.ToString());
                }
            }
        }

        private Dropdown RemoveItemFromDropdown(Dropdown dropdown, string removedItem)
        {
            if (dropdown.SelectedItem.Equals(removedItem))
            {
                dropdown.SelectedItem = _placeholderRoleName;
            }
            dropdown.Items.Remove(removedItem);
            return dropdown;
        }

        private Dropdown AddItemToDropddown(Dropdown dropdown, string newItem)
        {
            dropdown.Items.Add(newItem);
            return dropdown;
        }

        private string GetPreviousCharactersToolTipText()
        {
            var tooltip = new StringBuilder("Previously played...").AppendLine();
            foreach (var character in _player.PreviouslyPlayedCharacters)
            {
                tooltip.AppendLine($"{Specialization.GetEliteName(character.CurrentSpecialization, character.Profession)} ({character.CharacterName})");
            }
            return tooltip.ToString().Trim();
        }
    }
}
