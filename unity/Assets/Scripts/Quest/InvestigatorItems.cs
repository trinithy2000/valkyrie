using Assets.Scripts.Content;
using Assets.Scripts.UI;
using Assets.Scripts.UI.MOM;
using System.Collections.Generic;
using UnityEngine;

// Window with starting Investigator items
public class InvestigatorItems
{
    public InvestigatorItems()
    {
        Game game = Game.Get();

        // Items from heroes
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.heroData != null)
            {
                if (game.cd.items.ContainsKey(h.heroData.item))
                {
                    game.quest.items.Add(h.heroData.item);
                }
            }
        }

        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            QuestData.QItem item = kv.Value as QuestData.QItem;
            if (item != null && item.starting && game.quest.itemSelect.ContainsKey(kv.Key)
                && item.tests != null && game.quest.vars.Test(item.tests))
            {
                game.quest.items.Add(game.quest.itemSelect[kv.Key]);
                if (item.inspect.Length > 0)
                {
                    if (game.quest.itemInspect.ContainsKey(game.quest.itemSelect[kv.Key]))
                    {
                        game.quest.itemInspect.Remove(game.quest.itemSelect[kv.Key]);
                    }
                    game.quest.itemInspect.Add(game.quest.itemSelect[kv.Key], item.inspect);
                }
            }
        }

        // If a dialog window is open we force it closed (this shouldn't happen)
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.DIALOG))
        {
            Object.Destroy(go);
        }

        UIElement ui = new UIElement();

        ui.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
        ui.SetImage(CommonImageKeys.mom_bgnd_Investigator);
        ui.GetTransform().SetAsFirstSibling();

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-UIScaler.GetRelWidth(2.4f)), UIScaler.GetBottom(-UIScaler.GetRelHeight(4)), UIScaler.GetRelWidth(1.2f), UIScaler.GetRelHeight(4));
        ui.SetImage(CommonImageKeys.mom_heroTry);
        ui.GetTransform().SetAsLastSibling();


        ui = new UIElement();
        ui.SetLocation(10, 0.5f, UIScaler.GetWidthUnits() - 20, 4);
        ui.SetText(CommonStringKeys.TIT_SELECT_ITEMS);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetFont(Game.Get().gameType.GetHeaderFont());
        ui.SetBGColor(Color.clear);
        new UITitleBackGround(ui, CommonString.items);

        SortedList<string, SortedList<string, string>> itemSort = new SortedList<string, SortedList<string, string>>();

        foreach (string item in game.quest.items)
        {
            // Ignore "ItemX", find next capital letter
            int charIndex = 5;
            while (charIndex < item.Length - 1)
            {
                if (System.Char.IsUpper(item[charIndex++]))
                {
                    break;
                }
            }
            string typeString = item.Substring(0, charIndex);
            string translationString = game.cd.items[item].name.Translate();

            if (!itemSort.ContainsKey(typeString))
            {
                itemSort.Add(typeString, new SortedList<string, string>());
            }

            // Duplicate names
            while (itemSort[typeString].ContainsKey(translationString))
            {
                translationString += "D";
            }

            itemSort[typeString].Add(translationString, item);
        }

        int y = 0;
        int x = 0;
        foreach (string category in itemSort.Keys)
        {
            foreach (string item in itemSort[category].Values)
            {
                Texture2D tex = ContentData.FileToTexture(game.cd.items[item].image);
                Sprite sprite = null;
                if (tex != null)
                {
                    sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1, 0, SpriteMeshType.FullRect);
                }

                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(8f * x) - 19, 7f + (9f * y), 6, 6);
                if (sprite != null)
                {
                    ui.SetImage(sprite);
                }

                ui = new UIElement();
                ui.SetLocation(UIScaler.GetHCenter(8f * x) - 20, 14f + (9f * y), 8, 1);
                ui.SetText(game.cd.items[item].name);
                ui.SetBGColor(Color.clear);
                new UITitleBackGround_MOM(ui, CommonString.itemTitle);

                x++;
                if (x > 4)
                {
                    x = 0;
                    y++;
                }
            }
        }
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetRight(-10f), UIScaler.GetBottom(-6.5f), 7.5f, 5f);
        ui.SetText(CommonStringKeys.CONTINUE_PREPAR, new Color(0.439f, 0.366f, 0.209f));

        ui.SetFont(game.gameType.GetHeaderFont());
        ui.SetFontSize(UIScaler.GetSemiSmallFont());
        ui.SetButton(Destroyer.QuestSelect);
        ui.SetImage(CommonScriptFuntions.RotateImage(CommonImageKeys.mom_btn_chr_menu, 180));
        ui.SetTextAlignment(TextAnchor.MiddleCenter);
        ui.SetButton(game.QuestStartEvent);
    }
}
