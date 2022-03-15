using System.Collections.Generic;

namespace Torlando.SquadTracker.Models
{
    public class Squad
    {
        ICollection<Player> CurrentSquadMembers { get; } = new HashSet<Player>();
        ICollection<Player> FormerSquadMembers { get; } = new HashSet<Player>();

        ICollection<Role> FilledRoles { get; } = new List<Role>();
    }
}
