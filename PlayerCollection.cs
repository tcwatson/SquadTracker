using Blish_HUD.ArcDps.Common;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task AddPlayer(CommonFields.Player arcPlayer, Task<Blish_HUD.Content.AsyncTexture2D> icon, ObservableCollection<string> availableRoles)
        {
            if (_players.ContainsKey(arcPlayer.CharacterName))
            {
                // Move from former players if player rejoined
                var playerDisplay = GetPlayer(arcPlayer);
                if (playerDisplay?.IsFormerSquadMember ?? false)
                {
                    playerDisplay.MoveFormerSquadMemberToActivePanel();
                }
                //Don't add duplicate player
                return;
            }

            var player = new Player(arcPlayer.AccountName, isSelf: arcPlayer.Self, characterName: arcPlayer.CharacterName, profession: arcPlayer.Profession, currentSpecialization: arcPlayer.Elite);

            _players.Add(player.CharacterName, player);
            _playerDisplays.Add(new PlayerDisplay(_activePlayerPanel, _formerPlayerPanel, player, await icon, availableRoles));
        }

        public void UpdatePlayerSpecialization(string characterName, uint newSpec)
        {
            var hasPlayer = _players.TryGetValue(characterName, out var player);
            if (!hasPlayer) return;
            if (player.CurrentSpecialization == newSpec) return;

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
            return _playerDisplays.First(x => x.CharacterName.Equals(arcPlayer.CharacterName));
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
        private Blish_HUD.Content.AsyncTexture2D _icon;
        private const string _placeholderRoleName = "Select a role...";
        #endregion

        public string CharacterName => _player.CharacterName;
        public bool IsFormerSquadMember => _detailsButton.Parent?.Equals(_formerPlayerPanel) ?? false;
        public bool IsSelf => _player.IsSelf;
        public PlayerDisplay(Panel activePlayerPanel,
            Panel formerPlayerPanel, Player player, Blish_HUD.Content.AsyncTexture2D icon, ObservableCollection<string> availableRoles)
        {
            _activePlayerPanel = activePlayerPanel;
            _formerPlayerPanel = formerPlayerPanel;
            _player = player;
            _icon = icon;
            _availableRoles = availableRoles;
            CreateDetailsButton();
            _availableRoles.CollectionChanged += UpdateDropdowns;
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
        }

        private void CreateDetailsButton()
        {
            _detailsButton = new DetailsButton
            {
                Parent = _activePlayerPanel,
                Text = $"{_player.CharacterName} ({_player.AccountName})",
                IconSize = DetailsIconSize.Small,
                ShowVignette = true,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                Icon = _icon,
                Size = new Point(354, 90)
            };
            _dropdown1 = CreateDropdown();
            _dropdown2 = CreateDropdown();
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

    }
}
