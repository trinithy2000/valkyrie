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

            UIElement ui = new UIElement();
            ui.SetLocation(UIScaler.GetRelWidth(4), .2f, UIScaler.GetRelWidth(2), 4);
            ui.SetText(q.name, game.gameType is D2EGameType ? Color.white : Color.black);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetBGColor(Color.clear);
            new UITitleBackGround(ui, CommonString.title);

            // Draw Image
            ui = new UIElement();
            ui.SetLocation(UIScaler.GetHCenter(-19.8f), 5, 13f, 13);
            if (q.image.Length > 0)
            {
                ui.SetImage(ContentData.FileToTexture(Path.Combine(q.path, q.image)));
            }
            else
            {
                ui.SetImage(CommonImageKeys.default_img_quest);
            }

            new UITitleBackGround(ui, CommonString.image);

            // Draw Description
            ui = new UIElement();
            float height = UIScaler.GetHeightUnits() / 2.1f;
            UIElementScrollVertical scrollArea = new UIElementScrollVertical();
            scrollArea.SetLocation(UIScaler.GetHCenter(-4), UIScaler.GetTop(7), UIScaler.GetWidthUnits() / 2, height);

            ui = new UIElement(scrollArea.GetScrollTransform());
            ui.SetLocation(1, UIScaler.GetRelHeight(7), UIScaler.GetRelWidth(2.2f), height);
            ui.SetText(q.description, Color.black);
            ui.SetBGColor(Color.clear);
            scrollArea.SetScrollSize(height * 4);
            new UITitleBackGround(scrollArea, CommonString.description);

            // Draw authors
            ui = new UIElement();
            ui.SetLocation(UIScaler.GetHCenter(-20.15f), UIScaler.GetBottom(-10f), 14, 6.5f);
            ui.SetText(q.authors);
            new UIElementBorderDialog(ui, CommonString.dialogOne);

            // Difficulty
            if (q.difficulty != 0)
            {
                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(-13), 27, 11, 1);
                ui.SetText(new StringKey("val", "DIFFICULTY"));
                string symbol = "*";
                if (game.gameType is MoMGameType)
                {
                    symbol = new StringKey("val", "ICON_SUCCESS_RESULT").Translate();
                }
                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(-13), 28, 11, 2);
                ui.SetText(symbol + symbol + symbol + symbol + symbol);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(-10.95f) + (q.difficulty * 6.9f), 28, (1 - q.difficulty) * 6.9f, 2);
                ui.SetBGColor(new Color(0, 0, 0, 0.7f));
            }

            // Duration
            if (q.lengthMax != 0)
            {
                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(2), 27, 11, 1);
                ui.SetText(new StringKey("val", "DURATION"));

                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(2), 28, 4, 2);
                ui.SetText(q.lengthMin.ToString());
                ui.SetFontSize(UIScaler.GetMediumFont());

                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(6.5f), 28, 2, 2);
                ui.SetText("-");
                ui.SetFontSize(UIScaler.GetMediumFont());

                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(9), 28, 4, 2);
                ui.SetText(q.lengthMax.ToString());
                ui.SetFontSize(UIScaler.GetMediumFont());
            }

            // DELETE button (only for archive, directory might be edited by user)
            if (Path.GetExtension(Path.GetFileName(q.path)) == ".valkyrie")
            {
                ui = new UIElement();
                ui.SetLocation(UIScaler.GetRight(-8.5f), 0.5f, 8, GameUtils.ReturnValueGameType<float>(2, 2.5f));
                ui.SetText(CommonStringKeys.DELETE, Color.grey);
                ui.SetFont(game.gameType.GetHeaderFont());
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Delete(q); });
                ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_menu_blue));
            }

            ui = new UIElement();
            ui.SetLocation(0.5f, UIScaler.GetBottom(-2.5f), 8, GameUtils.ReturnValueGameType<float>(2, 2.5f));
            ui.SetText(CommonStringKeys.BACK, Color.red);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Cancel);
            ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_red));

            ui = new UIElement();
            ui.SetLocation(UIScaler.GetRight(-8.5f), UIScaler.GetBottom(-2.5f), 8, GameUtils.ReturnValueGameType<float>(2, 2.5f));
            ui.SetText(new StringKey("val", "START"), Color.green);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(delegate { Start(q); });
            ui.SetImage(GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_btn_menu, CommonImageKeys.d2e_btn_green));

        }

        /// <summary>
        /// Select to delete
        /// </summary>
        /// <param file="file">File name to delete</param>
        public void Delete(QuestData.Quest q)
        {
            ValkyrieDebug.Log("INFO: Delete quest");

            string toDelete = "";

            if (Path.GetExtension(Path.GetFileName(q.path)) == ".valkyrie")
            {
                toDelete = ContentData.DownloadPath() + Path.DirectorySeparatorChar + Path.GetFileName(q.path);
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
