using Microsoft.Xna.Framework.Graphics;

namespace Torlando.SquadTracker
{
    public class Role
    {
        public string Name { get; set; }
        public Texture2D Icon { get; set; }

        public Role(string name, Texture2D icon)
        {
            Name = name;
            Icon = icon;
        }

        public Role(string name)
        {
            Name = name;
        }
    }
}
