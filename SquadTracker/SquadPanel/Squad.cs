using System.Collections.Generic;
using Torlando.SquadTracker.Models;

namespace Torlando.SquadTracker.SquadPanel
{
    public class Squad
    {
        ICollection<PlayerModel> CurrentSquadMembers { get; } = new HashSet<PlayerModel>();
        ICollection<PlayerModel> FormerSquadMembers { get; } = new HashSet<PlayerModel>();

        ICollection<Role> FilledRoles { get; } = new List<Role>();
    }
}
