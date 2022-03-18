using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torlando.SquadTracker.Models;

namespace Torlando.SquadTracker.Player
{
    class PlayerButton : DetailsButton
    {
        public AsyncTexture2D RoleIcon1 { get; set; }
        public AsyncTexture2D RoleIcon2 { get; set; }
        public Dropdown Role1Dropdown { get; set; }
        public Dropdown Role2Dropdown { get; set; }

        public PlayerButton(Container container, PlayerModel player, AsyncTexture2D icon) : base()
        {
            Parent = container;
            Text = $"{player.CharacterName} ({player.AccountName})";
            IconSize = DetailsIconSize.Small;
            ShowVignette = true;
            HighlightType = DetailsHighlightType.LightHighlight;
            ShowToggleButton = true;
            Icon = icon;
            Size = new Point(354, 90);
        }
    }
}
