﻿using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace Torlando.SquadTracker.Models
{
    public class Role
    {
        public string Name { get; private set; }

        [JsonIgnore]
        public Texture2D Icon { get; set; }
        public string IconPath { get; set; } = string.Empty;

        [JsonConstructor]
        public Role(string name)
        {
            Name = name;
        }

        public Role() { }

        public Role(string name, string iconPath, ContentsManager contentsManager)
        {
            Name = name;
            IconPath = iconPath;
            Icon = contentsManager.GetTexture(IconPath);
        }
    }
}
