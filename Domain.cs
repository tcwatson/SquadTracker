using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Torlando.SquadTracker
{
    public class Player : INotifyPropertyChanged
    {
        public Player(string accountName, bool isSelf, string characterName, uint profession, uint currentSpecialization)
        {
            AccountName = accountName;
            IsSelf = isSelf;
            CharacterName = characterName;
            Profession = profession;

            _currentSpecialization = currentSpecialization;
        }

        public string AccountName { get; private set; }
        public bool IsSelf { get; private set; }
        public string CharacterName { get; private set; }
        public uint Profession { get; private set; }

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
            if (((PlayerDisplay)PropertyChanged?.Target)?.IsFormerSquadMember ?? false) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
