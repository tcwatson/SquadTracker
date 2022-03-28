using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.ArcDps;
using Blish_HUD.ArcDps.Common;

namespace Torlando.SquadTracker
{
    class PlayersManager
    {
        public delegate void PlayerJoinedInstanceHandler(Player newPlayer);
        public delegate void PlayerLeftInstanceHandler(string accountName);
        public delegate void CharacterChangedSpecializationHandler(Character character);

        public event PlayerJoinedInstanceHandler PlayerJoinedInstance;
        public event PlayerLeftInstanceHandler PlayerLeftInstance;
        public event CharacterChangedSpecializationHandler CharacterChangedSpecialization;

        private readonly ArcDpsService _arcDpsService;

        private readonly IDictionary<string, Player> _players = new Dictionary<string, Player>();
        private readonly IDictionary<string, Character> _characters = new Dictionary<string, Character>();

        public PlayersManager(ArcDpsService arcDpsService)
        {
            _arcDpsService = arcDpsService;

            _arcDpsService.Common.PlayerAdded += OnPlayerJoinedInstance;
            _arcDpsService.RawCombatEvent += OnSomeoneDidSomething;
            _arcDpsService.Common.PlayerRemoved += OnPlayerLeftInstance;
        }

        public IReadOnlyCollection<Player> GetPlayers()
        {
            return _players.Values.ToList(); // Return a clone.
        }

        private void OnPlayerJoinedInstance(CommonFields.Player arcDpsPlayer)
        {
            if (_characters.TryGetValue(arcDpsPlayer.CharacterName, out var character))
            {
                character.Specialization = arcDpsPlayer.Elite;
            }
            else
            {
                character = new Character(arcDpsPlayer.CharacterName, arcDpsPlayer.Profession, arcDpsPlayer.Elite);
                _characters.Add(character.Name, character);
            }

            if (_players.TryGetValue(arcDpsPlayer.AccountName, out var player))
            {
                player.CurrentCharacter = character;
                player.IsInInstance = true;
            }
            else
            {
                player = new Player(arcDpsPlayer.AccountName, character);
                _players.Add(player.AccountName, player);
            }

            this.PlayerJoinedInstance?.Invoke(player);
        }

        private void OnSomeoneDidSomething(object sender, RawCombatEventArgs e)
        {
            // Focus only on events tracing combat actions.
            if (e.CombatEvent.Ev == null) return;

            var src = e.CombatEvent.Src;
            if (_characters.TryGetValue(src.Name, out var srcCharacter) && srcCharacter.Specialization != src.Elite)
            {
                srcCharacter.Specialization = src.Elite;
                this.CharacterChangedSpecialization?.Invoke(srcCharacter);
            }

            var dst = e.CombatEvent.Dst;
            if (_characters.TryGetValue(dst.Name, out var dstCharacter) && dstCharacter.Specialization != dst.Elite)
            {
                dstCharacter.Specialization = dst.Elite;
                this.CharacterChangedSpecialization?.Invoke(dstCharacter);
            }
        }

        private void OnPlayerLeftInstance(CommonFields.Player arcDpsPlayer)
        {
            if (!_players.TryGetValue(arcDpsPlayer.AccountName, out var player)) return;

            player.IsInInstance = false;
            this.PlayerLeftInstance?.Invoke(player.AccountName);
        }
    }
}
