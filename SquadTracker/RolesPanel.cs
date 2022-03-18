using System.Collections.ObjectModel;
using System.Linq;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Torlando.SquadTracker.SquadPanel;

namespace Torlando.SquadTracker
{
    public class RolesPanel
    {
        #region Controls

        public Panel MainPanel { get; private set; }
        private TextBox NewRoleTextBox { get; set; }
        private StandardButton AddButton { get; set; }
        private Label ErrorLabel { get; set; }
        public Panel RolesFlowPanel { get; private set; }

        #endregion

        private ObservableCollection<Role> _roles;

        public RolesPanel(Panel basePanel, ObservableCollection<Role> roles, int marginLeft)
        {
            _roles = roles;

            BuildPanel(basePanel, marginLeft);
        }

        private void BuildPanel(Panel basePanel, int marginLeft)
        {
            // Main container
            MainPanel = new Panel
            {
                Parent = basePanel,
                Location = new Point(marginLeft, basePanel.Top),
                Size = new Point(basePanel.Width - marginLeft, basePanel.Height),
                Visible = false,
            };

            // First row, with a 5 units gap between elements
            NewRoleTextBox = new TextBox
            {
                Parent = MainPanel,
                PlaceholderText = "Create a new role…",
            };

            AddButton = new StandardButton
            {
                Parent = MainPanel,
                Text = "Add",
                Location = new Point(NewRoleTextBox.Right + 5, MainPanel.Top),
                Size = new Point(50, NewRoleTextBox.Height),
            };

            ErrorLabel = new Label
            {
                Parent = MainPanel,
                TextColor = Color.OrangeRed,
                Location = new Point(AddButton.Right + 5, MainPanel.Top + 3),
                AutoSizeWidth = true,
            };

            // Second row, with a 10 units gap with the first row
            RolesFlowPanel = new FlowPanel
            {
                Parent = MainPanel,
                Location = new Point(NewRoleTextBox.Left, NewRoleTextBox.Bottom + 10),
                Title = "Currently Defined Roles",
                Size = new Point(MainPanel.Width, MainPanel.Height - NewRoleTextBox.Height - 10),
                CanScroll = true,
                ShowBorder = true,
                ControlPadding = new Vector2(8, 8)
            };

            foreach (var role in _roles)
            {
                CreateRoleButton(role);
            }

            // Events
            NewRoleTextBox.EnterPressed += (o, e) => AddNewRole();
            AddButton.Click += (o, e) => AddNewRole();
        }

        private void AddNewRole()
        {
            var newRoleName = NewRoleTextBox.Text.Trim();
            if (_roles.Any(role => role.Name == newRoleName))
            {
                ErrorLabel.Text = "A role with this name already exists.";
                return;
            }

            ErrorLabel.Text = string.Empty;

            var role = new Role(newRoleName);
            _roles.Add(role);
            CreateRoleButton(role);
            NewRoleTextBox.Text = string.Empty;

            // Keep the focus on the textbox for a better flow.
            //newRoleTb.Focused = true; // TODO: Not working yet, need to see if it's a Blish HUD bug.
        }

        private void CreateRoleButton(Role role)
        {
            var newRoleButton = new DetailsButton
            {
                Parent = RolesFlowPanel,
                Text = role.Name,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowVignette = true,
                ShowToggleButton = true,
                Icon = role.Icon,
                IconSize = DetailsIconSize.Small
            };

            var removeButton = new StandardButton
            {
                Parent = newRoleButton,
                Text = "Remove"
            };

            removeButton.Click += (o, e) =>
            {
                _roles.Remove(role);
                RolesFlowPanel.RemoveChild(newRoleButton);
            };
        }
    }
}
