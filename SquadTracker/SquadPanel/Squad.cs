using System.Collections.Generic;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class Squad
    {
        public ICollection<PlayerModel> CurrentSquadMembers { get; } = new HashSet<PlayerModel>();
        public IReadOnlyCollection<PlayerModel> FormerSquadMembers { get; private set; } = new HashSet<PlayerModel>();


        public ICollection<Role> FilledRoles { get; } = new List<Role>();

        public void ClearFormerSquadMembers()
        {
            FormerSquadMembers = new HashSet<PlayerModel>();
        }


        #region Test
        public void AddPlayer(PlayerModel playerModel)
        {
            CurrentSquadMembers.Add(playerModel);
        }
        #endregion
    }
}
