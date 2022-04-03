using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Torlando.SquadTracker.Constants;
using Torlando.SquadTracker.RolesScreen;

namespace Torlando.SquadTracker.SquadPanel
{
    class PlayerDisplay : DetailsButton
    {
        public Dropdown Role1Dropdown { get; set; }
        public Dropdown Role2Dropdown { get; set; }
        public string CharacterName 
        { 
            get 
            { 
                return _characterName; 
            } 
            set 
            {
                _characterName = value;
                Text = $"{CharacterName} ({AccountName})";
            } 
        }
        private string _characterName;
        public string AccountName 
        { 
            get 
            { 
                return _accountName; 
            }
            set
            {
                _accountName = value;
                Text = $"{CharacterName} ({AccountName})";
            } 
        }
        private string _accountName;

        private Image _roleIcon1 = new Image { Size = new Point(27, 27) };
        private Image _roleIcon2 = new Image { Size = new Point(27, 27) };

        private const string _placeholderRoleName = Placeholder.DefaultRole;

        public PlayerDisplay(IEnumerable<Role> availableRoles) : base()
        {
            IconSize = DetailsIconSize.Small;
            ShowVignette = true;
            HighlightType = DetailsHighlightType.LightHighlight;
            ShowToggleButton = true;
            Size = new Point(354, 90);
            Role1Dropdown = CreateDropdown(availableRoles, _roleIcon1);
            Role2Dropdown = CreateDropdown(availableRoles, _roleIcon2);
        }

        //ToDo - KnownCharacters
        public void UpdateToolTip(string text)
        {
            Tooltip.BasicTooltipText = text;
        }

        private Dropdown CreateDropdown(IEnumerable<Role> roles, Image roleIcon)
        {
            roleIcon.Parent = this;
            var dropdown = new Dropdown
            {
                Parent = this,
                Width = 135
            };
            dropdown.Items.Add(_placeholderRoleName);
            foreach (var role in roles.OrderBy(role => role.Name.ToLowerInvariant()))
            {
                dropdown.Items.Add(role.Name);
            }
            dropdown.ValueChanged += delegate
            {
                var selectedRole = roles.FirstOrDefault(role => role.Name.Equals(dropdown.SelectedItem));
                roleIcon.Texture = selectedRole?.Icon ?? null;
            };
            return dropdown;
        }
    }
}
