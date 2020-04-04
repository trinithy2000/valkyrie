using Assets.Scripts.Content;
using Assets.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;

public class EditorComponentToken : EditorComponentEvent
{
    private QuestData.Token tokenComponent;

    public EditorComponentToken(string nameIn) : base(nameIn)
    {
    }

    public override void Highlight()
    {
        CameraController.SetCamera(component.location);
    }

    public override void AddLocationType(float offset)
    {
    }

    public override float AddSubEventComponents(float offset)
    {
        tokenComponent = component as QuestData.Token;

        UIElement ui = new UIElement(Game.EDITOR, scrollArea.GetScrollTransform());
        ui.SetLocation(0, offset, 6, 1);
        ui.SetText(new StringKey("val", "X_COLON", new StringKey("val", "ROTATION")));

        ui = new UIElement(Game.EDITOR, scrollArea.GetScrollTransform());
        ui.SetLocation(6, offset, 3, 1);
        ui.SetText(tokenComponent.rotation.ToString());
        ui.SetButton(delegate { Rotate(); });
        new UIElementBorder(ui);
        offset += 2;

        ui = new UIElement(Game.EDITOR, scrollArea.GetScrollTransform());
        ui.SetLocation(0, offset, 4, 1);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.TYPE));

        ui = new UIElement(Game.EDITOR, scrollArea.GetScrollTransform());
        ui.SetLocation(4, offset, 12, 1);
        ui.SetText(tokenComponent.tokenName);
        ui.SetButton(delegate { Type(); });
        new UIElementBorder(ui);
        offset += 2;

        game.quest.ChangeAlpha(tokenComponent.sectionName, 1f);

        return offset;
    }

    public override float AddEventTrigger(float offset)
    {
        return offset;
    }

    public override float AddEventVarConditionComponents(float offset)
    {
        return offset;
    }

    public void Rotate()
    {
        tokenComponent.rotation += 90;
        if (tokenComponent.rotation > 300)
        {
            tokenComponent.rotation = 0;
        }
        Game.Get().quest.Remove(tokenComponent.sectionName);
        Game.Get().quest.Add(tokenComponent.sectionName);
        Update();
    }

    public void Type()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }
        Game game = Game.Get();
        UIWindowSelectionListTraits select = new UIWindowSelectionListImage(SelectType, new StringKey("val", "SELECT", CommonStringKeys.TOKEN));

        select.AddItem(CommonStringKeys.NONE.Translate(), "{NONE}");

        foreach (KeyValuePair<string, TokenData> kv in game.cd.tokens)
        {
            select.AddItem(kv.Value);
        }
        select.ExcludeExpansions();
        select.Draw();
    }

    public void SelectType(string token)
    {
        tokenComponent.tokenName = token.Split(" ".ToCharArray())[0];
        Game.Get().quest.Remove(tokenComponent.sectionName);
        Game.Get().quest.Add(tokenComponent.sectionName);
        Update();
    }
}
