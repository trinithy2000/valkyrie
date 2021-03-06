﻿using Assets.Scripts.Content;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Screens;
using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ValkyrieTools;

// General controller for the game
// There is one object of this class and it is used to find most game components
public class Game : MonoBehaviour
{

    public static readonly string MONSTERS = "monsters";
    public static readonly string HEROSELECT = "heroselect";
    public static readonly string BOARD = "board";
    public static readonly string QUESTUI = "questui";
    public static readonly string QUESTLIST = "questlist";
    public static readonly string EDITOR = "editor";
    public static readonly string UIPHASE = "uiphase";
    public static readonly string TRANSITION = "transition";
    public static readonly string DIALOG = "dialog";
    public static readonly string ACTIVATION = "activation";
    public static readonly string SHOP = "shop";
    public static readonly string ENDGAME = "endgame";
    public static readonly string BG_TASKS = "bg_tasks";
    public static readonly string LOGS = "logs";

    // This is populated at run time from the text asset
    public string version = "";

    // This is a reference to the Game object
    public static Game game;

    // These components are referenced here for easy of use
    // Data included in content packs
    public ContentData cd;
    // Data for the current quest
    public Quest quest;
    // Canvas for UI components (fixed on screen)
    public Canvas uICanvas;
    // Canvas for board tiles (tilted, in game space)
    public Canvas boardCanvas;
    // Canvas for board tokens (just above board tiles)
    public Canvas tokenCanvas;
    // Class for management of tokens on the board
    public TokenBoard tokenBoard;
    // Class for management of hero selection panel
    public HeroCanvas heroCanvas;
    // Class for management of monster selection panel
    public MonsterCanvas monsterCanvas;
    // Utility Class for UI scale and position
    public UIScaler uiScaler;
    // Class for Morale counter
    public MoraleDisplay moraleDisplay;
    // Class for quest editor management
    public QuestEditorData qed;
    // Class for gameType information (controls specific to a game type)
    public GameType gameType;
    // Class for camera management (zoom, scroll)
    public CameraController cc;
    // Class for managing user configuration
    public ConfigFile config;
    // Class for progress of activations, rounds
    public RoundController roundControl;
    // Class for stage control UI
    public NextStageButton stageUI;
    // Class log window
    public LogWindow logWindow;
    // Class for stage control UI
    public Audio audioControl;
    // Transparecny value for non selected component in the editor
    public float editorTransparency;
    // Quest started as test from editor
    public bool testMode = false;
    // Stats manager for quest rating
    public StatsManager stats;
    // Quests manager
    public QuestsManager questsList;

    // List of things that want to know if the mouse is clicked
    protected List<IUpdateListener> updateList;

    // Import thread
    public GameSelectionScreen gameSelect;

    // List of quests window
    public GameObject go_questSelectionScreen = null;
    public QuestSelectionScreen questSelectionScreen = null;

    // Current language
    public string currentLang;

    // Set when in quest editor
    public bool editMode = false;

    // Debug option
    public bool debugTests = false;

    // main thread Id
    public System.Threading.Thread mainThread = null;

    // This is used all over the place to find the game object.  Game then provides acces to common objects
    public static Game Get()
    {
        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
        return game;
    }

