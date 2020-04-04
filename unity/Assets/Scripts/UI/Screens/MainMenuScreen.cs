using Assets.Scripts.Content;
using System.Collections.Generic;
using UnityEngine;
using ValkyrieTools;

namespace Assets.Scripts.UI.Screens
{

    // Class for creation and management of the main menu
    public class MainMenuScreen
    {
        private static readonly StringKey SELECT_CONTENT = new StringKey("val", "SELECT_CONTENT");
        private static readonly StringKey ABOUT = new StringKey("val", "ABOUT");
        private static readonly StringKey OPTIONS = new StringKey("val", "OPTIONS");
        private static readonly StringKey ABOUT_FFG = new StringKey("val", "ABOUT_FFG");
        private static readonly StringKey ABOUT_LIBS = new StringKey("val", "ABOUT_LIBS");
        private static readonly StringKey START_QUEST = new StringKey("val", "START_QUEST");
        private static readonly StringKey LOAD_QUEST = new StringKey("val", "LOAD_QUEST");

        // Create a menu which will take up the whole screen and have options.  All items are dialog for destruction.
        public MainMenuScreen()
        {
            // This will destroy all, because we shouldn't have anything left at the main menu
            Destroyer.Destroy();
            Game game = Game.Get();

            List<string> music = new List<string>();
            foreach (AudioData ad in game.cd.audio.Values)
            {
                if (ad.ContainsTrait("menu"))
                {
                    music.Add(ad.file);
                }
            }
            game.audioControl.PlayDefaultQuestMusic(music);


            // Name.  Should this be the banner, or better to print Valkyrie with the game font?    

            UIElement ui = new UIElement();

            ui = new UIElement();
            ui.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());

            if (game.gameType is MoMGameType)
            {
                ui.SetImage(CommonImageKeys.mom_bgnd_mansion);
            }
            else if (game.gameType is D2EGameType)
            {
                ui.SetImage(CommonImageKeys.d2e_bgnd_sreen);
            }

            ui = new UIElement();
            ui.SetLocation(2.5f, 1, 11, 3);
            ui.SetText("Valkyrie");
            ui.SetBGColor(Color.clear);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetLargeFont());

            // Button for start quest/scenario
            ui = GetButtonMenu(game, 10, START_QUEST);
            ui.SetButton(Start);

            // Button for continue quest/scenario
            ui = GetButtonMenu(game, 12.7f, LOAD_QUEST);
            if (SaveManager.SaveExists())
            {
                ui.SetButton(delegate { new SaveSelectScreen(); });
            }
            else
            {
                ui.SetText(LOAD_QUEST, Color.grey);
            }

            // Content selection page
            ui = GetButtonMenu(game, 15.4f, SELECT_CONTENT);
            ui.SetButton(Content);

            // Quest/Scenario edito
            ui = GetButtonMenu(game, 18.1f, new StringKey("val", "QUEST_NAME_EDITOR", game.gameType.QuestName()));
            ui.SetButton(Editor);

            // About page (managed in this class)
            ui = GetButtonMenu(game, 20.8f, ABOUT);
            ui.SetButton(About);

            // Configuration menu
            ui = GetButtonMenu(game, 23.5f, OPTIONS);
            ui.SetButton(Config);

