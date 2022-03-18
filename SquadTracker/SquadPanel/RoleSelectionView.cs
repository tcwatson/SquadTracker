using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Torlando.SquadTracker.SquadPanel
{
    class RoleSelectionView : View<RoleSelectionPresenter>
    {
        #region Controls
        private Dropdown _dropdown1;
        private Dropdown _dropdown2;
        private Image _roleIcon1 = new Image { Size = new Point(27, 27) };
        private Image _roleIcon2 = new Image { Size = new Point(27, 27) };

        private const string _placeholderRoleName = "Select a role...";
        #endregion

        public RoleSelectionView(ObservableCollection<Role> availableRoles)
        {
            Presenter = new RoleSelectionPresenter(this, availableRoles);
        }

        protected override void Build(Container buildPanel)
        {
            _dropdown1 = CreateDropdown(buildPanel);
            _dropdown2 = CreateDropdown(buildPanel);
            // base.Build(buildPanel);
        }


        private Dropdown CreateDropdown(Container parent)
        {
            var dropdown = new Dropdown
            {
                Parent = parent,
                Width = 135
            };
            dropdown.Items.Add(_placeholderRoleName);
            return dropdown;

            //foreach (var role in _availableRoles.OrderBy(role => role.Name.ToLowerInvariant()))
            //{
            //    dropdown.Items.Add(role.Name);
            //}
            //dropdown.ValueChanged += delegate
            //{
            //    roleIcon.Texture = _availableRoles.FirstOrDefault(role => role.Name.Equals(dropdown.SelectedItem))?.Icon ?? null;
            //};
            //return dropdown;
        }
    }
}
