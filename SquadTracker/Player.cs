using System.Collections.Generic;

namespace Torlando.SquadTracker
{
    internal class Player
    {
        public string AccountName { get; set; }
        public bool IsInInstance { get; set; } = true;

        public Character CurrentCharacter
        {
            get => _currentCharacter;
            set
            {
                _currentCharacter = value;
                if (!_knownCharacters.Contains(value))
                {
                    _knownCharacters.Add(value);
                    value.Player = this;
                }
            }
        }
        public IReadOnlyCollection<Character> KnownCharacters => _knownCharacters;

        public Player(string accountName, Character currentCharacter)
        {
            AccountName = accountName;
            CurrentCharacter = currentCharacter;
        }

        private Character _currentCharacter;
        private readonly HashSet<Character> _knownCharacters = new HashSet<Character>();

#if DEBUG
        public override string ToString()
        {
            return $"{AccountName} ({_knownCharacters.Count} character(s))";
        }
#endif
    }
}
