using Assets.Scripts.Content;
using Assets.Scripts.UI;
using UnityEngine;

// Next stage button is used by MoM to move between investigators and monsters
public class NextStageButton
{
    private readonly StringKey PHASE_INVESTIGATOR = new StringKey("val", "PHASE_INVESTIGATOR");
    private readonly StringKey PHASE_MYTHOS = new StringKey("val", "PHASE_MYTHOS");
    private readonly StringKey MONSTER_STEP = new StringKey("val", "MONSTER_STEP");
    private readonly StringKey HORROR_STEP = new StringKey("val", "HORROR_STEP");

    // Construct and display
    public NextStageButton()
    {
        if (Game.Get().gameType.DisplayHeroes())
        {
            return;
        }

        Update();
    }

    public void Update()
    {
        // do not display the button bar when we reach the end of the game screen
        if (Game.Get().quest.questHasEnded)
        {
            return;
        }

        // First tile has not been displayed, button bar is not required yet
        if (!Game.Get().quest.firstTileDisplayed)
        {
            return;
        }

        // do not display the button bar when we are in the editor
        if (Game.Get().editMode)
        {
            return;
        }

        // Clean up everything marked as 'uiphase'
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.UIPHASE))
        {
            Object.Destroy(go);
        }

        Color bgColor = new Color(0.05f, 0, 0, 0.9f);
        StringKey phase;
        if (Game.Get().quest.phase == Quest.MoMPhase.horror)
        {
            phase = HORROR_STEP;
        }
        else if (Game.Get().quest.phase == Quest.MoMPhase.mythos)
        {
            phase = PHASE_MYTHOS;
        }
        else if (Game.Get().quest.phase == Quest.MoMPhase.monsters)
        {
            phase = MONSTER_STEP;
        }
        else
        {
            phase = PHASE_INVESTIGATOR;
            bgColor = new Color(0, 0, 0, 0.9f);
        }

        float string_width = UIScaler.GetRelWidth(13.4f);
        float offset = UIScaler.GetRelHeight(14.5f);
        float button_hw = UIScaler.GetRelWidth(16.3f);
        float button_ph = UIScaler.GetBottom(-UIScaler.GetRelHeight(7.92f));

        UIElement ui_parent = new UIElement(Game.UIPHASE);
        ui_parent.SetLocation(1, UIScaler.GetBottom(-UIScaler.GetRelHeight(5.6f)), UIScaler.GetRelWidth(4f), UIScaler.GetRelHeight(4.2f));
        ui_parent.SetImage(CommonImageKeys.mom_bgnd_bbox_r);
        ui_parent.GetTransform().SetAsLastSibling();

        // Inventory button 
        UIElement ui = new UIElement(Game.UIPHASE);
        ui.SetLocation(offset, button_ph, button_hw, button_hw);
        ui.SetImage(CommonImageKeys.mom_btn_system);
        ui.SetButton(Set);
        ui.GetTransform().parent = ui_parent.GetTransform();
        offset += string_width;

        // BAG button
        ui = new UIElement(Game.UIPHASE);
        ui.SetLocation(offset, button_ph, button_hw, button_hw);
        ui.SetImage(CommonImageKeys.mom_btn_bag);
        ui.SetButton(Items);
        ui.GetTransform().parent = ui_parent.GetTransform();
        offset += string_width;

        // Log button (text from previous event)
        ui = new UIElement(Game.UIPHASE);
        ui.SetLocation(offset, button_ph, button_hw, button_hw);
        ui.SetImage(CommonImageKeys.mom_btn_log);
        ui.SetButton(Log);
        ui.GetTransform().parent = ui_parent.GetTransform();

        ui_parent = new UIElement(Game.UIPHASE);
        ui_parent.SetLocation(UIScaler.GetRight(-UIScaler.GetRelWidth(8)), UIScaler.GetBottom(-UIScaler.GetRelHeight(6.5f)), UIScaler.GetRelWidth(10.5f), UIScaler.GetRelHeight(5));
        ui_parent.SetImage(CommonImageKeys.mom_bgnd_bbox_l);
        ui_parent.GetTransform().SetAsLastSibling();

        ui = new UIElement(Game.UIPHASE);
        ui.SetLocation(UIScaler.GetRight(-UIScaler.GetRelWidth(9.3f)), button_ph, button_hw, button_hw);
        ui.SetImage(CommonImageKeys.mom_btn_next);
        ui.SetButton(Next);
        ui.GetTransform().parent = ui_parent.GetTransform();

        // Text description for current phase
        ui = new UIElement(Game.UIPHASE);
        Color color;
        if (phase == PHASE_INVESTIGATOR)
        {
            color = Color.white;
        }
        else
        {
            color = Color.red;
        }

        ui.SetText(phase, color);
        string_width = ui.GetStringWidth(phase, UIScaler.GetMediumFont(), Game.Get().gameType.GetHeaderFont()) + 0.5f;
        ui.SetLocation(offset + ((UIScaler.GetRight(-4f) - offset - string_width) * 0.5f), UIScaler.GetBottom(-1.8f), string_width, 1.8f);
        ui.SetBGColor(bgColor);
        ui.SetFont(Game.Get().gameType.GetHeaderFont());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetFontStyle(FontStyle.Italic);

    }

    // Button pressed
    public void Next()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }

        Game game = Game.Get();

        if (game.quest.UIItemsPresent())
        {
            return;
        }

        // Add to undo stack
        game.quest.Save();

        if (game.quest.phase == Quest.MoMPhase.monsters)
        {
            game.audioControl.PlayTrait("horror");
            game.quest.phase = Quest.MoMPhase.horror;
            return;
        }

        if (game.quest.phase == Quest.MoMPhase.horror)
        {
            game.roundControl.EndRound();
        }
        else
        {
            game.quest.log.Add(new Quest.LogEntry(new StringKey("val", "PHASE_MYTHOS").Translate()));
            game.roundControl.HeroActivated();
        }
    }

    public void Items()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }
        new InventoryWindowMoM();
    }

    public void Log()
    {
        Destroyer.Dialog();
        new LogWindow();
    }

    public void Set()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }
        new SetWindow();
    }
}
