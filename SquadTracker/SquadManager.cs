using System.Linq;

namespace Torlando.SquadTracker
{
    class SquadManager
    {
        public delegate void PlayerJoinedSquadHandler(Player player, bool isReturning);
        public delegate void PlayerLeftSquadHandler(string accountName);

        public event PlayerJoinedSquadHandler PlayerJoinedSquad;
        public event PlayerLeftSquadHandler PlayerLeftSquad;

        private readonly PlayersManager _playersManager;

        private readonly Squad _squad;

        public SquadManager(PlayersManager playersManager)
        {
            _playersManager = playersManager;

            _squad = new Squad();

            var players = _playersManager.GetPlayers();
            foreach (var player in players.Where(p => p.IsInInstance))
            {
                _squad.CurrentMembers.Add(player);
            }

            _playersManager.PlayerJoinedInstance += OnPlayerJoinedInstance;
            _playersManager.PlayerLeftInstance += OnPlayerLeftInstance;
        }

        public Squad GetSquad()
        {
            return _squad;
        }

        private void OnPlayerJoinedInstance(Player newPlayer)
        {
            _squad.CurrentMembers.Add(newPlayer);

            var isReturning = false;
            if (_squad.FormerMembers.Contains(newPlayer))
            {
                isReturning = true;
                _squad.FormerMembers.Remove(newPlayer);
            }

            this.PlayerJoinedSquad?.Invoke(newPlayer, isReturning);
        }

        private void OnPlayerLeftInstance(string accountName)
        {
            var player = _squad.CurrentMembers.FirstOrDefault(p => p.AccountName == accountName);
            if (player == null) return;

            _squad.CurrentMembers.Remove(player);
            _squad.FormerMembers.Add(player);

            this.PlayerLeftSquad?.Invoke(accountName);
        }
    }
}
