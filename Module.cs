using Blish_HUD;
using Blish_HUD.ArcDps;
using Blish_HUD.ArcDps.Common;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Torlando.SquadTracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        private const string MODULE_FOLDER_NAME = "squadtracker";

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        private ObservableCollection<Role> _customRoles;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        #region Textures

        private IReadOnlyDictionary<uint, AsyncTexture2D> _professionIcons;
        private IReadOnlyDictionary<uint, AsyncTexture2D> _specializationIcons;

        #endregion

        #region Controls
        private WindowTab _windowTab;
        private Panel _tabPanel;
        private FlowPanel _squadMembersPanel;
        private FlowPanel _formerSquadMembersPanel;
        private FlowPanel _squadRolesFlowPanel;
        private Panel _squadRolePanel;
        private MenuItem _squadMembersMenu;
        private MenuItem _squadRolesMenu;
        private Panel _menu;
        private StandardButton _clearFormerSquadButton;
        private List<DetailsButton> _playersDetails = new List<DetailsButton>();

        #endregion

        private PlayerCollection _playerCollection;

        private ConcurrentDictionary<string, CommonFields.Player> _arcPlayers;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        /// <summary>
        /// Define the settings you would like to use in your module.  Settings are persistent
        /// between updates to both Blish HUD and your module.
        /// </summary>
        protected override void DefineSettings(SettingCollection settings)
        {
        }

        /// <summary>
        /// Allows your module to perform any initialization it needs before starting to run.
        /// Please note that Initialize is NOT asynchronous and will block Blish HUD's update
        /// and render loop, so be sure to not do anything here that takes too long.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Load content and more here. This call is asynchronous, so it is a good time to run
        /// any long running steps for your module including loading resources from file or ref.
        /// </summary>
        protected override async Task LoadAsync()
        {
            await LoadSpecializationIconsAsync();
            await LoadRoles();
        }

        private async Task LoadSpecializationIconsAsync()
        {
            var connection = new Gw2Sharp.Connection();
            using var client = new Gw2Sharp.Gw2Client(connection);
            var webApiClient = client.WebApi.V2;

            var professionsClient = webApiClient.Professions;
            var professions = await professionsClient.AllAsync();

            _professionIcons = professions.ToDictionary(
                keySelector: (profession) => (uint)profession.Code,
                (profession) => GameService.Content.GetRenderServiceTexture(profession.IconBig)
            );

            var specializationClient = webApiClient.Specializations;
            var eliteSpecs = await specializationClient.ManyAsync(Specialization.EliteCodes);

            _specializationIcons = eliteSpecs.ToDictionary(
                keySelector: (spec) => (uint)spec.Id,
                (spec) => GameService.Content.GetRenderServiceTexture(spec.ProfessionIconBig)
            );
        }

        private async Task LoadRoles()
        {
            // Throws if the squadtracker folder does not exists, but Blish
            // HUD creates it from the manifest so it's probably okay!
            var directoryName = DirectoriesManager.RegisteredDirectories.First(directoryName => directoryName == MODULE_FOLDER_NAME);
            var directoryPath = DirectoriesManager.GetFullDirectoryPath(directoryName);

            _customRoles = await RolesPersister.LoadRolesFromFileSystem(directoryPath);

            foreach (var role in _customRoles)
            {
                if (!string.IsNullOrEmpty(role.IconPath))
                {
                    try
                    {
                        role.Icon = ContentsManager.GetTexture(role.IconPath);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn($"Could not load texture {role.IconPath}: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Allows you to perform an action once your module has finished loading (once
        /// <see cref="LoadAsync"/> has completed).  You must call "base.OnModuleLoaded(e)" at the
        /// end for the <see cref="Module.ModuleLoaded"/> event to fire.
        /// </summary>
        protected override void OnModuleLoaded(EventArgs e)
        {
            _tabPanel = BuildPanel(GameService.Overlay.BlishHudWindow.ContentRegion);
            _windowTab = GameService.Overlay.BlishHudWindow.AddTab("Squad Tracker", ContentsManager.GetTexture(@"textures\commandertag.png"), _tabPanel);
            GameService.ArcDps.Common.Activate();
            GameService.ArcDps.Common.PlayerAdded += PlayerAddedEvent;
            GameService.ArcDps.Common.PlayerRemoved += PlayerRemovedEvent;
            GameService.ArcDps.RawCombatEvent += RawCombatEvent;
            _playerCollection = new PlayerCollection(_arcPlayers, _squadMembersPanel, _formerSquadMembersPanel);

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private void PlayerAddedEvent(CommonFields.Player player)
        {
            _arcPlayers = (ConcurrentDictionary<string, CommonFields.Player>)GameService.ArcDps.Common.PlayersInSquad;
            _playerCollection.AddPlayer(player, GetSpecializationIcon, _customRoles);
            _squadMembersPanel.BasicTooltipText = "";
        }

        private void RawCombatEvent(object sender, RawCombatEventArgs e)
        {
            var ag = e.CombatEvent.Src;
            _playerCollection.UpdatePlayerSpecialization(ag.Name, ag.Elite);
        }

        private void PlayerRemovedEvent(CommonFields.Player player)
        {
            _arcPlayers = (ConcurrentDictionary<string, CommonFields.Player>)GameService.ArcDps.Common.PlayersInSquad;
            _playerCollection.RemovePlayerFromActivePanel(player);
        }

        private Panel BuildPanel(Rectangle panelBounds)
        {
            var panel = new Panel
            {
                CanScroll = false,
                Size = panelBounds.Size,
                
            };
            SetupMenu(panel);

            _squadRolePanel = new Panel
            {
                Parent = panel,
                Location = new Point(_menu.Right + 10, _menu.Top),
                Size = new Point(panel.Width, panel.Height),
                Visible = false
            };
            var newRole = new TextBox
            {
                Parent = _squadRolePanel,
                PlaceholderText = "new role here"
            };
            var addButton = new StandardButton
            {
                Parent = _squadRolePanel,
                Text = "Add",
                Location = new Point (newRole.Right + 50, _squadRolePanel.Top)
            };


            _squadRolesFlowPanel = new FlowPanel
            {
                Parent = _squadRolePanel,
                Location = new Point(newRole.Left, newRole.Bottom + 10),
                Title = "Currently Defined Roles",
                Size = new Point(panel.Width - _menu.Width - 5, panel.Height),
                CanScroll = true,
                ShowBorder = true,
                ControlPadding = new Vector2(8, 8)
            };

            foreach (var role in _customRoles)
            {
                CreateRoleButton(role);
            }

            addButton.Click += delegate
            {
                var role = new Role(newRole.Text);
                _customRoles.Add(role);
                CreateRoleButton(role);
                newRole.Text = string.Empty;
            };

            _squadMembersMenu.Click += delegate { 
                _squadMembersPanel.Visible = true;
                _formerSquadMembersPanel.Visible = true;
                _squadRolePanel.Visible = false;
                _clearFormerSquadButton.Visible = true;
            };
            _squadRolesMenu.Click += delegate { 
                _squadMembersPanel.Visible = false;
                _squadRolePanel.Visible = true;
                _formerSquadMembersPanel.Visible = false;
                _clearFormerSquadButton.Visible = false;
            };

            _squadMembersPanel.BasicTooltipText = "You loaded Blish HUD after starting Guild Wars 2. Please change maps to refresh.";
            //SetupPlaceholderPlayers();

            return panel;
        }

        private void SetupMenu(Panel basePanel)
        {
            _menu = new Panel
            {
                Title = "Squad Tracker Menu",
                ShowBorder = true,
                Size = Panel.MenuStandard.Size,
                Parent = basePanel
            };

            var menuCategories = new Menu
            {
                Size = _menu.ContentRegion.Size,
                MenuItemHeight = 40,
                Parent = _menu,
                CanSelect = true
            };
            _squadMembersMenu = menuCategories.AddMenuItem("Squad Members");
            _squadMembersMenu.Select();

            _squadRolesMenu = menuCategories.AddMenuItem("Squad Roles");

            _squadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = basePanel,
                Location = new Point(_menu.Right + 10, _menu.Top),
                CanScroll = true,
                Size = new Point(basePanel.Width - _menu.Width - 5, 530), //
                Title = "Current Squad Members",
                ShowBorder = true
            };
            _formerSquadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = basePanel,
                Location = new Point(_menu.Right + 10, _squadMembersPanel.Bottom + 10),
                CanScroll = true,
                Size = new Point(basePanel.Width - _menu.Width - 5, 150),
                Title = "Former Squad Members",
                ShowBorder = true
            };
            _clearFormerSquadButton = new StandardButton
            {
                Parent = basePanel,
                Text = "Clear",
                Location = new Point(_formerSquadMembersPanel.Right - 135, _formerSquadMembersPanel.Top + 5)
            };
            _clearFormerSquadButton.Click += delegate
            {
                _playerCollection.ClearFormerPlayers();
            };
        }

        private void CreateRoleButton(Role role)
        {
            var newRoleButton = new DetailsButton
            {
                Parent = _squadRolesFlowPanel,
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
                //Location = new Point(newRoleButton.Right, newRoleButton.Top),
                Text = "Remove"
            };
            removeButton.Click += delegate
            {
                _customRoles.Remove(role);
                _squadRolesFlowPanel.RemoveChild(newRoleButton);
            };
        }

        protected override void Update(GameTime gameTime)
        {

        }


        // happens when you disable the module
        protected override void Unload()
        {
            // Unload here
            Console.WriteLine("unloaded");
            // All static members must be manually unset
        }

        private AsyncTexture2D GetSpecializationIcon(uint professionCode, uint specializationCode)
        {
            if (specializationCode == 0)
            {
                return _professionIcons[professionCode];
            }

            return _specializationIcons[specializationCode];
        }




        /// <summary>
        /// Call this from BuildPanel() for testing with placeholder characters
        /// </summary>
        private void SetupPlaceholderPlayers()
        {
            var placeHolderPlayer = new DetailsButton
            {
                Parent = _squadMembersPanel,
                Text = "placeholder 1",
                IconSize = DetailsIconSize.Small,
                ShowVignette = true,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                Icon = GameService.Content.GetRenderServiceTexture("https://render.guildwars2.com/file/A84BD2D74D3239451E3FF4EFC0F6A146F3F6653E/1770224.png"),
                Size = new Point(354, 90)
            };
            var placeHolderPlayer2 = new DetailsButton
            {
                Parent = _squadMembersPanel,
                Text = "placeholder 2",
                IconSize = DetailsIconSize.Small,
                ShowVignette = true,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                Icon = GameService.Content.GetRenderServiceTexture("https://render.guildwars2.com/file/2BE4F4AB7F69206BBDABB20CACB1DC7911B33F4E/1770212.png"),
                Size = new Point(354, 90)
            };
            var dropDown = new Dropdown
            {
                Parent = placeHolderPlayer2,
                //Location = new Point(placeHolderPlayer2.Right, Dropdown.Standard.ControlOffset.Y),
                Width = 150
            };
            var dropDown2 = new Dropdown
            {
                Parent = placeHolderPlayer2,
                //Location = new Point(placeHolderPlayer2.Right, Dropdown.Standard.ControlOffset.Y),
                Width = 150
            };
            dropDown.Items.Add("test item 1");
            dropDown.Items.Add("test item 2");
            dropDown2.Items.Add("test item 1");
            dropDown2.Items.Add("test item 2");
            _customRoles.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action.ToString().Equals("Remove"))
                {
                    foreach (var removedItem in e.OldItems)
                    {
                        if (dropDown.SelectedItem.Equals(removedItem.ToString()))
                        {
                            dropDown.SelectedItem = "test item 1";
                        }
                        if (dropDown2.SelectedItem.Equals(removedItem.ToString()))
                        {
                            dropDown2.SelectedItem = "test item 1";
                        }
                        dropDown.Items.Remove(removedItem.ToString());
                        dropDown2.Items.Remove(removedItem.ToString());
                    }
                }
                else
                {
                    foreach (var newItem in e.NewItems)
                    {
                        dropDown.Items.Add(newItem.ToString());
                        dropDown2.Items.Add(newItem.ToString());
                    }
                }

            };
        }
    }

}
