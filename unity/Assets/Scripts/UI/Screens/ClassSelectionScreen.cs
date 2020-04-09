using Assets.Scripts.Content;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Screens
{
    internal class ClassSelectionScreen
    {
        protected List<float> scrollOffset = new List<float>();
        protected List<UIElementScrollVertical> scrollArea = new List<UIElementScrollVertical>();
        public ClassSelectionScreen()
        {
            Draw();
        }

        public void Draw()
        {
            // Clean up
            Destroyer.Dialog();
            foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.HEROSELECT))
            {
                Object.Destroy(go);
            }

            Game game = Game.Get();

            // Heading
            UIElement screenUI = new UIElement(null, "Screen_bg");
            screenUI.SetLocation(0, 0, UIScaler.GetWidthUnits(), UIScaler.GetHeightUnits());
            screenUI.SetBGColor(Color.clear);

            // Add a title to the page
            UIElement ui = new UIElement(screenUI.GetTransform(),Game.HEROSELECT, "Tittle");
            ui.SetLocation(8, 1, UIScaler.GetWidthUnits() - 16, 3);
            ui.SetText(new StringKey("val", "SELECT_CLASS"));
            ui.SetFont(Game.Get().gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            new UITitleBackGround(ui, CommonString.title);

            // Get all heros
            int heroCount = 0;
            // Count number of selected heroes
            foreach (Quest.Hero h in game.quest.heroes)
            {
                if (h.heroData != null)
                    heroCount++;
            }

            float xOffset = UIScaler.GetHCenter(-22);

            if (heroCount < 4)
            {
                xOffset += 5f;
            }

            if (heroCount < 3)
            {
                xOffset += 5f;
            }

            for (int i = 0; i < heroCount; i++)
            {
                DrawHero(xOffset, i, screenUI);
                xOffset += 11f;
            }

            // Add a finished button to start the quest

            ui = new UIElement(screenUI.GetTransform(), Game.HEROSELECT, "Btn_back");
            ui.SetLocation(0.5f, UIScaler.GetBottom(-3.5f), 8, GameUtils.ReturnValueGameType<float>(2, 2.5f,2));
            ui.SetText(CommonStringKeys.BACK, Color.red);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Destroyer.QuestSelect);
            ui.SetImage(CommonImageKeys.d2e_btn_red);

            ui = new UIElement(screenUI.GetTransform(), Game.HEROSELECT, "Btn_finished");
            ui.SetLocation(UIScaler.GetRight(-8.5f), UIScaler.GetBottom(-3.5f), 8, 2.5f);
            ui.SetText(CommonStringKeys.FINISHED, Color.green);
            ui.SetFont(game.gameType.GetHeaderFont());
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Finished);
            ui.SetImage(CommonImageKeys.d2e_btn_green);
        }

        public void DrawHero(float xOffset, int hero, UIElement parent)
        {
            Game game = Game.Get();

            if (scrollOffset.Count > hero)
            {
                scrollOffset[hero] = scrollArea[hero].GetScrollPosition();
            }

            string archetype = game.quest.heroes[hero].heroData.archetype;
            string hybridClass = game.quest.heroes[hero].hybridClass;
            float yStart = 15f;

            UIElement ui = null;
            if (hybridClass.Length > 0)
            {
                archetype = game.cd.classes[hybridClass].hybridArchetype;
                ui = new UIElement(parent.GetTransform(), Game.HEROSELECT, "Hero_border_" + hero);
                ui.SetLocation(xOffset + 0.25f, yStart, 10.5f, 5);
                new UIElementBorder(ui);

                ui = new UIElement(parent.GetTransform(), Game.HEROSELECT, "Hero_text_" + hero);
                ui.SetLocation(xOffset + 1, yStart + 0.5f, 7, 4);
                ui.SetText(game.cd.classes[hybridClass].name, Color.black);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { Select(hero, hybridClass); });
                ui.SetImage(CommonImageKeys.d2e_btn_white);
                yStart += 5;
            }

            while (scrollArea.Count <= hero)
            {
                scrollArea.Add(null);
            }
            scrollArea[hero] = new UIElementScrollVertical(parent.GetTransform(), Game.HEROSELECT, "Hero_scroll_" + hero);
            scrollArea[hero].SetImage(CommonImageKeys.d2e_bgnd_pergam_gold);
            scrollArea[hero].SetLocation(xOffset + 1.7f, yStart, 9.5f, 25.5f - yStart);

            float yOffset = 1;

            foreach (ClassData cd in game.cd.classes.Values)
            {
                if (!cd.archetype.Equals(archetype))
                    continue;
                if (cd.hybridArchetype.Length > 0 && hybridClass.Length > 0)
                    continue;

                string className = cd.sectionName;
                bool available = true;
                bool pick = false;

                for (int i = 0; i < game.quest.heroes.Count; i++)
                {
                    if (game.quest.heroes[i].className.Equals(className))
                    {
                        available = false;
                         pick = (hero == i);
                    }
                    if (game.quest.heroes[i].hybridClass.Equals(className))
                        available = false;
                }

                ui = new UIElement(scrollArea[hero].GetScrollTransform(), Game.HEROSELECT, "Hero_name_" + hero);
                ui.SetLocation(.65f, yOffset, 7.8f, 3.4f);
                ui.SetText(cd.name, Color.black);
                if (available)
                {
                    ui.SetImage(CommonImageKeys.d2e_btn_white);
                    ui.SetButton(delegate { Select(hero, className); });
                }
                else
                {
                    ui.SetImage(CommonImageKeys.d2e_btn_grey);
                    if (pick)
                    {
                        ui.SetText(cd.name, Color.white);
                        ui.SetImage(CommonImageKeys.d2e_btn_blue);
                    }
                }   
                ui.SetFontSize(UIScaler.GetSemiSmallFont());
                yOffset += 5f;
            }

            scrollArea[hero].SetScrollSize(yOffset);
            if (scrollOffset.Count > hero)
            {
                scrollArea[hero].SetScrollPosition(scrollOffset[hero]);
            }
            else
            {
                scrollOffset.Add(0);
            }

            Texture2D heroTex = ContentData.FileToTexture(game.quest.heroes[hero].heroData.image);
            UIElement imageUI = new UIElement(parent.GetTransform(), Game.HEROSELECT, "Hero_image_" + hero);
            imageUI.SetLocation(xOffset + 2.5f, 5f, 7.8f, 7.8f);
            imageUI.SetImage(heroTex);
            new UICharacterBorders(imageUI, game.quest.heroes[hero].heroData.name);

            UIElement barUI = new UIElement(parent.GetTransform(), Game.HEROSELECT, "Hero_bar_image_" + hero);
            barUI.SetLocation(xOffset + 1.4f, 25f, 10f, .86f);
            barUI.SetImage(CommonImageKeys.d2e_bar_gold);
        }

        public void Select(int hero, string className)
        {
            Game game = Game.Get();
            if (game.cd.classes[className].hybridArchetype.Length > 0)
            {
                game.quest.heroes[hero].className = "";
                if (game.quest.heroes[hero].hybridClass.Length > 0)
                {
                    game.quest.heroes[hero].hybridClass = "";
                }
                else
                {
                    game.quest.heroes[hero].hybridClass = className;
                }
            }
            else
            {
                game.quest.heroes[hero].className = className;
            }
            Draw();
        }

        public void Finished()
        {
            Game game = Game.Get();

            HashSet<string> items = new HashSet<string>();
            foreach (Quest.Hero h in game.quest.heroes)
            {
                if (h.heroData == null)
                {
                    continue;
                }

                if (h.className.Length == 0)
                {
                    return;
                }

                game.quest.vars.SetValue("#" + h.className, 1);

                foreach (string s in game.cd.classes[h.className].items)
                {
                    items.Add(s);
                }

                if (h.hybridClass.Length == 0)
                {
                    continue;
                }

                foreach (string s in game.cd.classes[h.hybridClass].items)
                {
                    items.Add(s);
                }
            }
            game.quest.items.UnionWith(items);

            foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.HEROSELECT))
            {
                Object.Destroy(go);
            }

            game.moraleDisplay = new MoraleDisplay();
            game.QuestStartEvent();
        }
    }
}