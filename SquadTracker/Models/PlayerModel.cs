using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Torlando.SquadTracker.Models
{
    internal class PlayerModel : INotifyPropertyChanged
    {
        public ICollection<Character> KnownCharacters { get; } = new HashSet<Character>();
        public ICollection<Role> AssignedRoles { get; } = new List<Role>();
        public string AccountName { get; set; }
        //public bool IsSelf { get; private set; }
        public string CharacterName { get; set; }
        public uint Profession { get; private set; }
        public bool HasChangedCharacters => KnownCharacters?.Count > 1;

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