            // Exit Valkyrie
            ui = GetButtonMenu(game, 26.2f, CommonStringKeys.EXIT);
            ui.SetButton(Exit);
        }


        private UIElement GetButtonMenu(Game game, float position_Vert, StringKey action)
        {
            if (game.gameType is MoMGameType)
            {
                return GetButtonMenuMom(game, position_Vert, action);
            }
            else if (game.gameType is D2EGameType)
            {
                return GetButtonMenuD2E(game, position_Vert, action);
            }
            else
            {
                return null;
            }
        }

        private UIElement GetButtonMenuD2E(Game game, float position_Vert, StringKey action)
        {
            float width = (UIScaler.GetWidthUnits() - 13) / 16;

            UIElement ui = new UIElement();
            ui.SetLocation(width, position_Vert - 2, 14, 2.5f);
            ui.SetText(action);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            if (action.Equals(CommonStringKeys.EXIT))
            {
                ui.SetImage(CommonImageKeys.d2e_btn_menu_red);
            }
            else
            {
                ui.SetImage(CommonImageKeys.d2e_btn_menu_blue);
            }

            return ui;
        }

        private UIElement GetButtonMenuMom(Game game, float position_Vert, StringKey action)
        {
            float width = (UIScaler.GetWidthUnits() - 13) / 16;

            UIElement ui = new UIElement();

            ui.SetLocation(width, position_Vert, 13, 2.6f);
            ui.SetText(action);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetImage(CommonImageKeys.mom_btn_menu);
            return ui;
        }

        // Start quest
        public void Start()
        {
            ValkyrieDebug.Log("INFO: Accessing quests");

            Game game = Game.Get();

            // Remove the main menu
            Destroyer.Dialog();

            game.SelectQuest();
        }

        public void Content()
        {
            ValkyrieDebug.Log("INFO: Accessing content selection screen");

            new ContentSelectScreen();
        }

        public void Editor()
        {
            ValkyrieDebug.Log("INFO: Accessing content selection screen");

            Game game = Game.Get();
            game.SelectEditQuest();
        }

        private void Config()
        {
            ValkyrieDebug.Log("INFO: Accessing config");
            new OptionsScreen();
        }

        private static int click_counter = 0;
        public static void TestCrash()
        {
            click_counter++;

            if (click_counter >= 5)
            {
                DebugManager.Crash();
            }
        }

        // Create the about dialog
        public void About()
        {
            ValkyrieDebug.Log("INFO: Accessing about");

            // This will destroy all, because we shouldn't have anything left at the main menu
            Destroyer.Destroy();

            Sprite bannerSprite;
            Texture2D newTex = Resources.Load("sprites/banner") as Texture2D;

            GameObject banner = new GameObject("banner")
            {
                tag = Game.DIALOG
            };

            banner.transform.SetParent(Game.Get().uICanvas.transform);

            RectTransform trans = banner.AddComponent<RectTransform>();
            trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1 * UIScaler.GetPixelsPerUnit(), 7f * UIScaler.GetPixelsPerUnit());
            trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (UIScaler.GetWidthUnits() - 18f) * UIScaler.GetPixelsPerUnit() / 2f, 18f * UIScaler.GetPixelsPerUnit());
            banner.AddComponent<CanvasRenderer>();


            UnityEngine.UI.Image image = banner.AddComponent<UnityEngine.UI.Image>();
            bannerSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero, 1);
            image.sprite = bannerSprite;
            image.rectTransform.sizeDelta = new Vector2(18f * UIScaler.GetPixelsPerUnit(), 7f * UIScaler.GetPixelsPerUnit());

            UIElement ui = new UIElement();
            ui.SetLocation((UIScaler.GetWidthUnits() - 30f) / 2, 10, 30, 6);
            ui.SetText(ABOUT_FFG);
            ui.SetFontSize(UIScaler.GetMediumFont());

            ui = new UIElement();
            ui.SetLocation((UIScaler.GetWidthUnits() - 30f) / 2, 18, 30, 5);
            ui.SetText(ABOUT_LIBS);
            ui.SetFontSize(UIScaler.GetMediumFont());

            ui = new UIElement();
            ui.SetLocation(UIScaler.GetWidthUnits() - 5, UIScaler.GetBottom(-3), 5, 2);
            ui.SetText(Game.Get().version);
            ui.SetFontSize(UIScaler.GetMediumFont());

            ui = new UIElement();
            ui.SetLocation(1, UIScaler.GetBottom(-3), 8, 2);
            ui.SetText(CommonStringKeys.BACK);
            ui.SetFont(Game.Get().gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Destroyer.MainMenu);
            ui.SetBGColor(new Color(0, 0.03f, 0f));
            new UIElementBorder(ui);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}