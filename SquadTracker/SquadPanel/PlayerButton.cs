using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Torlando.SquadTracker.SquadPanel
{
    class PlayerButton : DetailsButton
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

        private const string _placeholderRoleName = "Select a role...";

        public PlayerButton(AsyncTexture2D icon, List<Role> availableRoles) : base()
        {
            IconSize = DetailsIconSize.Small;
            ShowVignette = true;
            HighlightType = DetailsHighlightType.LightHighlight;
            ShowToggleButton = true;
            Icon = icon;
            Size = new Point(354, 90);
            Role1Dropdown = CreateDropdown(availableRoles, _roleIcon1);
            Role2Dropdown = CreateDropdown(availableRoles, _roleIcon2);
        }

        //ToDo - KnownCharacters
        public void UpdateToolTip(string text)
        {
            Tooltip.BasicTooltipText = text;
        }

        private Dropdown CreateDropdown(List<Role> roles, Image roleIcon)
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
                roleIcon.Texture = roles.FirstOrDefault(role => role.Name.Equals(dropdown.SelectedItem))?.Icon ?? null;
            };
            return dropdown;
        }
    }
}
