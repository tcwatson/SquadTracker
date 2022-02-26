using Blish_HUD.ArcDps.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Torlando.SquadTracker
{
    public class Player : INotifyPropertyChanged
    {
        public Player(CommonFields.Player arcPlayer, Player previousCharacter = null) 
        {
            AccountName = arcPlayer.AccountName;
            IsSelf = arcPlayer.Self;
            CharacterName = arcPlayer.CharacterName;
            Profession = arcPlayer.Profession;

            _currentSpecialization = arcPlayer.Elite;
            if (previousCharacter != null)
            {
                PreviouslyPlayedCharacters = previousCharacter.PreviouslyPlayedCharacters;
                PreviouslyPlayedCharacters.Add(previousCharacter);
            }
            else
            {
                PreviouslyPlayedCharacters = new HashSet<Player>();
            }
        }

        public string AccountName { get; private set; }
        public bool IsSelf { get; private set; }
        public string CharacterName { get; private set; }
        public uint Profession { get; private set; }
        public bool HasChangedCharacters => PreviouslyPlayedCharacters?.Count > 0;
        public HashSet<Player> PreviouslyPlayedCharacters { get; set; }
        public bool TryGetPlayedCharacterByName(string characterName, out Player player)
        {
            if (CharacterName.Equals(characterName))
            {
                player = this;
                return true;
            }
            foreach (var character in PreviouslyPlayedCharacters)
            {
                if (characterName.Equals(character.CharacterName))
                {
                    player = character;
                    return true;
                }
            }
            player = null;
            return false;
        }

        public uint CurrentSpecialization
        {
            get => _currentSpecialization;
            set
            {
                _currentSpecialization = value;
                OnPropertyChanged();
            }
        }

        private uint _currentSpecialization;

        #region INotifyPropertyChanged

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