    // Unity fires off this function
    private void Awake()
    {
        // save main thread Id
        mainThread = System.Threading.Thread.CurrentThread;

        // Set specific configuration for Android 
        if (Application.platform == RuntimePlatform.Android)
        {
            // activate crashlytics
            DebugManager.Enable();

            // deactivate screen timeount while in Valkyrie
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        // Find the common objects we use.  These are created by unity.
        cc = GameObject.FindObjectOfType<CameraController>();
        uICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        boardCanvas = GameObject.Find("BoardCanvas").GetComponent<Canvas>();
        tokenCanvas = GameObject.Find("TokenCanvas").GetComponent<Canvas>();
        tokenBoard = GameObject.FindObjectOfType<TokenBoard>();
        heroCanvas = GameObject.FindObjectOfType<HeroCanvas>();
        monsterCanvas = GameObject.FindObjectOfType<MonsterCanvas>();

        // Create some things
        uiScaler = new UIScaler(uICanvas);
        config = new ConfigFile();
        GameObject go = new GameObject("audio");
        audioControl = go.AddComponent<Audio>();
        updateList = new List<IUpdateListener>();
        stats = new StatsManager();
        stats.DownloadStats();

        if (config.data.Get("UserConfig") == null)
        {
            // English is the default current language
            config.data.Add("UserConfig", "currentLang", "English");
            config.Save();
        }
        currentLang = config.data.Get("UserConfig", "currentLang");

        string vSet = config.data.Get("UserConfig", "editorTransparency");
        if (vSet == "")
        {
            editorTransparency = 0.3f;
        }
        else
        {
            float.TryParse(vSet, out editorTransparency);
        }

        string s_debug_tests = config.data.Get("Debug", "tests");
        if (s_debug_tests != "")
        {
            s_debug_tests = s_debug_tests.ToLower();
            if (s_debug_tests == "true" || s_debug_tests == "1")
            {
                debugTests = true;
            }
        }

        // On android extract streaming assets for use
        if (Application.platform == RuntimePlatform.Android)
        {
            System.IO.Directory.CreateDirectory(ContentData.ContentPath());
            using (ZipFile jar = ZipFile.Read(Application.dataPath))
            {
                foreach (ZipEntry e in jar)
                {
                    if (!e.FileName.StartsWith("assets"))
                    {
                        continue;
                    }

                    if (e.FileName.StartsWith("assets/bin"))
                    {
                        continue;
                    }

                    e.Extract(ContentData.ContentPath() + "../..", ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        DictionaryI18n valDict = new DictionaryI18n();
        foreach (string file in System.IO.Directory.GetFiles(ContentData.ContentPath() + "../text", "Localization*.txt"))
        {
            valDict.AddDataFromFile(file);
        }
        LocalizationRead.AddDictionary("val", valDict);

        roundControl = new RoundController();

        // Read the version and add it to the log
        TextAsset versionFile = Resources.Load("version") as TextAsset;
        version = versionFile.text.Trim();
        // The newline at the end stops the stack trace appearing in the log
        ValkyrieDebug.Log("Valkyrie Version: " + version + System.Environment.NewLine);

#if UNITY_STANDALONE_WIN
        SetScreenOrientationToLandscape();
#endif

        // Bring up the Game selector
        gameSelect = new GameSelectionScreen();
    }

    // This is called by 'start quest' on the main menu
    public void SelectQuest()
    {
        Dictionary<string, string> packs = config.data.Get(gameType.TypeName() + "Packs");
        if (packs != null)
        {
            foreach (KeyValuePair<string, string> kv in packs)
            {
                cd.LoadContentID(kv.Key);
            }
        }

        // Pull up the quest selection page
        if (questSelectionScreen == null)
        {
            go_questSelectionScreen = new GameObject("QuestSelectionScreen");
            questSelectionScreen = go_questSelectionScreen.AddComponent<QuestSelectionScreen>();
        }
        else
        {
            questSelectionScreen.Show();
        }
    }

    // This is called by editor on the main menu
    public void SelectEditQuest()
    {
        // We load all packs for the editor, not just those selected
        foreach (string pack in cd.GetPacks())
        {
            cd.LoadContent(pack);
        }

        // Pull up the quest selection page
        new QuestEditSelection();
    }

    // This is called when a quest is selected
    public void StartQuest(QuestData.Quest q)
    {
        if (Path.GetExtension(Path.GetFileName(q.path)) == ".valkyrie")
        {
            // extract the full package
            QuestLoader.ExtractSinglePackageFull(ContentData.DownloadPath() + Path.DirectorySeparatorChar + Path.GetFileName(q.path));
        }

        Texture2D[] arrow = ArrowMeasures();
        float[] values = ObtainValues();

        // Fetch all of the quest data and initialise the quest
        quest = new Quest(q);
        heroCanvas.heroSelection = new HeroSelection();

        UIElement ui = new UIElement();
        ui.SetLocation(values[0], values[1], values[2], values[3]);
        ui.SetImage(arrow[0]);

        values[0] = GameUtils.ReturnValueGameType<float>(UIScaler.GetRight(-6), UIScaler.GetRight(-7), UIScaler.GetRight(-6));

        ui = new UIElement();
        ui.SetLocation(values[0], values[1], values[2], values[3]);
        ui.SetImage(arrow[1]);

        ui = new UIElement();

        if (GameUtils.IsMoMGameType())
        {
            ui.SetLocation(UIScaler.GetHCenter(-UIScaler.GetRelWidth(2.4f)), UIScaler.GetBottom(-UIScaler.GetRelHeight(4)), UIScaler.GetRelWidth(1.2f), UIScaler.GetRelHeight(4));
            ui.SetImage(CommonImageKeys.mom_heroTry);

        }
        else if (GameUtils.IsD2EGameType())
        {
            ui.SetLocation(UIScaler.GetHCenter(-UIScaler.GetRelWidth(3.3f)), UIScaler.GetBottom(-UIScaler.GetRelHeight(4.7f)), UIScaler.GetRelWidth(1.9f), UIScaler.GetRelHeight(3.65f));
            ui.SetImage(CommonImageKeys.d2e_bar_heroTry);

        }
        else if (GameUtils.IsIAGameType())
        {
            ui.SetLocation(0, UIScaler.GetBottom(-UIScaler.GetRelHeight(5f)), UIScaler.GetWidthUnits(), UIScaler.GetRelHeight(5.2f));
            ui.SetImage(CommonImageKeys.ia_bgnd_Heroes_down);
        }
        ui.GetTransform().SetAsLastSibling();

        // Draw the hero icons, which are buttons for selection
        heroCanvas.SetupUI(ui);

        ui = new UIElement(Game.HEROSELECT);
        ui.SetText(new StringKey("val", "SELECT", gameType.HeroesName()), GameUtils.ReturnValueGameType<Color>(Color.black, Color.white, Color.white));
        ui.SetLocation(UIScaler.GetRelWidth(4), GameUtils.ReturnValueGameType<float>(.2f, .6f, 1f), UIScaler.GetRelWidth(2), 4);

        ui.SetFont(game.gameType.GetHeaderFont());
        ui.SetFontSize(UIScaler.GetMediumFont());
        if (GameUtils.IsIAGameType())
            ui.SetFontSize(UIScaler.GetBigFont());
        else
            ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);
        new UITitleBackGround(ui, CommonString.title);

        if (GameUtils.IsMoMGameType())
        {
            // Add a finished button to start the quest
            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetRight(-UIScaler.GetRelWidth(5)), UIScaler.GetBottom(-6.5f), 7.5f, 5f);
            ui.SetText(CommonStringKeys.OBT_OBJECTS, new Color(0.439f, 0.366f, 0.209f));
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetTextAlignment(TextAnchor.MiddleCenter);
            ui.SetButton(EndSelection);

            Texture2D texture = CommonScriptFuntions.RotateImage(CommonImageKeys.mom_btn_chr_menu, 180);
            ui.SetImage(texture);

            // Add a finished button to return the quest

            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetLeft(UIScaler.GetRelWidth(16)), UIScaler.GetBottom(-6.5f), 7.5f, 5f);
            ui.SetText(CommonStringKeys.RET_SELECT_QUEST, new Color(0.439f, 0.366f, 0.209f));
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetButton(Destroyer.QuestSelect);
            ui.SetImage(CommonImageKeys.mom_btn_chr_menu);
            ui.SetTextAlignment(TextAnchor.MiddleCenter);
        }
        else if (GameUtils.IsD2EGameType())
        {
            // Add a finished button to start the quest
            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetRight(-UIScaler.GetRelWidth(4)), UIScaler.GetBottom(-3.5f), 7.5f, 3f);
            ui.SetText(CommonStringKeys.CONFIRM, Color.green);
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetImage(CommonImageKeys.d2e_btn_green);
            ui.SetButton(EndSelection);

            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetLeft(UIScaler.GetRelWidth(16)), UIScaler.GetBottom(-3.5f), 7.5f, 3f);
            ui.SetText(CommonStringKeys.CANCEL, Color.red);
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetImage(CommonImageKeys.d2e_btn_red);
            ui.SetButton(Destroyer.QuestSelect);
        }
        else if (GameUtils.IsIAGameType())
        {
            // Add a finished button to start the quest
            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetRight(-UIScaler.GetRelWidth(7)), UIScaler.GetBottom(-2.2f), 6.5f, 2.2f);
            ui.SetText(CommonStringKeys.CONTINUE);
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetImage(CommonImageKeys.ia_btn_menu);
            ui.SetButton(EndSelection);

            ui = new UIElement(Game.HEROSELECT);
            ui.SetLocation(UIScaler.GetLeft(UIScaler.GetRelWidth(29)), UIScaler.GetBottom(-2.2f), 6.5f, 2.2f);
            ui.SetText(CommonStringKeys.RETURN);
            ui.SetFont(gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetSemiSmallFont());
            ui.SetImage(CommonImageKeys.ia_btn_menu);
            ui.SetButton(Destroyer.QuestSelect);
        }
    }

    private static float[] ObtainValues()
    {
        return new float[] {
            GameUtils.ReturnValueGameType<float>(UIScaler.GetLeft(3.5f), UIScaler.GetLeft(2f), UIScaler.GetLeft(3.5f)),
            GameUtils.ReturnValueGameType<float>(UIScaler.GetRelHeight(2) - 6, UIScaler.GetRelHeight(2) - 5, UIScaler.GetRelHeight(2) - 6),
            GameUtils.ReturnValueGameType<float>(UIScaler.GetRelWidth(15), UIScaler.GetRelWidth(10), UIScaler.GetRelWidth(16)),
            GameUtils.ReturnValueGameType<float>(UIScaler.GetRelHeight(3), UIScaler.GetRelHeight(5), UIScaler.GetRelHeight(4))
        };
    }

    private static Texture2D[] ArrowMeasures()
    {
        return new Texture2D[] {
            GameUtils.ReturnValueGameType<Texture2D>(
                        CommonImageKeys.mom_btn_arrow_gold,
                        CommonScriptFuntions.RotateImage(CommonImageKeys.d2e_btn_arrow_blue, -90),
                         CommonImageKeys.ia_button_arrow_inv
                    ),
            GameUtils.ReturnValueGameType<Texture2D>(
                        CommonScriptFuntions.RotateImage(CommonImageKeys.mom_btn_arrow_gold, 180),
                        CommonScriptFuntions.RotateImage(CommonImageKeys.d2e_btn_arrow_blue, 90),
                        CommonImageKeys.ia_button_arrow
                    ),
        };
    }

    // HeroCanvas validates selection and starts quest if everything is good
    public void EndSelection()
    {
        // Count up how many heros have been selected
        int count = 0;
        foreach (Quest.Hero h in Game.Get().quest.heroes)
        {
            if (h.heroData != null)
            {
                count++;
            }
        }
        // Starting morale is number of heros
        quest.vars.SetValue("$%morale", count);
        // This validates the selection then if OK starts first quest event
        heroCanvas.EndSection();
    }

    public void QuestStartEvent()
    {
        // Start quest music
        List<string> music = new List<string>();
        foreach (AudioData ad in cd.audio.Values)
        {
            if (ad.ContainsTrait("quest"))
            {
                music.Add(ad.file);
            }
        }
        audioControl.PlayDefaultQuestMusic(music);

        Destroyer.Dialog();
        // Create the menu button
        new MenuButton();
        new LogButton();
        new SkillButton();
        new InventoryButton();
        // Draw next stage button if required
        stageUI = new NextStageButton();

        // Start round events
        quest.eManager.EventTriggerType("StartRound", false);
        // Start the quest (top of stack)
        quest.eManager.EventTriggerType("EventStart", false);
        quest.eManager.TriggerEvent();
    }

    // On quitting
    private void OnApplicationQuit()
    {
        // This exists for the editor, because quitting doesn't actually work.
        Destroyer.Destroy();
        // Clean up temporary files
        QuestLoader.CleanTemp();
    }

    //  This is here because the editor doesn't get an update, so we are passing through mouse clicks to the editor
    private void Update()
    {
        updateList.RemoveAll(delegate (IUpdateListener o) { return o == null; });
        for (int i = 0; i < updateList.Count; i++)
        {
            if (!updateList[i].Update())
            {
                updateList[i] = null;
            }
        }
        updateList.RemoveAll(delegate (IUpdateListener o) { return o == null; });

        if (Input.GetMouseButtonDown(0))
        {
            foreach (IUpdateListener iul in updateList)
            {
                iul.Click();
            }
        }
        // 0 is the left mouse button
        if (qed != null && Input.GetMouseButtonDown(0))
        {
            qed.MouseDown();
        }

        // 0 is the left mouse button
        if (qed != null && Input.GetMouseButtonDown(1))
        {
            qed.RightClick();
        }

        if (quest != null)
        {
            quest.Update();
        }

        if (Input.GetKey("right alt") || Input.GetKey("left alt"))
        {
            if (Input.GetKeyDown("d") && logWindow != null)
            {
                logWindow.Update(true);
            }
        }

        if (gameSelect != null)
        {
            gameSelect.Update();
        }
    }

    public static string AppData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string appData = Path.Combine(Android.GetStorage(), "Valkyrie");
            if (appData != null)
            {
                return appData;
            }
        }
        return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Valkyrie");
    }

    public void AddUpdateListener(IUpdateListener obj)
    {
        updateList.Add(obj);
    }

