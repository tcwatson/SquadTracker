using Blish_HUD.ArcDps.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Torlando.SquadTracker.Models
{
    internal class OldPlayer : INotifyPropertyChanged
    {
        public OldPlayer(CommonFields.Player arcPlayer, OldPlayer previousCharacter = null) 
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
                PreviouslyPlayedCharacters = new HashSet<OldPlayer>();
            }
        }

        public string AccountName { get; private set; }
        public bool IsSelf { get; private set; }
        public string CharacterName { get; private set; }
        public uint Profession { get; private set; }
        public bool HasChangedCharacters => PreviouslyPlayedCharacters?.Count > 0;
        public HashSet<OldPlayer> PreviouslyPlayedCharacters { get; set; }

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
