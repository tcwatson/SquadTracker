using Blish_HUD.ArcDps.Common;
using Blish_HUD.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyler.Blish_HUD_Module1
{
    class PlayerCollection
    {
        private ObservableCollection<Player> _players;
    }

    class Player
    {
        private DetailsButton _detailsButton;
        private CommonFields.Player _arcPlayer;
    }
}