#if UNITY_STANDALONE_WIN
    [System.Runtime.InteropServices.DllImport("User32.dll")]
    private static extern bool SetDisplayAutoRotationPreferences(int value);

    private static void SetScreenOrientationToLandscape()
    {
        try
        {
            SetDisplayAutoRotationPreferences((int)ORIENTATION_PREFERENCE.ORIENTATION_PREFERENCE_LANDSCAPE |
                (int)ORIENTATION_PREFERENCE.ORIENTATION_PREFERENCE_LANDSCAPE_FLIPPED);
        }

        catch (System.EntryPointNotFoundException e)
        {
            Debug.Log("Exception triggered and caught :" + e.GetType().Name);
            Debug.Log("message :" + e.Message);
        }
    }

    private enum ORIENTATION_PREFERENCE
    {
        ORIENTATION_PREFERENCE_NONE = 0x0,
        ORIENTATION_PREFERENCE_LANDSCAPE = 0x1,
        ORIENTATION_PREFERENCE_PORTRAIT = 0x2,
        ORIENTATION_PREFERENCE_LANDSCAPE_FLIPPED = 0x4,
        ORIENTATION_PREFERENCE_PORTRAIT_FLIPPED = 0x8
    }
#endif

}

public interface IUpdateListener
{
    /// <summary>
    /// This method is called on click
    /// </summary>
    void Click();

    /// <summary>
    /// This method is called on Unity Update.  Must return false to allow garbage collection.
    /// </summary>
    /// <returns>True to keep this in the update list, false to remove.</returns>
    bool Update();
}


public class GameUtils
{
    public static T ReturnValueGameType<T>(T mom, T d2e, T ia)
    {
        if (IsMoMGameType())
            return mom;
        else if (IsD2EGameType())
            return d2e;
        else if (IsIAGameType())
            return ia;
        else
            return default(T);
    }

    public static bool IsD2EGameType()
    {
        return Game.Get().gameType is D2EGameType;
    }

    public static bool IsMoMGameType()
    {
        return Game.Get().gameType is MoMGameType;
    }
    public static bool IsIAGameType()
    {
        return Game.Get().gameType is IAGameType;
    }

    public static string GetImagePath(string img)
    {
        Dictionary<string, ImageData> images = Game.Get().cd.images;
        if (images.ContainsKey(img))
            return images[img].image;
        else
            return "";
    }

}
