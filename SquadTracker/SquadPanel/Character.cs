using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class Character : INotifyPropertyChanged
    {
        public Character(string name, PlayerModel player)
        {
            Name = name;
            Player = player;

            player.KnownCharacters.Add(this);
        }

        public string Name { get; }
        public PlayerModel Player { get; }

        public uint? Profession { get => profession; set => SetField(ref profession, value); }
        private uint? profession;

        public uint? Elite { get => elite; set => SetField(ref elite, value); }
        private uint? elite;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion INotifyPropertyChanged
    }
}
