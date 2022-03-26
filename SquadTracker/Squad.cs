using System.Collections.Generic;

namespace Torlando.SquadTracker
{
    internal class Squad
    {
        public ICollection<Player> CurrentMembers { get; } = new HashSet<Player>();
        public ICollection<Player> FormerMembers { get; private set; } = new HashSet<Player>();

        //public ICollection<Role> FilledRoles { get; } = new List<Role>();

        public void ClearFormerMembers()
        {
            this.FormerMembers = new HashSet<Player>();
        }
    }
}
