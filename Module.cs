using Blish_HUD;
using Blish_HUD.ArcDps.Common;
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
using System.Linq;
using System.Threading.Tasks;

//todo - changing maps puts current player in former squad members. 
//todo - added members don't get the added dropdown items

namespace Blish_HUD_Module1
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        private ConcurrentDictionary<string, CommonFields.Player> _players;

        private ObservableCollection<string> _customRoles = new ObservableCollection<string>();
        private const string _placeholderRoleName = "Select a role...";

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        #region Controls
        private WindowTab _windowTab;
        private Panel _tabPanel;
        private FlowPanel _squadMembersPanel;
        private FlowPanel _formerSquadMembersPanel;
        private Panel _squadRolePanel;
        private MenuItem _squadMembersMenu;
        private MenuItem _squadRolesMenu;
        private Panel _menu;
        private List<DetailsButton> _playersDetails = new List<DetailsButton>();

        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            settings.DefineSetting("Test bool", true);
            settings.DefineSetting("Test float", (float)3.0);
            settings.DefineSetting("Test int", 2);
            settings.DefineSetting("Test string", "hello there");
            settings.AddSubCollection("subCollection1");
        }

        protected override void Initialize()
        {
            
        }

        protected override async Task LoadAsync()
        {

            

            _tabPanel = BuildPanel(GameService.Overlay.BlishHudWindow.ContentRegion);
            
        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            _windowTab = GameService.Overlay.BlishHudWindow.AddTab("testTab", this.ContentsManager.GetTexture(@"textures\1466345.png"), _tabPanel);
            GameService.ArcDps.Common.Activate();
            GameService.ArcDps.Common.PlayerAdded += PlayerAddedEvent;
            GameService.ArcDps.Common.PlayerRemoved += PlayerRemovedEvent;
            UpdateSquadMembers();
            // Base handler must be called
            base.OnModuleLoaded(e);
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
                Size = new Point(500, 500),
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
           

            var existingRoles = new FlowPanel
            {
                Parent = _squadRolePanel,
                Location = new Point(newRole.Left, newRole.Bottom + 10),
                Title = "Currently Defined Roles",
                Size = new Point(500, 500)
            };
            addButton.Click += delegate
            {
                _customRoles.Add(newRole.Text);
                var newRoleButton = new DetailsButton
                {
                    Parent = existingRoles,
                    Text = newRole.Text,
                    HighlightType = DetailsHighlightType.LightHighlight,
                    ShowVignette = false,
                    ShowToggleButton = true
                };
                var removeButton = new StandardButton
                {
                    Parent = newRoleButton,
                    //Location = new Point(newRoleButton.Right, newRoleButton.Top),
                    Text = "Remove"
                };
                removeButton.Click += delegate
                {
                    _customRoles.Remove(newRole.Text);
                    existingRoles.RemoveChild(newRoleButton);
                };
            };

            _squadMembersMenu.Click += delegate { 
                _squadMembersPanel.Visible = true;
                _formerSquadMembersPanel.Visible = true;
                _squadRolePanel.Visible = false; 
            };
            _squadRolesMenu.Click += delegate { 
                _squadMembersPanel.Visible = false;
                _squadRolePanel.Visible = true;
                _formerSquadMembersPanel.Visible = false;
            };

            SetupPlaceholderPlayers();

            return panel;
        }

        private void SetupMenu(Panel basePanel)
        {
            _menu = new Panel
            {
                Title = "my menu",
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
                Size = new Point(basePanel.Width - _menu.Width - 5, 500),
                Title = "Current Squad Members"
            };
            _formerSquadMembersPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(8, 8),
                Parent = basePanel,
                Location = new Point(_menu.Right + 10, _squadMembersPanel.Bottom + 10),
                CanScroll = true,
                Size = new Point(basePanel.Width - _menu.Width - 5, 500),
                Title = "Former Squad Members"
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

        private async Task<Blish_HUD.Content.AsyncTexture2D> GetSpecializationIcon(CommonFields.Player player)
        {
            // todo icon for core specs? throws null refernce?
            var connection = new Gw2Sharp.Connection();
            using var client = new Gw2Sharp.Gw2Client(connection);
            var webApiClient = client.WebApi.V2;
            var specializationClient = webApiClient.Specializations;
            var professionsClient = webApiClient.Professions;
            if (player.Elite == 0)
            {
                int playerProfession = (int) player.Profession;
                var allProffesions = await professionsClient.AllAsync();
                var proffessionId = allProffesions[playerProfession + 1].Id;
                var profession = await professionsClient.GetAsync(proffessionId);
                return GameService.Content.GetRenderServiceTexture(profession.IconBig);
            }
            var eliteSpec = await specializationClient.GetAsync((int)player.Elite);
            return GameService.Content.GetRenderServiceTexture(eliteSpec.ProfessionIconBig);
        }

        private async void UpdateSquadMembers()
        {
            _players = (ConcurrentDictionary<string, CommonFields.Player>)GameService.ArcDps.Common.PlayersInSquad;
            foreach (var member in _players)
            {
                if (!_playersDetails.Any(x => x.Text.Equals(member.Value.CharacterName)))
                {
                    var playerButton = new DetailsButton
                    {
                        Parent = _squadMembersPanel,
                        Text = member.Value.CharacterName,
                        IconSize = DetailsIconSize.Small,
                        ShowVignette = true,
                        HighlightType = DetailsHighlightType.LightHighlight,
                        ShowToggleButton = true,
                        Icon = await GetSpecializationIcon(member.Value),
                        Size = new Point(354, 90)
                    };
                    _playersDetails.Add(playerButton);
                     SetupDropdowns(playerButton);
                }
            }
        }


        private void SetupDropdowns(DetailsButton playerButton)
        {
            var dropDown = new Dropdown
            {
                Parent = playerButton,
                Width = 150
            };
            dropDown.Items.Add(_placeholderRoleName);
            var dropDown2 = new Dropdown
            {
                Parent = playerButton,
                Width = 150
            };
            dropDown2.Items.Add(_placeholderRoleName);
            _customRoles.CollectionChanged += UpdateDropdowns;
        }

        private void UpdateDropdowns(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var playerButton in _playersDetails)
            {
                var dropDowns = playerButton.GetDescendants().Where(x => x.GetType().Equals(typeof(Dropdown)));
                foreach (Dropdown dropDown in dropDowns)
                {
                    if (e.Action.ToString().Equals("Remove"))
                    {
                        foreach (var removedItem in e.OldItems)
                        {
                            if (dropDown.SelectedItem.Equals(removedItem.ToString()))
                            {
                                dropDown.SelectedItem = _placeholderRoleName;
                            }
                            dropDown.Items.Remove(removedItem.ToString());
                        }
                    }
                    else
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            dropDown.Items.Add(newItem.ToString());
                        }
                    }
                }
            }
        }

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

        private void PlayerAddedEvent(CommonFields.Player player)
        {
            UpdateSquadMembers();
        }

        private void PlayerRemovedEvent(CommonFields.Player player)
        {
            _players = (ConcurrentDictionary<string, CommonFields.Player>)GameService.ArcDps.Common.PlayersInSquad;
            //_playersDetails.Remove(_playersDetails.Find(x => x.Text.Equals(player.CharacterName)));
            _squadMembersPanel.Children.Where(x => ((DetailsButton)x).Text.Equals(player.CharacterName)).FirstOrDefault().Parent = _formerSquadMembersPanel;
        }
    }

}
