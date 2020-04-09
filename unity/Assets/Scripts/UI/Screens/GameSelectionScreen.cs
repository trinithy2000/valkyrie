using Assets.Scripts.Content;
using FFGAppImport;
using System.IO;
using System.Threading;
using UnityEngine;
using ValkyrieTools;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Screens
{

    // First screen which contains game type selection
    // and import controls
    public class GameSelectionScreen
    {
        private FFGImport fcD2E;
        private FFGImport fcMoM;
        private FFGImport fcIA;

        protected string importType = "";
        private Thread importThread;

        private static readonly StringKey D2E_NAME = new StringKey("val", "D2E_NAME");
        private static readonly StringKey D2E_APP_NOT_FOUND = new StringKey("val", "D2E_APP_NOT_FOUND");

        private static readonly StringKey MOM_NAME = new StringKey("val", "MOM_NAME");
        private static readonly StringKey MOM_APP_NOT_FOUND = new StringKey("val", "MOM_APP_NOT_FOUND");

        private static readonly StringKey IA_NAME = new StringKey("val", "IA_NAME");
        private static readonly StringKey IA_APP_NOT_FOUND = new StringKey("val", "IA_APP_NOT_FOUND");

        private static readonly StringKey CONTENT_IMPORT = new StringKey("val", "CONTENT_IMPORT");
        private static readonly StringKey CONTENT_REIMPORT = new StringKey("val", "CONTENT_REIMPORT");
        private static readonly StringKey CONTENT_IMPORTING = new StringKey("val", "CONTENT_IMPORTING");
        private static readonly StringKey CONTENT_LOCATE = new StringKey("val", "CONTENT_LOCATE");
        private static readonly StringKey CONTENT_INSTALL_VIA_STEAM = new StringKey("val", "CONTENT_INSTALL_VIA_STEAM");
        private static readonly StringKey CONTENT_INSTALL_VIA_GOOGLEPLAY = new StringKey("val", "CONTENT_INSTALL_VIA_GOOGLEPLAY");

        private static readonly string MOM_APP_URL_ANDROID = "https://play.google.com/store/apps/details?id=com.fantasyflightgames.mom";
        private static readonly string MOM_APP_URL_STEAM = "https://store.steampowered.com/app/478980/Mansions_of_Madness/";

        private static readonly string D2E_APP_URL_ANDROID = "https://play.google.com/store/apps/details?id=com.fantasyflightgames.rtl";
        private static readonly string D2E_APP_URL_STEAM = "https://store.steampowered.com/app/477200/Descent_Road_to_Legend/";

        private static readonly string IA_APP_URL_ANDROID = "https://play.google.com/store/apps/details?id=com.fantasyflightgames.rtl";
        private static readonly string IA_APP_URL_STEAM = "https://store.steampowered.com/app/703980/Star_Wars_Imperial_Assault__Legends_of_the_Alliance/";

        // Create a menu which will take up the whole screen and have options.  All items are dialog for destruction.
        public GameSelectionScreen()
        {
            Draw();
        }

        public void Draw()
        {
            // This will destroy all
            Destroyer.Destroy();

            Game game = Game.Get();

            game.gameType = new NoGameType();

            // Get the current content for games
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                fcD2E = new FFGImport(FFGAppImport.GameType.D2E, Platform.MacOS, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcMoM = new FFGImport(FFGAppImport.GameType.MoM, Platform.MacOS, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcIA = new FFGImport(FFGAppImport.GameType.IA, Platform.MacOS, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);

            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                fcD2E = new FFGImport(FFGAppImport.GameType.D2E, Platform.Android, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcMoM = new FFGImport(FFGAppImport.GameType.MoM, Platform.Android, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcIA = new FFGImport(FFGAppImport.GameType.IA, Platform.Android, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
            }
            else
            {
                fcD2E = new FFGImport(FFGAppImport.GameType.D2E, Platform.Windows, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcMoM = new FFGImport(FFGAppImport.GameType.MoM, Platform.Windows, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
                fcIA = new FFGImport(FFGAppImport.GameType.IA, Platform.Windows, Game.AppData() + Path.DirectorySeparatorChar, Application.isEditor);
            }

            fcD2E.Inspect();
            fcMoM.Inspect();
            fcIA.Inspect();

            // Banner Image
            Sprite bannerSprite;
            Texture2D newTex = Resources.Load("sprites/banner") as Texture2D;

            GameObject banner = new GameObject("banner")
            {
                tag = Game.DIALOG
            };

            banner.transform.SetParent(game.uICanvas.transform);

            RectTransform trans = banner.AddComponent<RectTransform>();
            trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1 * UIScaler.GetPixelsPerUnit(), 7f * UIScaler.GetPixelsPerUnit());
            trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (UIScaler.GetWidthUnits() - 18f) * UIScaler.GetPixelsPerUnit() / 2f, 18f * UIScaler.GetPixelsPerUnit());
            banner.AddComponent<CanvasRenderer>();

            Image image = banner.AddComponent<Image>();
            bannerSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero, 1);
            image.sprite = bannerSprite;
            image.rectTransform.sizeDelta = new Vector2(18f * UIScaler.GetPixelsPerUnit(), 7f * UIScaler.GetPixelsPerUnit());

            // Heading
            UIElement screenUI = new UIElement(null, "Screen_bg");
            screenUI.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
            screenUI.SetBGColor(Color.clear);

            // first button y offset
            float offset = 9.5f;

            // Draw D2E button
            bool D2E_need_import = fcD2E.NeedImport();
            bool D2E_import_available = fcD2E.ImportAvailable();
            Color startColor = D2E_need_import ? Color.grey : Color.white;
            int fontSize = UIScaler.GetMediumFont();

            UIElement ui = new UIElement(screenUI.GetTransform(), "Descent");
            ui.SetLocation((UIScaler.GetWidthUnits() - 30) / 2, offset, 30, 3);
            // If we need to import we can't play this type
            if (!D2E_need_import)
            {
                ui.SetText(D2E_NAME, startColor);
                ui.SetButton(delegate { D2E(); });
            }
            else
            {
                string message = "";
                if (D2E_import_available)
                {
                    message = D2E_NAME.Translate();
                }
                else
                {
                    message = D2E_NAME.Translate() + System.Environment.NewLine + D2E_APP_NOT_FOUND.Translate();
                    fontSize = (int)(UIScaler.GetMediumFont() / 1.05f);
                }
                ui.SetText(message, startColor);
            }
            ui.SetFontSize(fontSize);
            ui.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(ui, startColor);

            // Draw D2E import button
            ui = new UIElement(screenUI.GetTransform(), "Descent_import");
            if (D2E_import_available || !D2E_need_import)
            {
                ui.SetLocation((UIScaler.GetWidthUnits() - 14) / 2, offset + 3.3f, 14, 2);
                StringKey keyText = D2E_need_import ? CONTENT_IMPORT : CONTENT_REIMPORT;
                ui.SetText(keyText);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Import("D2E", !D2E_import_available); });
                ui.SetBGColor(new Color(0, 0.03f, 0f));
                new UIElementBorder(ui);
            }
            else // Import unavailable
            {
                // only install button for Android
                if (Application.platform == RuntimePlatform.Android)
                {
                    ui = new UIElement(screenUI.GetTransform(), "Descent_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() - 24) / 2, offset + 3.3f, 24, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_GOOGLEPLAY, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(D2E_APP_URL_ANDROID); });
                    new UIElementBorder(ui, Color.red);
                }
                else
                {
                    // install and locate button for other systems
                    ui = new UIElement(screenUI.GetTransform(), "Descent_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) - 13, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_STEAM, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(D2E_APP_URL_STEAM); });
                    new UIElementBorder(ui, Color.red);

                    ui = new UIElement(screenUI.GetTransform(), "Descent_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) + 1, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_LOCATE, Color.red);
                    ui.SetButton(delegate { Import("D2E", true); });
                    new UIElementBorder(ui, Color.red);
                }
            }

            offset += 6.5f;

            // Draw MoM button
            bool MoM_need_import = fcMoM.NeedImport();
            bool MoM_import_available = fcMoM.ImportAvailable();
            startColor = MoM_need_import ? Color.grey : Color.white;
            fontSize = UIScaler.GetMediumFont();

            ui = new UIElement(screenUI.GetTransform(), "MoM");
            ui.SetLocation((UIScaler.GetWidthUnits() - 30) / 2, offset, 30, 3);
            // If we need to import we can't play this type
            if (!MoM_need_import)
            {
                ui.SetText(MOM_NAME, startColor);
                ui.SetButton(delegate { MoM(); });
            }
            else
            {
                string message = "";
                if (MoM_import_available)
                {
                    message = MOM_NAME.Translate();
                }
                else
                {
                    message = MOM_NAME.Translate() + System.Environment.NewLine + MOM_APP_NOT_FOUND.Translate();
                    fontSize = (int)(UIScaler.GetMediumFont() / 1.05f);
                }
                ui.SetText(message, startColor);
            }
            ui.SetFontSize(fontSize);
            ui.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(ui, startColor);

            // Draw MoM import button
            ui = new UIElement(screenUI.GetTransform(), "MoM_import");
            if (MoM_import_available || !MoM_need_import)
            {
                ui.SetLocation((UIScaler.GetWidthUnits() - 14) / 2, offset + 3.3f, 14, 2);
                StringKey keyText = MoM_need_import ? CONTENT_IMPORT : CONTENT_REIMPORT;
                ui.SetText(keyText);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Import("MoM", !MoM_import_available); });
                ui.SetBGColor(new Color(0, 0.03f, 0f));
                new UIElementBorder(ui);
            }
            else // Import unavailable
            {
                // only install button for Android
                if (Application.platform == RuntimePlatform.Android)
                {
                    ui = new UIElement(screenUI.GetTransform(), "MoM_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() - 24) / 2, offset + 3.3f, 24, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_GOOGLEPLAY, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(MOM_APP_URL_ANDROID); });
                    new UIElementBorder(ui, Color.red);
                }
                else
                {
                    // install and locate button for other systems
                    ui = new UIElement(screenUI.GetTransform(), "MoM_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) - 13, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_STEAM, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(MOM_APP_URL_STEAM); });
                    new UIElementBorder(ui, Color.red);

                    ui = new UIElement(screenUI.GetTransform(), "MoM_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) + 1, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_LOCATE, Color.red);
                    ui.SetButton(delegate { Import("MoM", true); });
                    new UIElementBorder(ui, Color.red);
                }
            }

            offset += 6.5f;

            // Draw IA button
            bool IA_need_Import = fcIA.NeedImport();
            bool IA_import_available = fcIA.ImportAvailable();
            startColor = IA_need_Import ? Color.grey : Color.white;
            fontSize = UIScaler.GetMediumFont();

            ui = new UIElement(screenUI.GetTransform(), "IA");
            ui.SetLocation((UIScaler.GetWidthUnits() - 30) / 2, offset, 30, 3);
            // If we need to import we can't play this type
            if (!IA_need_Import)
            {
                ui.SetText(IA_NAME, startColor);
                ui.SetButton(delegate { IA(); });
            }
            else
            {
                string message = "";
                if (IA_import_available)
                {
                    message = IA_NAME.Translate();
                }
                else
                {
                    message = IA_NAME.Translate() + System.Environment.NewLine + IA_APP_NOT_FOUND.Translate();
                    fontSize = (int)(UIScaler.GetMediumFont() / 1.05f);
                }
                ui.SetText(message, startColor);
            }

            ui.SetFontSize(fontSize);
            ui.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(ui, startColor);

            // Draw IA import button
            ui = new UIElement(screenUI.GetTransform(), "IA_import");
            if (IA_import_available || !IA_need_Import)
            {
                ui.SetLocation((UIScaler.GetWidthUnits() - 14) / 2, offset + 3.3f, 14, 2);
                StringKey keyText = D2E_need_import ? CONTENT_IMPORT : CONTENT_REIMPORT;
                ui.SetText(keyText);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Import("IA", !IA_import_available); });
                ui.SetBGColor(new Color(0, 0.03f, 0f));
                new UIElementBorder(ui);
            }
            else // Import unavailable
            {
                // only install button for Android
                if (Application.platform == RuntimePlatform.Android)
                {
                    ui = new UIElement(screenUI.GetTransform(), "IA_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() - 24) / 2, offset + 3.3f, 24, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_GOOGLEPLAY, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(IA_APP_URL_ANDROID); });
                    new UIElementBorder(ui, Color.red);
                }
                else
                {

                    // install and locate button for other systems
                    ui = new UIElement(screenUI.GetTransform(), "IA_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) - 13, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_INSTALL_VIA_STEAM, Color.red);
                    ui.SetButton(delegate { GotoWebBrowser(IA_APP_URL_STEAM); });
                    new UIElementBorder(ui, Color.red);

                    ui = new UIElement(screenUI.GetTransform(), "IA_import");
                    ui.SetLocation((UIScaler.GetWidthUnits() / 2) + 1, offset + 3.3f, 12, 1.3f);
                    ui.SetText(CONTENT_LOCATE, Color.red);
                    ui.SetButton(delegate { Import("IA", true); });
                    new UIElementBorder(ui, Color.red);
                }
            }
            // will display a button if a new version is available
            VersionManager.GetLatestVersionAsync(CheckForNewValkyrieVersion);
        }

        // Start game as D2E
        public void D2E()
        {
            ValkyrieDebug.Log("INFO: Start game as D2E");

            // Check if import neeeded
            if (!fcD2E.NeedImport())
            {
                Game game = Game.Get();

                game.gameType = new D2EGameType();

                // Loading list of content - doing this later is not required
                game.cd = new ContentData(game.gameType.DataDirectory());
                // Check if we found anything
                if (game.cd.GetPacks().Count == 0)
                {
                    ValkyrieDebug.Log("Error: Failed to find any content packs, please check that you have them present in: " + game.gameType.DataDirectory() + System.Environment.NewLine);
                    Application.Quit();
                }

                // Load localization before content
                loadLocalization();

                // Load the base content - pack will be loaded later if required
                game.cd.LoadContentID("");

                // Download quests list
                game.questsList = new QuestsManager();
                Texture2D cursor = Resources.Load("sprites/CursorD2E") as Texture2D;
                Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

                Destroyer.MainMenu();
            }
        }

        // Import content
        public void Import(string type, bool manual_path_selection = false)
        {
            string path = null;

            ValkyrieDebug.Log("INFO: Import " + type);


            if (manual_path_selection)
            {
                string app_filename = "";
                if (type.Equals("D2E"))
                {
                    app_filename = "Road to Legend";
                }

                if (type.Equals("MoM"))
                {
                    app_filename = "Mansions of Madness";
                }

                if (type.Equals("IA"))
                {
                    app_filename = "Imperial assault";
                }

                string[] array_path = SFB.StandaloneFileBrowser.OpenFilePanel("Select file " + app_filename + ".exe", "", "exe", false);

                // return when pressing back
                if (array_path.Length == 0)
                {
                    return;
                }

                path = Path.Combine(Path.GetDirectoryName(array_path[0]), app_filename + "_Data");

                // return if wrong file is selected
                if (!Directory.Exists(path))
                {
                    return;
                }
            }

            Destroyer.Destroy();

            new LoadingScreen(CONTENT_IMPORTING.Translate());
            importType = type;

            if (type.Equals("D2E"))
            {
                importThread = new Thread(new ThreadStart(delegate { fcD2E.Import(path); }));
            }
            if (type.Equals("MoM"))
            {
                importThread = new Thread(new ThreadStart(delegate { fcMoM.Import(path); }));
            }

            if (type.Equals("IA"))
            {
                importThread = new Thread(new ThreadStart(delegate { fcIA.Import(path); }));
            }

            importThread.Start();
        }

        // Start game as MoM
        public void MoM()
        {
            ValkyrieDebug.Log("INFO: Start game as MoM");

            // Check if import neeeded
            if (!fcMoM.NeedImport())
            {
                Game game = Game.Get();
                game.gameType = new MoMGameType();

                // Loading list of content - doing this later is not required
                game.cd = new ContentData(game.gameType.DataDirectory());
                // Check if we found anything
                if (game.cd.GetPacks().Count == 0)
                {
                    ValkyrieDebug.Log("Error: Failed to find any content packs, please check that you have them present in: " + game.gameType.DataDirectory() + System.Environment.NewLine);
                    Application.Quit();
                }

                // Load localization before content
                loadLocalization();

                // Load the base content - pack will be loaded later if required
                game.cd.LoadContentID("");

                // Download quests list
                game.questsList = new QuestsManager();
                // MoM also has a special reound controller
                game.roundControl = new RoundControllerMoM();
                Texture2D cursor = Resources.Load("sprites/CursorMoM") as Texture2D;
                Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

                Destroyer.MainMenu();
            }
        }

        // Start game as IA
        public void IA()
        {
            ValkyrieDebug.Log("INFO: Start game as IA");

            // Check if import neeeded
            if (!fcIA.NeedImport())
            {
                Game game = Game.Get();
                game.gameType = new IAGameType();

                // Loading list of content - doing this later is not required
                game.cd = new ContentData(game.gameType.DataDirectory());
                // Check if we found anything
                if (game.cd.GetPacks().Count == 0)
                {
                    ValkyrieDebug.Log("Error: Failed to find any content packs, please check that you have them present in: " + game.gameType.DataDirectory() + System.Environment.NewLine);
                    Application.Quit();
                }

                // Load localization before content
                loadLocalization();

                // Load the base content - pack will be loaded later if required
                game.cd.LoadContentID("");

                // Download quests list
                game.questsList = new QuestsManager();
                // MoM also has a special reound controller
                game.roundControl = new RoundControllerMoM();
                Texture2D cursor = Resources.Load("sprites/CursorIA") as Texture2D;
                Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

                Destroyer.MainMenu();
            }

        }

        /// <summary>
        /// After selecting game, we load the localization file.
        /// Deppends on the gameType selected.
        /// There are two Localization files from ffg, one for D2E and one for MoM
        /// </summary>
        private void loadLocalization()
        {
            // After content import, we load the localization file
            if (LocalizationRead.selectDictionary("ffg") == null)
            {
                DictionaryI18n ffgDict = new DictionaryI18n();
                foreach (string file in Directory.GetFiles(ContentData.ImportPath() + "/text", "Localization_*.txt"))
                {
                    ffgDict.AddDataFromFile(file);
                }
                LocalizationRead.AddDictionary("ffg", ffgDict);

                // CoSH used for Dunwich Horror data
                DictionaryI18n cshDict = new DictionaryI18n();
                foreach (string file in Directory.GetFiles(ContentData.ImportPath() + "/text", "SCENARIO_CULT_OF_SENTINEL_HILL_MAD22_*.txt"))
                {
                    cshDict.AddDataFromFile(file);
                }
                LocalizationRead.AddDictionary("csh", cshDict);
            }
        }

        public void Update()
        {
            if (importThread == null)
            {
                return;
            }

            if (importThread.IsAlive)
            {
                return;
            }

            importThread = null;
            ExtractBundles();
            // TODO: Delete Obb dir for Android build here
            Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractBundles()
        {
            try
            {
                string importDir = Path.Combine(Game.AppData(), importType + Path.DirectorySeparatorChar + "import");
                string bundlesFile = Path.Combine(importDir, "bundles.txt");
                ValkyrieDebug.Log("Loading all bundles from '" + bundlesFile + "'");
                string[] bundles = File.ReadAllLines(bundlesFile);
                foreach (string file in bundles)
                {
                    AssetBundle bundle = AssetBundle.LoadFromFile(file);
                    if (bundle == null)
                    {
                        continue;
                    }

                    ValkyrieDebug.Log("Loading assets from '" + file + "'");
                    foreach (TextAsset asset in bundle.LoadAllAssets<TextAsset>())
                    {
                        string textDir = Path.Combine(importDir, "text");
                        Directory.CreateDirectory(textDir);
                        string f = Path.Combine(importDir, Path.Combine(textDir, asset.name + ".txt"));
                        ValkyrieDebug.Log("Writing text asset to '" + f + "'");
                        File.WriteAllText(f, asset.ToString());
                    }
                    bundle.Unload(false);
                }
            }
            catch (System.Exception ex)
            {
                ValkyrieDebug.Log("ExtractBundles caused " + ex.GetType().Name + ": " + ex.Message + " " + ex.StackTrace);
            }
            importType = "";
        }

        // Exit Valkyrie
        public void Exit()
        {
            ValkyrieDebug.Log("INFO: Leaving Valkyrie");

            Application.Quit();
        }

        // Open link and quit Valkyrie
        public void GotoWebBrowser(string url)
        {
            ValkyrieDebug.Log("INFO: Accessing new version");

            Application.OpenURL(url);

            Application.Quit();
        }

        public void CheckForNewValkyrieVersion()
        {
            StringKey NEW_VERSION_AVAILABLE = new StringKey("val", "NEW_VERSION_AVAILABLE");

            if (VersionManager.VersionNewer(Game.Get().version, VersionManager.online_version))
            {
                float string_width = 0f;
                UIElement ui = new UIElement();
                ui.SetText(NEW_VERSION_AVAILABLE, Color.green);
                string_width = ui.GetStringWidth(NEW_VERSION_AVAILABLE, UIScaler.GetMediumFont());
                ui.SetLocation(UIScaler.GetRight() - 3 - string_width, UIScaler.GetBottom(-3), string_width + 2, 2);
                ui.SetText(NEW_VERSION_AVAILABLE, Color.green);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { GotoWebBrowser(VersionManager.GetlatestReleaseURL()); });
                ui.SetBGColor(new Color(0, 0.03f, 0f));
                new UIElementBorder(ui, Color.green);
            }
        }

    }
}