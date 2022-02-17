namespace Torlando.SquadTracker
{
    public class Player
    {
        public Player(string accountName, bool isSelf, string characterName, uint profession, uint currentSpecialization)
        {
            AccountName = accountName;
            IsSelf = isSelf;
            CharacterName = characterName;
            Profession = profession;
            CurrentSpecialization = currentSpecialization;
        }

        public string AccountName { get; private set; }
        public bool IsSelf { get; private set; }
        public string CharacterName { get; private set; }
        public uint Profession { get; private set; }
        public uint CurrentSpecialization { get; private set; }
    }
}
