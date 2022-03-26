using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Torlando.SquadTracker.RolesScreen
{
    public class Role
    {
        public string Name { get; }

        [JsonIgnore]
        public Texture2D Icon { get; set; }

        public string IconPath { get; set; } = string.Empty;

        public Role(string name)
        {
            Name = name;
        }

        [JsonConstructor]
        public Role(string name, string iconPath)
        {
            Name = name;
            IconPath = iconPath;
        }

        public Role(string name, string iconPath, Texture2D icon)
        {
            Name = name;
            IconPath = iconPath;
            Icon = icon;
        }
    }
}
