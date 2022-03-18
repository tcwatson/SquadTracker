using Blish_HUD.Content;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Torlando.SquadTracker.Models;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class SquadPanelPresenter : Presenter<SquadPanelView, Squad>
    {
        private readonly ContentsManager _contentsManager;
        private SettingEntry<bool> _areColorIconsEnabled;

        #region Textures

        private IReadOnlyDictionary<uint, Texture2D> _professionIcons;
        private IReadOnlyDictionary<uint, Texture2D> _specializationIcons;

        #endregion

        public SquadPanelPresenter(
            SquadPanelView view, 
            Squad model, 
            ContentsManager contentsManager,
            SettingEntry<bool> areColorIconsEnabled
        ) : base (view, model) 
        {
            _contentsManager = contentsManager;
            _areColorIconsEnabled = areColorIconsEnabled;

            LoadSpecializationIcons();
        }
        
        public void ClearFormerSquadMembers()
        {
            Model.ClearFormerSquadMembers();
        }

        private AsyncTexture2D GetSpecializationIcon(uint professionCode, uint specializationCode)
        {
            if (specializationCode == 0)
            {
                return _professionIcons[professionCode];
            }

            return _specializationIcons[specializationCode];
        }

        private void LoadSpecializationIcons()
        {
            bool useTangoIcons = _areColorIconsEnabled.Value;

            _professionIcons = Specialization.ProfessionCodes.ToDictionary(
                keySelector: (profession) => profession,
                (profession) => _contentsManager.GetTexture(Specialization.GetCoreIconPath(profession, useTangoIcons))
            );

            _specializationIcons = Specialization.EliteCodes.ToDictionary(
                keySelector: (spec) => spec,
                (spec) => _contentsManager.GetTexture(Specialization.GetEliteIconPath(spec, useTangoIcons))
            );
        }

        #region Test
        public void AddPlayer()
        {
            var player = new PlayerModel
            {
                AccountName = "test.1234",
                CharacterName = "Frodo",
                Profession = 2,
                CurrentSpecialization = 18
            };
            var icon = GetSpecializationIcon(player.Profession, player.CurrentSpecialization);
            Model.AddPlayer(player);
            View.SpawnPlayerButton(player, icon);
        }
        #endregion
    }
}
