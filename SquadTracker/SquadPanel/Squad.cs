using System.Collections.Generic;
using Torlando.SquadTracker.Constants;

namespace Torlando.SquadTracker.SquadPanel
{
    internal class Squad
    {
        public ICollection<Player> CurrentMembers { get; } = new HashSet<Player>();
        public ICollection<Player> FormerMembers { get; private set; } = new HashSet<Player>();
        /// <summary>
        /// Key is account name, value is list of role names
        /// </summary>
        private Dictionary<string, List<string>> assignedRoles = new Dictionary<string, List<string>>();

        public List<string> GetRoles(string accountName)
        {
            if (!assignedRoles.TryGetValue(accountName, out var roles)) return new List<string> { Placeholder.DefaultRole, Placeholder.DefaultRole };
            return roles;
        }

        public void SetRole(string accountName, string role, int index)
        {
            if (!assignedRoles.ContainsKey(accountName))
            {
                assignedRoles.Add(accountName, new List<string> { Placeholder.DefaultRole, Placeholder.DefaultRole });
            }
            assignedRoles[accountName][index] = role;
        }

        //public ICollection<Role> FilledRoles { get; } = new List<Role>();

        public void ClearFormerMembers()
        {
            FormerMembers = new HashSet<Player>();
        }
    }
}
