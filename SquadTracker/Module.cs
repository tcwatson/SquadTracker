using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Torlando.SquadTracker.MainScreen;
using Torlando.SquadTracker.RolesScreen;
using Torlando.SquadTracker.SquadPanel;

namespace Torlando.SquadTracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        private const string MODULE_FOLDER_NAME = "squadtracker";

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        private PlayersManager _playersManager;
        private SquadManager _squadManager;
        private PlayerIconsManager _playerIconsManager;
        private ObservableCollection<Role> _customRoles;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion


        #region Controls
        private WindowTab _newTab;

        #endregion

        private SettingEntry<bool> _areColorIconsEnabled; //todo: remove after refactor

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        /// <summary>
        /// Define the settings you would like to use in your module.  Settings are persistent
        /// between updates to both Blish HUD and your module.
        /// </summary>
        protected override void DefineSettings(SettingCollection settings)
        {
            _areColorIconsEnabled = settings.DefineSetting(
                "EnableColorIcons", 
                true, () => "Enable Color Icons", 
                () => "When enabled, replaces the monochrome icons with icons colored to match their profession color"
            );
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
            await LoadRoles();
            _playerIconsManager = new PlayerIconsManager(this.ContentsManager, _areColorIconsEnabled);
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
                        if (role.IconPath.StartsWith("icons"))
                        {
                            role.Icon = ContentsManager.GetTexture(role.IconPath);
                        }
                        else
                        {
                            if (!File.Exists(role.IconPath)) return;
                            using var textureStream = File.Open(role.IconPath, FileMode.Open);
                            if (textureStream != null)
                            {
                                Logger.Debug("Successfully loaded texture {dataReaderFilePath}.", role.IconPath);
                                role.Icon = TextureUtil.FromStreamPremultiplied(GameService.Graphics.GraphicsDevice, textureStream);
                            }
                        }
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
            _playersManager = new PlayersManager(GameService.ArcDps);
            _squadManager = new SquadManager(_playersManager);

            _newTab = GameService.Overlay.BlishHudWindow.AddTab(
                icon: ContentsManager.GetTexture(@"textures\commandertag.png"),
                viewFunc: () => {
                    var view = new MainScreenView();
                    var presenter = new MainScreenPresenter(view, _playersManager, _squadManager, _playerIconsManager, _customRoles);
                    return view.WithPresenter(presenter);
                },
                name: "Squad Tracker Tab"
            );
            
            GameService.ArcDps.Common.Activate();

            // Base handler must be called
            base.OnModuleLoaded(e);

            #if DEBUG
            GameService.Overlay.BlishHudWindow.Show();
            #endif
        }

        protected override void Update(GameTime gameTime)
        {

        }

        // happens when you disable the module
        protected override void Unload()
        {
            
        }
    }

}
