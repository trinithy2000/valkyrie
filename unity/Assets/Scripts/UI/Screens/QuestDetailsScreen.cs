using Assets.Scripts.Content;
using System.IO;
using UnityEngine;
using ValkyrieTools;

namespace Assets.Scripts.UI.Screens
{
    public class QuestDetailsScreen
    {
        public QuestDetailsScreen(QuestData.Quest q)
        {

            Game game = Game.Get();
            LocalizationRead.AddDictionary("qst", q.localizationDict);
            // If a dialog window is open we force it closed (this shouldn't happen)
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.DIALOG))
            {
                Object.Destroy(go);
            }

            // Heading
            UIElement screenUI = new UIElement(null, "Screen_bg");
            screenUI.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
            screenUI.SetBGColor(Color.clear);

            UIElement ui = new UIElement(screenUI.GetRectTransform(), "Screen_tittle");
            ui.SetLocation(UIScaler.GetRelWidth(4), .2f, UIScaler.GetRelWidth(2), 4);
            ui.SetText(q.name, game.gameType is D2EGameType ? Color.white : Color.black);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetBGColor(Color.clear);
            new UITitleBackGround(ui, CommonString.title);

            // Draw Image
            ui = new UIElement(screenUI.GetRectTransform(), "Mission_img");
            ui.SetLocation(UIScaler.GetHCenter(-20.8f), 4.75f, 13f, 13);
            ui.SetImage(q.image.Length > 0 ? ContentData.FileToTexture(Path.Combine(q.path, q.image)) :
                CommonImageKeys.default_img_quest);
            new UITitleBackGround(ui, CommonString.image);

            // Draw Description
            float height = UIScaler.GetHeightUnits() / 2.1f;
            UIElementScrollVertical scrollArea = new UIElementScrollVertical(screenUI.GetRectTransform(), "Mission_text");
            scrollArea.SetLocation(UIScaler.GetHCenter(-4.85f), UIScaler.GetTop(5.65f), UIScaler.GetRelWidth(1.8f), height);
            //---------  
            UIElement scrollUI = new UIElement(scrollArea.GetScrollTransform(), "Mission_text");
            scrollUI.SetLocation(1, UIScaler.GetRelHeight(7), UIScaler.GetRelWidth(2f), height);
            scrollUI.SetText(q.description, Color.black);
            scrollUI.SetBGColor(Color.clear);
            scrollArea.SetScrollSize(height * 4);
            new UITitleBackGround(scrollArea, CommonString.description);

            // Draw authors
            UIElementScrollVertical scrollAutor = new UIElementScrollVertical(screenUI.GetRectTransform(), "Authors");
            scrollAutor.SetLocation(UIScaler.GetHCenter(-21.15f), UIScaler.GetBottom(-11), 14, 2.5f);
            //---------    
            UIElement scrollAutorUI = new UIElement(scrollAutor.GetScrollTransform(), "Authors_text");
            scrollAutorUI.SetLocation(1, 0, 14, 5.5f);
            scrollAutorUI.SetText(q.authors, Color.white);
            scrollAutorUI.SetBGColor(Color.clear);
            scrollAutor.SetScrollSize(6.5f * 1.5f);
            new UITitleBackGround(scrollAutor, CommonString.none);
            //     new UIElementBorderDialog(scrollAutor, CommonString.dialogOne);


            UIElement dif_dur_ui = new UIElement(screenUI.GetRectTransform(), "dif_bar");
            dif_dur_ui.SetLocation(UIScaler.GetHCenter(-15), 27, 30, 3);
            dif_dur_ui.SetBGColor(Color.green);

            // Difficulty
            if (q.difficulty != 0)
            {
                ui = new UIElement(dif_dur_ui.GetRectTransform(), "difficulty-text");
                ui.SetLocation(1, 0, 13, 1);
                ui.SetText(new StringKey("val", "DIFFICULTY"));
                string symbol = "*";
                if (game.gameType is MoMGameType)
                {
                    symbol = new StringKey("val", "ICON_SUCCESS_RESULT").Translate();
                }
                ui = new UIElement(dif_dur_ui.GetRectTransform(), "difficulty-symbols");
                ui.SetLocation(1, 1, 13, 2);
                ui.SetText(symbol + symbol + symbol + symbol + symbol);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui = new UIElement(dif_dur_ui.GetRectTransform(), "difficulty-shade");
                ui.SetLocation(1 + (q.difficulty * 12f), 1, (1 - q.difficulty) * 12f, 2);
                ui.SetBGColor(new Color(0, 0, 0, 0.7f));
            }

