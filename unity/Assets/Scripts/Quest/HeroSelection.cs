using Assets.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;

// Hero selection options
// This comes up when selection a hero icon to pick hero
public class HeroSelection
{

    public Dictionary<string, List<UIElement>> buttons;

    // Create page of options
    public HeroSelection()
    {
        Draw();
    }

    public void Draw()
    {
        // Clean up
        Destroyer.Dialog();

        Game game = Game.Get();
        // Get all available heros
        List<string> heroList = new List<string>(game.cd.heroes.Keys);
        heroList.Sort();

        UIElement ui = new UIElement();

        if (game.gameType is MoMGameType)
        {
            ui.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
            ui.SetImage(CommonImageKeys.mom_bgnd_Investigator);
            ui.GetTransform().SetAsFirstSibling();
        }

        UIElementScrollVertical scrollArea;

        float offset = 0;
        bool up = true;
        buttons = new Dictionary<string, List<UIElement>>();
        ui = null;

        scrollArea = new UICharactersScroll(Game.HEROSELECT);
        scrollArea.SetLocation(UIScaler.GetHCenter(-UIScaler.GetRelWidth(2.6f)), 4.5f, UIScaler.GetRelWidth(1.3f), 42f);
        scrollArea.SetBGColor(Color.clear);

        foreach (string hero in heroList)
        {
            buttons.Add(hero, new List<UIElement>());
            // Should be game type specific
            Texture2D newTex = ContentData.FileToTexture(game.cd.heroes[hero].image);
            ui = new UIElement(Game.HEROSELECT, scrollArea.GetScrollTransform());

            float[] values =
            {
                GameUtils.ReturnValueGameType<float>(.2f,1f,.2f),
                GameUtils.ReturnValueGameType<float>(8,6.1f,8),
                GameUtils.ReturnValueGameType<float>(9,6.1f,9),
                GameUtils.ReturnValueGameType<float>(9.5f,10.5f,9.5f)
            };

            if (up)
                ui.SetLocation(1 + offset, values[0], values[1], values[2]);
            else
                ui.SetLocation(-3 + offset, values[3], values[1], values[2]);

            ui.SetBGColor(Color.clear);
            ui.SetImage(newTex);
            ui.SetButton(delegate { Select(hero); });
            buttons[hero].Add(ui);

            new UICharacterBorders(ui, game.cd.heroes[hero].name);

            up = !up;

            offset += GameUtils.ReturnValueGameType<float>(4f, 4.4f, 4f);

        }

        scrollArea.SetScrollSize(offset + 2);
    }

    public void Select(string name)
    {
        Game game = Game.Get();
        HeroData hData = null;
        foreach (KeyValuePair<string, HeroData> hd in game.cd.heroes)
        {
            if (hd.Value.sectionName.Equals(name))
            {
                hData = hd.Value;
                break;
            }
        }
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (hData == h.heroData)
            {
                return;
            }
        }
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.heroData == null)
            {
                foreach (UIElement ui in buttons[name])
                {
                    ui.SetBGColor(new Color(0.4f, 0.4f, 0.4f));
                }
                h.heroData = hData;
                game.heroCanvas.UpdateImages();
                return;
            }
        }
    }

    public void Update()
    {
        foreach (KeyValuePair<string, List<UIElement>> kv in buttons)
        {
            Color c = Color.white;
            foreach (Quest.Hero h in Game.Get().quest.heroes)
            {
                if (h.heroData != null && h.heroData.sectionName.Equals(kv.Key))
                {
                    c = new Color(0.4f, 0.4f, 0.4f);
                }
            }

            foreach (UIElement ui in kv.Value)
            {
                ui.SetBGColor(c);
            }
        }
    }
}
