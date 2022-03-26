namespace Torlando.SquadTracker
{
    internal class Character
    {
        public Character(string name, uint profession, uint specialization = default)
        {
            Name = name;
            Profession = profession;
            Specialization = specialization;
        }

        public string Name { get; }
        public uint Profession { get; }
        public uint Specialization { get; set; } = default;

        // Needed to use HashSets efficiently.
        public override int GetHashCode()
            => this.Name.GetHashCode();

#if DEBUG
        public override string ToString()
        {
            return $"{Name} ({SquadTracker.Specialization.GetEliteName(Specialization, Profession)})";
        }
#endif
    }
}
