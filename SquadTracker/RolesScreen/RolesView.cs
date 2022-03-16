using System.Collections.Generic;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Torlando.SquadTracker.SquadPanel;

namespace Torlando.SquadTracker.RolesScreen
{
    class RolesView : View<RolesPresenter>
    {
        #region Controls

        private Panel _mainPanel;
        private FlowPanel _rolesFlowPanel;
        private TextBox _newRoleTextBox;
        private StandardButton _addRoleButton;
        private Label _addRoleErrorLabel;
        private readonly IDictionary<string, DetailsButton> _roleDisplays = new Dictionary<string, DetailsButton>();

        #endregion

        protected override void Build(Container buildPanel)
        {
            // Main container
            _mainPanel = new Panel
            {
                Parent = buildPanel,
                Location = new Point(buildPanel.ContentRegion.Left, buildPanel.ContentRegion.Top),
                Size = new Point(buildPanel.ContentRegion.Width, buildPanel.ContentRegion.Height),
            };

            // First row, with a 5 units gap between elements
            _newRoleTextBox = new TextBox
            {
                Parent = _mainPanel,
                PlaceholderText = "Create a new role…",
            };

            _addRoleButton = new StandardButton
            {
                Parent = _mainPanel,
                Text = "Add",
                Location = new Point(_newRoleTextBox.Right + 5, _mainPanel.Top),
                Size = new Point(50, _newRoleTextBox.Height),
            };

            _addRoleErrorLabel = new Label
            {
                Parent = _mainPanel,
                TextColor = Color.OrangeRed,
                Location = new Point(_addRoleButton.Right + 5, _mainPanel.Top + 3),
                AutoSizeWidth = true,
            };

            // Second row, with a 10 units gap with the first row
            _rolesFlowPanel = new FlowPanel
            {
                Parent = _mainPanel,
                Location = new Point(_newRoleTextBox.Left, _newRoleTextBox.Bottom + 10),
                Title = "Currently Defined Roles",
                Size = new Point(_mainPanel.Width, _mainPanel.Height - _newRoleTextBox.Height - 10),
                CanScroll = true,
                ShowBorder = true,
                ControlPadding = new Vector2(8, 8)
            };

            // Events
            _newRoleTextBox.EnterPressed += (o, e) => this.Presenter.CreateRole(_newRoleTextBox.Text);
            _addRoleButton.Click += (o, e) => this.Presenter.CreateRole(_newRoleTextBox.Text);
        }

        public void AddRoleDisplay(Role role)
        {
            var newRoleButton = new DetailsButton
            {
                Parent = _rolesFlowPanel,
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

            _roleDisplays.Add(role.Name, newRoleButton);

            removeButton.Click += (o, e) => this.Presenter.DeleteRole(role);
        }

        public void RemoveRoleDisplay(string roleName)
        {
            if (_roleDisplays.TryGetValue(roleName, out var roleDisplay) == false) return;

            _rolesFlowPanel.RemoveChild(roleDisplay);
            _roleDisplays.Remove(roleName);
        }

        public void ResetAddRoleForm()
        {
            _addRoleErrorLabel.Text = string.Empty;
            _newRoleTextBox.Text = string.Empty;

            // Keep the focus on the textbox for a better flow.
            //_newRoleTextBox.Focused = true; // TODO: Not working yet, need to see if it's a Blish HUD bug.
        }

        public void DisplayAddRoleError(string error)
            => _addRoleErrorLabel.Text = error;
    }
}