            // Duration
            if (q.lengthMax != 0)
            {
                ui = new UIElement(dif_dur_ui.GetRectTransform(), "duration-text");
                ui.SetLocation(16, 0, 13, 1);
                ui.SetText(new StringKey("val", "DURATION"));

                ui = new UIElement(dif_dur_ui.GetRectTransform(), "duration-min");
                ui.SetLocation(16, 1, 5, 2);
                ui.SetText(q.lengthMin.ToString());
                ui.SetFontSize(UIScaler.GetMediumFont());

                ui = new UIElement(dif_dur_ui.GetRectTransform(), "duration-colon");
                ui.SetLocation(21, 1, 4, 2);
                ui.SetText("-");
                ui.SetFontSize(UIScaler.GetMediumFont());

                ui = new UIElement(dif_dur_ui.GetRectTransform(), "duration-max");
                ui.SetLocation(25, 1, 5, 2);
                ui.SetText(q.lengthMax.ToString());
                ui.SetFontSize(UIScaler.GetMediumFont());
            }

            // DELETE button (only for archive, directory might be edited by user)
            if (Path.GetExtension(Path.GetFileName(q.path)) == ".valkyrie")
            {
                ui = new UIElement(screenUI.GetRectTransform(), "delete_btn");
                ui.SetLocation(UIScaler.GetRight(-8.5f), 0.5f, 8, GameUtils.ReturnValueGameType<float>(2, 2.5f,2));
                ui.SetText(CommonStringKeys.DELETE, Color.grey);
                ui.SetFont(game.gameType.GetHeaderFont());
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Delete(q); });
                ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_menu_blue, CommonImageKeys.ia_btn_menu));
            }

            ui = new UIElement(screenUI.GetRectTransform(), "back_btn");
            ui.SetLocation(0.5f, UIScaler.GetBottom(-2.5f), 8, GameUtils.ReturnValueGameType<float>(2, 2.5f,2));
            ui.SetText(CommonStringKeys.BACK, Color.red);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Cancel);
            ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_red, CommonImageKeys.ia_btn_menu));

            ui = new UIElement(screenUI.GetRectTransform(), "start_btn");
            ui.SetLocation(UIScaler.GetRight(-8.5f), UIScaler.GetBottom(-2.5f), 8, GameUtils.ReturnValueGameType<float>(2, 2.5f,2));
            ui.SetText(new StringKey("val", "START"), Color.green);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(delegate { Start(q); });
            ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_green, CommonImageKeys.ia_btn_menu));

        }

        /// <summary>
        /// Select to delete
        /// </summary>
        /// <param file="file">File name to delete</param>
        public void Delete(QuestData.Quest q)
        {
            ValkyrieDebug.Log("INFO: Delete quest");

            if (Path.GetExtension(Path.GetFileName(q.path)) == ".valkyrie")
            {
                string toDelete = ContentData.DownloadPath() + Path.DirectorySeparatorChar + Path.GetFileName(q.path);
                File.Delete(toDelete);

                // update quest status : downloaded/updated
                Game.Get().questsList.SetQuestAvailability(Path.GetFileNameWithoutExtension(q.path), false);
            }
            else
            {
                // this is not an archive, it is a local quest within a directory
                Directory.Delete(q.path, true);

                Game.Get().questsList.UnloadLocalQuests();
            }

            Destroyer.Dialog();

            // Pull up the quest selection page
            Game.Get().questSelectionScreen.Show();
        }

        // Return to quest selection
        public void Cancel()
        {
            ValkyrieDebug.Log("INFO: Return to quest list from details screen");

            Destroyer.Dialog();

            // Pull up the quest selection page
            Game.Get().questSelectionScreen.Show();
        }

        // Select a quest
        public void Start(QuestData.Quest q)
        {
            ValkyrieDebug.Log("INFO: Start quest from details screen");

            Destroyer.Dialog();
            Game.Get().StartQuest(q);
        }
    }
}
