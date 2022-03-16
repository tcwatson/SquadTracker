using Blish_HUD.Content;
using Blish_HUD.Graphics.UI;
using Torlando.SquadTracker.Helpers;
using Torlando.SquadTracker.Models;
using Torlando.SquadTracker.Views;

namespace Torlando.SquadTracker.Presenters
{
    internal class PlayerPresenter : Presenter<PlayerView, PlayerModel>
    {
        public PlayerPresenter(PlayerView view, PlayerModel model) : base(view, model) 
        {
        }

        public void SetPlayerName()
        {
            View.SetPlayerText($"{Model.CharacterName} ({Model.AccountName})");
        }

        public void SetPlayerIcon(AsyncTexture2D icon)
        {
            //ToDo: how to get Settings here? 
            var iconPath = IconHelper.GetIconPath(Model.CurrentSpecialization, Model.Profession, true);
            //View.SetPlayerIcon(_contentsManager.GetTexture(iconPath));
            View.SetPlayerIcon(icon);
        }
    }
}
