using Blish_HUD.ArcDps.Common;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Torlando.SquadTracker
{
    public class PlayerCollection
    {
        private ObservableCollection<Player> _players;
        private ConcurrentDictionary<string, CommonFields.Player> _arcPlayersInSquad;

        private Panel _activePlayerPanel;
        private Panel _formerPlayerPanel;

        public PlayerCollection(ConcurrentDictionary<string, CommonFields.Player> arcPlayersInSquad, Panel activePlayerPanel, Panel formerPlayerPanel)
        {
            _players = new ObservableCollection<Player>();
            _arcPlayersInSquad = arcPlayersInSquad;
            _activePlayerPanel = activePlayerPanel;
            _formerPlayerPanel = formerPlayerPanel;
        }

        public async Task AddPlayer(CommonFields.Player arcPlayer, Task<Blish_HUD.Content.AsyncTexture2D> icon, ObservableCollection<string> availableRoles)
        {
            if (_players.Any(x => x.CharacterName.Equals(arcPlayer.CharacterName)))
            {
                // Move from former players if player rejoined
                var player = GetPlayer(arcPlayer);
                if (player?.IsFormerSquadMember ?? false)
                {
                    player.MoveFormerSquadMemberToActivePanel();
                }
                //Don't add duplicate player
                return;
            }
            _players.Add(new Player(_activePlayerPanel, _formerPlayerPanel, arcPlayer, await icon, availableRoles));
        }

        public void RemovePlayerFromActivePanel(CommonFields.Player arcPlayer)
        {
            //if (arcPlayer.Self && !_players.Any(x => x.IsSelf)) return; //Don't remove yourself, unless you changed characters
            var playerToRemove = GetPlayer(arcPlayer);
            playerToRemove.RemovePlayerFromActivePanel();
        }

        public void ClearFormerPlayers()
        {
            foreach (var player in _players)
            {
                if (player.IsFormerSquadMember)
                    player.DisposeDetailsButton(); //todo - test this
            }
        }

        private CommonFields.Player GetArcPlayer(string characterName)
        {
            return _arcPlayersInSquad.First(x => x.Value.CharacterName.Equals(characterName)).Value;
        }

        private Player GetPlayer(CommonFields.Player arcPlayer)
        {
            return _players.First(x => x.CharacterName.Equals(arcPlayer.CharacterName));
        }
    }

    public class Player
    {
        #region Data
        private CommonFields.Player _arcPlayer;
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

        public string CharacterName => _arcPlayer.CharacterName;
        public bool IsFormerSquadMember => _detailsButton.Parent?.Equals(_formerPlayerPanel) ?? false;
        public bool IsSelf => _arcPlayer.Self;
        public Player(Panel activePlayerPanel, 
            Panel formerPlayerPanel, CommonFields.Player arcPlayer, Blish_HUD.Content.AsyncTexture2D icon, ObservableCollection<string> availableRoles)
        {
            _activePlayerPanel = activePlayerPanel;
            _formerPlayerPanel = formerPlayerPanel;
            _arcPlayer = arcPlayer;
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
                Text = $"{_arcPlayer.CharacterName} ({_arcPlayer.AccountName})",
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
