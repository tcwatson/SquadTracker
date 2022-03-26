using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class Squad
    {
        public ICollection<Player> CurrentSquadMembers { get; } = new HashSet<Player>();
        public ICollection<Player> FormerSquadMembers { get; /*private set;*/ } = new HashSet<Player>();


        //public ICollection<Role> FilledRoles { get; } = new List<Role>();

        public void ClearFormerSquadMembers()
        {
            //FormerSquadMembers = new HashSet<Player>();
        }
    }
}
