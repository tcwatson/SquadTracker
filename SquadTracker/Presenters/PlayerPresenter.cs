using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Torlando.SquadTracker.Helpers;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Views;

namespace Torlando.SquadTracker.Presenters
{
    internal class PlayerPresenter : Presenter<PlayerView, PlayerModel>
    {
        private readonly ContentsManager _contentsManager;
        public PlayerPresenter(PlayerView view, PlayerModel model, ContentsManager contentsManager) : base(view, model) 
        {
            _contentsManager = contentsManager;
        }

        public void SetPlayerName()
        {
            View.SetPlayerText($"{Model.CharacterName} ({Model.AccountName})");
        }

        public void SetPlayerIcon()
        {
            //ToDo: how to get Settings here? 
            var iconPath = IconHelper.GetIconPath(Model.CurrentSpecialization, Model.Profession, true);
            View.SetPlayerIcon(_contentsManager.GetTexture(iconPath));
        }
    }
}
