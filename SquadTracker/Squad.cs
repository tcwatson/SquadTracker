using System.Collections.Generic;

namespace Torlando.SquadTracker
{
    internal class Squad
    {
        public ICollection<Player> CurrentMembers { get; } = new HashSet<Player>();
        public ICollection<Player> FormerMembers { get; } = new HashSet<Player>();


        //public ICollection<Role> FilledRoles { get; } = new List<Role>();

        public void ClearFormerSquadMembers()
        {
            //FormerMembers = new HashSet<Player>();
        }
    }
}
