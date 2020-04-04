using Assets.Scripts.Content;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

// Class for creation of monster seleciton options
// Extends the standard class for MoM
public class MonsterDialogMoM : MonsterDialog
{
    private static readonly StringKey DEFEATED = new StringKey("val", "DEFEATED");
    private readonly StringKey EVADE = new StringKey("val", "EVADE");
    private readonly StringKey ATTACK = new StringKey("val", "ATTACK");
    private readonly StringKey HORROR_CHECK = new StringKey("val", "HORROR_CHECK");

    public MonsterDialogMoM(Quest.Monster m) : base(m)
    {
    }

    public override void CreateWindow()
    {
        Destroyer.Dialog();
        Game game = Game.Get();

        DrawMonster(monster);

        // In horror phase we do horror checks
        if (game.quest.phase == Quest.MoMPhase.horror)
        {
            UIElement ui = new UIElement();
            ui.SetLocation(UIScaler.GetHCenter(-8f), 2, 16, 2);
            ui.SetText(HORROR_CHECK);
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(Horror);
            new UIElementBorder(ui);

            ui = new UIElement();
            ui.SetLocation(UIScaler.GetHCenter(-5f), 4.5f, 10, 2);
            ui.SetText(CommonStringKeys.CANCEL);
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(OnCancel);
            new UIElementBorder(ui);
        }
        else
        { // In investigator phase we do attacks and evades
            DrawMonsterHealth(monster, delegate { CreateWindow(); });

            bool condition = monster.damage != monster.GetHealth();
            float center = UIScaler.GetHCenter(-22.4f);
            float anchor = 1.75f;

            UIElement ui = new UIElement();
            ui.SetLocation(center, 17.5f, 9, anchor);
            ui.SetText(new StringKey("val", "ACTION_X", ATTACK));
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetBGColor(Color.clear);
            ui.SetButton(Attack);
            new UIButtonBackGround(ui, 0);

            ui = new UIElement();
            ui.SetLocation(center, 19.5f, 9, anchor);
            ui.SetText(EVADE);
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetBGColor(Color.clear);
            ui.SetButton(Evade);
            new UIButtonBackGround(ui, 0);

            if (monster.damage != monster.GetHealth())
            {
                ui = new UIElement();
                ui.SetLocation(center, 21.5f, 9, anchor);
                ui.SetText(CommonStringKeys.CANCEL, condition ? Color.white : Color.gray);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetBGColor(Color.clear);
                if (condition)
                {
                    ui.SetButton(OnCancel);
                }

                new UIButtonBackGround(ui, 0);
            }
        }
    }

