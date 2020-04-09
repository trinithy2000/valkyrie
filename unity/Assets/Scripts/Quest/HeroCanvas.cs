using Assets.Scripts.UI.Screens;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using System.Diagnostics;

// This class is for drawing hero images on the screen
public class HeroCanvas : MonoBehaviour
{
    public float offset;
    public Dictionary<int, Image> icons;
    public Dictionary<int, Image> icon_frames;
    // This is assumed in a number of places
    public static float heroSize = 4.2f;
    public static float offsetStart = 3.75f;
    public HeroSelection heroSelection;

    // Called when a quest is started, draws to screen
    
    public void SetupUI(UIElement element = null)
    {

        icons = new Dictionary<int, Image>();
        icon_frames = new Dictionary<int, Image>();
        offset = offsetStart;
        Game game = Game.Get();
        var name = new StackFrame(1).GetMethod().Name;

        foreach (Quest.Hero h in game.quest.heroes)
        {
            if ("StartQuest".Equals(name))
                AddHeroHor(h, game, element);
            else if (GameUtils.IsD2EGameType())
                AddHeroVer(h, game, element);
            else if (GameUtils.IsMoMGameType())
                AddHeroHor(h, game, element);
            else
                AddHeroHor(h, game, element);
        }
    }

    // Called when existing quest, cleans up
    public void Clean()
    {
        icons = null;
        icon_frames = null;
        // Clean up everything marked as 'herodisplay'
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("herodisplay"))
        {
            Object.Destroy(go);
        }
    }

    private void AddHeroVer(Quest.Hero h, Game game, UIElement element = null)
    {
        Sprite heroSprite, frameSprite;
        Texture2D frameTex;
        string heroName;

        setImagesBorder(h, out frameTex, out heroName);

        GameObject heroFrame = new GameObject("heroFrame" + heroName);

        heroFrame.tag = "herodisplay";
        heroFrame.transform.SetParent(game.uICanvas.transform);
        RectTransform transFrame = heroFrame.AddComponent<RectTransform>();

        transFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (0.25f + offset) * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        transFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0.25f * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        heroFrame.AddComponent<CanvasRenderer>();

        Image imageFrame = heroFrame.AddComponent<Image>();
        icon_frames.Add(h.id, imageFrame);
        frameSprite = Sprite.Create(frameTex, new Rect(0, 0, frameTex.width, frameTex.height), Vector2.zero, 1);
        imageFrame.sprite = frameSprite;
        imageFrame.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());

        Button buttonFrame = heroFrame.AddComponent<Button>();
        buttonFrame.interactable = true;
        buttonFrame.onClick.AddListener(delegate { HeroDiag(h.id); });

        GameObject heroImg = new GameObject("heroImg" + heroName);
        heroImg.tag = "herodisplay";
        heroImg.transform.SetParent(game.uICanvas.transform);
        RectTransform trans = heroImg.AddComponent<RectTransform>();

        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (0.25f + offset) * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        if (game.quest.heroes.Count > 5)
        {
            offset += 22f / game.quest.heroes.Count;
        }
        else
        {
            offset += heroSize + 0.5f;
        }
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0.25f * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());

        heroImg.AddComponent<CanvasRenderer>();
        Image image = heroImg.AddComponent<Image>();

        icons.Add(h.id, image);
        image.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit() * 0.8f, heroSize * UIScaler.GetPixelsPerUnit() * 0.8f);
        if (game.gameType is MoMGameType)
        {
            image.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit() * 0.9f, heroSize * UIScaler.GetPixelsPerUnit() * 0.9f);
            heroFrame.transform.SetAsLastSibling();
        }
        image.color = Color.clear;

        Button button = heroImg.AddComponent<Button>();
        button.interactable = true;
        button.onClick.AddListener(delegate { HeroDiag(h.id); });

        // Add hero image if selected
        if (h.heroData != null)
        {
            Texture2D newTex = ContentData.FileToTexture(h.heroData.image);
            heroSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero, 1);
            image.sprite = heroSprite;
        }

        if (element != null)
        {
            heroFrame.transform.parent = element.GetRectTransform();
            heroImg.transform.parent = element.GetRectTransform();
        }
    }


    // Add a hero
    private void AddHeroHor(Quest.Hero h, Game game, UIElement element = null)
    {
        Sprite heroSprite, frameSprite;
        Texture2D frameTex;
        string heroName;

        setImagesBorder(h, out frameTex, out heroName);

        GameObject heroFrame = new GameObject("heroFrame" + heroName)
        {
            tag = "herodisplay"
        };
        heroFrame.transform.SetParent(game.uICanvas.transform);
        RectTransform transFrame = heroFrame.AddComponent<RectTransform>();

        transFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0.65f * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        transFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (10.5f + offset) * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        heroFrame.AddComponent<CanvasRenderer>();

        Image imageFrame = heroFrame.AddComponent<Image>();
        icon_frames.Add(h.id, imageFrame);
        frameSprite = Sprite.Create(frameTex, new Rect(0, 0, frameTex.width, frameTex.height), Vector2.zero, 1);
        imageFrame.sprite = frameSprite;
        imageFrame.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());

        Button buttonFrame = heroFrame.AddComponent<Button>();
        buttonFrame.interactable = true;
        buttonFrame.onClick.AddListener(delegate { HeroDiag(h.id); });

        GameObject heroImg = new GameObject("heroImg" + heroName)
        {
            tag = "herodisplay"
        };
        heroImg.transform.SetParent(game.uICanvas.transform);
        RectTransform trans = heroImg.AddComponent<RectTransform>();

        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0.65f * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (10.5f + offset) * UIScaler.GetPixelsPerUnit(), heroSize * UIScaler.GetPixelsPerUnit());

        if (game.quest.heroes.Count > 5)
        {
            offset += 26f / game.quest.heroes.Count;
        }
        else
        {
            offset += heroSize + 1f;
        }

        heroImg.AddComponent<CanvasRenderer>();
        Image image = heroImg.AddComponent<Image>();

        icons.Add(h.id, image);
                
        image.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit() * 0.85f, heroSize * UIScaler.GetPixelsPerUnit() * 0.85f);

        if (game.gameType is MoMGameType)
        {
            image.rectTransform.sizeDelta = new Vector2(heroSize * UIScaler.GetPixelsPerUnit() * 0.865f, heroSize * UIScaler.GetPixelsPerUnit() * 0.865f);
            heroFrame.transform.SetAsLastSibling();
        }
        image.color = Color.clear;

        Button button = heroImg.AddComponent<Button>();
        button.interactable = true;
        button.onClick.AddListener(delegate { HeroDiag(h.id); });

        // Add hero image if selected
        if (h.heroData != null)
        {
            Texture2D newTex = ContentData.FileToTexture(h.heroData.image);
            heroSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero, 1);
            image.sprite = heroSprite;
        }
        
        if (element != null)
        {
            heroFrame.transform.parent = element.GetRectTransform();
            heroImg.transform.parent = element.GetRectTransform();
        }
    }

    private static string getHeroName(Quest.Hero h)
    {
        return "_" + ((h.heroData != null) ? h.heroData.name.Translate().Trim().Replace(" ", "_") : h.id.ToString());
    }

    private static void setImagesBorder(Quest.Hero h, out Texture2D frameTex, out string heroName)
    {
        float width = UIScaler.GetRelWidth(5);
        float height = UIScaler.GetRelHeight(50);

        frameTex = GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_border_frame_empty, CommonImageKeys.d2e_border_grey_frame, CommonImageKeys.ia_border_frame_empty);
        heroName = getHeroName(h);
        if (h.heroData != null)
        {
            frameTex = GameUtils.ReturnValueGameType<Texture2D>(CommonImageKeys.mom_border_frame, CommonImageKeys.d2e_border_blue_frame, CommonImageKeys.ia_border_frame);
        }
    }

    // Update hero image state
    public void UpdateStatus()
    {
        // If we haven't set up yet just return
        if (icons == null)
        {
            return;
        }

        if (icon_frames == null)
        {
            return;
        }

        Game game = Game.Get();
        foreach (Quest.Hero h in game.quest.heroes)
        {
            // Start as white (normal)
            icons[h.id].color = Color.white;
            icon_frames[h.id].color = Color.white;

            if (h.defeated)
            {
                // Grey hero image
                icons[h.id].color = new Color((float)0.2, (float)0.2, (float)0.2, 1);
            }
            if (h.activated)
            {
                // Grey frame
                icon_frames[h.id].color = new Color((float)0.2, (float)0.2, (float)0.2, 1);
            }
            if (h.heroData == null)
            {
                // No hero, make invisible
                icons[h.id].color = Color.clear;
                icon_frames[h.id].color = Color.clear;
            }
            if (h.selected)
            {
                // green frame
                icon_frames[h.id].color = Color.green;
            }
        }
    }

    // Redraw images
    public void UpdateImages()
    {
        if (icons == null)
        {
            return;
        }

        if (icon_frames == null)
        {
            return;
        }

        Game game = Game.Get();

        foreach (Quest.Hero h in game.quest.heroes)
        {
            Texture2D frameTex = CommonImageKeys.d2e_border_grey_frame;
            if (game.gameType is MoMGameType)
            {
                frameTex = CommonImageKeys.mom_border_frame_empty;
            }
            icons[h.id].color = Color.clear;
            icon_frames[h.id].color = Color.clear;

            if (!game.quest.heroesSelected)
            {
                icon_frames[h.id].color = Color.white;
            }

            if (h.heroData != null)
            {
                frameTex = CommonImageKeys.d2e_border_blue_frame;
                if (game.gameType is MoMGameType)
                {
                    frameTex = CommonImageKeys.mom_border_frame;
                }
                Texture2D heroTex = ContentData.FileToTexture(h.heroData.image);
                Sprite heroSprite = null;
                if (heroTex != null)
                {
                    heroSprite = Sprite.Create(heroTex, new Rect(0, 0, heroTex.width, heroTex.height), Vector2.zero, 1);
                }
                if (heroSprite != null)
                {
                    icons[h.id].sprite = heroSprite;
                }
                icons[h.id].color = Color.white;
                icon_frames[h.id].color = Color.white;
            }
            Sprite frameSprite = null;
            if (frameTex != null)
            {
                frameSprite = Sprite.Create(frameTex, new Rect(0, 0, frameTex.width, frameTex.height), Vector2.zero, 1);
            }
            if (frameSprite != null)
            {
                icon_frames[h.id].sprite = frameSprite;
            }
        }
    }

    // Called when hero pressed
    private void HeroDiag(int id)
    {
        Game game = Game.Get();
        Quest.Hero target = null;

        // Find the pressed hero
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.id == id)
            {
                target = h;
                break;
            }
        }

        // Game hasn't started, remove any selected hero
        if (!game.quest.heroesSelected)
        {
            target.heroData = null;
            UpdateImages();
            if (heroSelection != null)
            {
                heroSelection.Update();
            }

            return;
        }

        // If there are any other dialogs
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            // Check if we are in a hero selection dialog
            if (game.quest.eManager.currentEvent != null && game.quest.eManager.currentEvent.qEvent.maxHeroes != 0)
            {
                // Invert hero selection
                target.selected = !target.selected;
                UpdateStatus();
            }
            // Non hero selection dialog, do nothing
            return;
        }

        // We are in game and a valid hero was selected
        if (game.quest.heroesSelected && target.heroData != null)
        {
            if (!game.quest.UIItemsPresent())
            {
                new HeroDialog(target);
            }
        }
    }

    // End hero selection and reorder heroselect
    // FIXME: bad name
    // FIXME: why is this even here?
    public void EndSection()
    {
        int heroCount = 0;
        SetupUI();
        // Count number of selected heroes
        Game game = Game.Get();
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.heroData != null)
            {
                heroCount++;
                // Create variable to value 1 for each selected Hero
                game.quest.vars.SetValue("#" + h.heroData.sectionName, 1);

            }
        }

        // Check for validity
        if (heroCount < game.quest.qd.quest.minHero)
        {
            return;
        }

        Destroyer.Dialog();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.HEROSELECT))
        {
            Object.Destroy(go);
        }

        heroSelection = null;

        // Reorder heros so that selected heroes are first
        for (int i = 0; i < game.quest.heroes.Count - 1; i++)
        {
            int j = i;

            while (game.quest.heroes[i].heroData == null && j < game.quest.heroes.Count)
            {
                game.quest.heroes[i].heroData = game.quest.heroes[j].heroData;
                game.quest.heroes[j].heroData = null;
                j++;
            }
        }

        // Set quest flag based on hero count
        game.quest.vars.SetValue("#heroes", heroCount);

        game.quest.heroesSelected = true;

        UpdateImages();
        UpdateStatus();

        // Clear off heros if not required
        if (!game.gameType.DisplayHeroes())
        {
            Clean();
            new InvestigatorItems();
        }
        else
        {
            // Pick class
        }

        // Draw morale if required
        if (game.gameType is D2EGameType)
        {
            new ClassSelectionScreen();
        }
        else
        {
            new InvestigatorItems();
        }
    }
}
