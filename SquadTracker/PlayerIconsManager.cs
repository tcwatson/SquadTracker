using System.Collections.Generic;
using System.Linq;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Graphics;

namespace Torlando.SquadTracker
{
    class PlayerIconsManager
    {
        private readonly ContentsManager _contentsManager;
        private readonly SettingEntry<bool> _areColorIconsEnabled;

        private IReadOnlyDictionary<uint, Texture2D> _professionIcons;
        private IReadOnlyDictionary<uint, Texture2D> _specializationIcons;

        public PlayerIconsManager(ContentsManager contentsManager, SettingEntry<bool> areColorIconsEnabled)
        {
            _contentsManager = contentsManager;
            _areColorIconsEnabled = areColorIconsEnabled;

            this.Initialize();

            _areColorIconsEnabled.SettingChanged += (o, e) => this.Initialize();
        }

        private void Initialize()
        {
            var useColoredIcons = _areColorIconsEnabled.Value;

            _professionIcons = Specialization.ProfessionCodes.ToDictionary(
                keySelector: (profession) => profession,
                (profession) => _contentsManager.GetTexture(Specialization.GetCoreIconPath(profession, useColoredIcons))
            );

            _specializationIcons = Specialization.EliteCodes.ToDictionary(
                keySelector: (spec) => spec,
                (spec) => _contentsManager.GetTexture(Specialization.GetEliteIconPath(spec, useColoredIcons))
            );
        }

        public AsyncTexture2D GetSpecializationIcon(uint professionCode, uint specializationCode)
        {
            if (specializationCode != default && specializationCode <= _specializationIcons.Keys.Max())
            {
                return _specializationIcons[specializationCode];
            }

            return _professionIcons[professionCode];
        }
    }
}