    public static void DrawMonster(Quest.Monster monster, bool displayHealth = false)
    {
        Game game = Game.Get();

        GameObject mImg = new GameObject("monsterImg" + monster.monsterData.name)
        {
            tag = Game.DIALOG
        };
        mImg.transform.SetParent(game.uICanvas.transform);

        RectTransform trans = mImg.AddComponent<RectTransform>();
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.4f * UIScaler.GetPixelsPerUnit(), 10.5f * UIScaler.GetPixelsPerUnit());
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 3.6f * UIScaler.GetPixelsPerUnit(), 10.5f * UIScaler.GetPixelsPerUnit());
        mImg.AddComponent<CanvasRenderer>();

        RawImage icon = mImg.AddComponent<RawImage>();
        icon.texture = ContentData.FileToTexture(monster.monsterData.image);
        icon.rectTransform.sizeDelta = new Vector2(11f * UIScaler.GetPixelsPerUnit(), 11f * UIScaler.GetPixelsPerUnit());

        GameObject mBgnd = new GameObject("monsterBgnd" + monster.monsterData.name)
        {
            tag = Game.DIALOG
        };
        mBgnd.transform.SetParent(game.uICanvas.transform);

        trans = mBgnd.AddComponent<RectTransform>();
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 1.5f * UIScaler.GetPixelsPerUnit(), 23f * UIScaler.GetPixelsPerUnit());
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 1f * UIScaler.GetPixelsPerUnit(), 16f * UIScaler.GetPixelsPerUnit());
        mBgnd.AddComponent<CanvasRenderer>();

        RawImage bgnd = mBgnd.AddComponent<RawImage>();
        bgnd.texture = CommonImageKeys.mom_bgnd_monster;
        bgnd.rectTransform.sizeDelta = new Vector2(16f * UIScaler.GetPixelsPerUnit(), 23f * UIScaler.GetPixelsPerUnit());

        GameObject mText = new GameObject("monsterName" + monster.monsterData.name)
        {
            tag = Game.DIALOG
        };
        mText.transform.SetParent(game.uICanvas.transform);

        trans = mText.AddComponent<RectTransform>();
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 6.2f * UIScaler.GetPixelsPerUnit(), 15f * UIScaler.GetPixelsPerUnit());
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 4.5f * UIScaler.GetPixelsPerUnit(), 15f * UIScaler.GetPixelsPerUnit());
        mText.AddComponent<CanvasRenderer>();

        Text uiText = mText.AddComponent<Text>();
        uiText.color = Color.black;
        uiText.font = game.gameType.GetHeaderFont();
        uiText.material = uiText.font.material;
        uiText.fontSize = UIScaler.GetSemiSmallFont();
        uiText.text = monster.monsterData.name.Translate();
        uiText.rectTransform.sizeDelta = new Vector2(13f * UIScaler.GetPixelsPerUnit(), 3f * UIScaler.GetPixelsPerUnit());

        Texture2D dupeTex = CommonImageKeys.ObtMomDupe(monster.duplicate);

        if (dupeTex != null)
        {
            GameObject mImgDupe = new GameObject("monsterDupe" + monster.monsterData.name)
            {
                tag = Game.DIALOG
            };
            mImgDupe.transform.SetParent(game.uICanvas.transform);

            RectTransform dupeFrame = mImgDupe.AddComponent<RectTransform>();
            dupeFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5f * UIScaler.GetPixelsPerUnit(), UIScaler.GetPixelsPerUnit() * 4f);
            dupeFrame.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5f * UIScaler.GetPixelsPerUnit(), 4f * UIScaler.GetPixelsPerUnit());
            mImgDupe.AddComponent<CanvasRenderer>();

            RawImage iconDupe = mImgDupe.AddComponent<RawImage>();
            iconDupe.texture = dupeTex;
            iconDupe.rectTransform.sizeDelta = new Vector2(4f * UIScaler.GetPixelsPerUnit(), 4f * UIScaler.GetPixelsPerUnit());
        }

        if (displayHealth)
        {
            DrawMonsterHealth(monster, DrawMonster);
        }
    }

    private static void DrawMonsterHealth(Quest.Monster monster, UnityEngine.Events.UnityAction<Quest.Monster, bool> call)
    {
        UIElement ui = new UIElement();
        ui.SetLocation(2.6f, 2.7f, 3, 3);
        ui.SetText(monster.GetHealth().ToString(), Color.white);
        ui.SetFontSize(UIScaler.GetLargeFont());
        ui.SetFontStyle(FontStyle.Bold);
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(5.1f, 14, 2, 2);
        ui.SetBGColor(Color.clear);

        if (monster.damage != 0)
        {
            ui.SetButton(delegate { MonsterDamageDec(monster, call); });
        }

        ui = new UIElement();
        ui.SetLocation(7.9f, 14.2f, 2, 2);
        ui.SetText(monster.damage.ToString(), Color.yellow);
        ui.SetFontSize(UIScaler.GetLargeFont());
        ui.SetFontStyle(FontStyle.Bold);
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(11, 14, 2, 2);
        ui.SetBGColor(Color.clear);
        if (monster.damage != monster.GetHealth())
        {
            ui.SetButton(delegate { MonsterDamageInc(monster, call); });
        }
        else
        {
            UIElement defui = new UIElement();
            defui.SetLocation(UIScaler.GetHCenter(-22.4f), 21.5f, 9, 1.75f);
            defui.SetText(DEFEATED);
            defui.SetFontSize(UIScaler.GetMediumFont());
            defui.SetButton(delegate { Defeated(monster); });
            defui.SetBGColor(Color.clear);
            new UIButtonBackGround(defui, 0);
        }
    }

    public static void Defeated(Quest.Monster monster)
    {
        Destroyer.Dialog();
        Game game = Game.Get();
        // Remove this monster group
        game.quest.monsters.Remove(monster);
        game.monsterCanvas.UpdateList();

        game.quest.vars.SetValue("#monsters", game.quest.monsters.Count);

        game.audioControl.PlayTrait("defeated");

        // end this event (fix #1112)
        Game.Get().quest.eManager.currentEvent = null;

        // Trigger defeated event
        game.quest.eManager.EventTriggerType("Defeated" + monster.monsterData.sectionName);
        // If unique trigger defeated unique event
        if (monster.unique)
        {
            game.quest.eManager.EventTriggerType("DefeatedUnique" + monster.monsterData.sectionName);
        }

        // fix #982
        if (game.quest.phase == Quest.MoMPhase.monsters)
        {
            Game.Get().roundControl.MonsterActivated();
        }
    }

    public static void MonsterDamageDec(Quest.Monster monster, UnityEngine.Events.UnityAction<Quest.Monster, bool> call)
    {
        monster.damage -= 1;
        if (monster.damage < 0)
        {
            monster.damage = 0;
        }
        call(monster, true);
    }

    public static void MonsterDamageInc(Quest.Monster monster, UnityEngine.Events.UnityAction<Quest.Monster, bool> call)
    {
        monster.damage += 1;
        if (monster.damage > monster.GetHealth())
        {
            monster.damage = monster.GetHealth();
        }
        call(monster, true);
    }


    public void Attack()
    {
        Game game = Game.Get();
        // Save to undo stack
        game.quest.Save();
        new InvestigatorAttack(monster);
    }

    public void Evade()
    {
        new InvestigatorEvade(monster);
    }

    public void Horror()
    {
        new HorrorCheck(monster);
    }
}
