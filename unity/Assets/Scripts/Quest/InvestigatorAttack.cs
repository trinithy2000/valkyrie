using Assets.Scripts.Content;
using Assets.Scripts.UI;
using UnityEngine;

// Window with Investigator attack information
public class InvestigatorAttack
{
    private readonly StringKey ATTACK_PROMPT = new StringKey("val", "ATTACK_PROMPT");

    // The monster that raises this dialog
    public Quest.Monster monster;
    public string attackText = "";

    public InvestigatorAttack(Quest.Monster m)
    {
        monster = m;
        AttackOptions();
    }

    public void AttackOptions()
    {
        // If a dialog window is open we force it closed (this shouldn't happen)
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.DIALOG))
        {
            Object.Destroy(go);
        }

        UIElement ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-8), 1.5f, 20, 2);
        ui.SetText(ATTACK_PROMPT);
        ui.SetFontSize(UIScaler.GetMediumFont());
        new UIElementBorderDialog(ui,CommonString.dialogOne);

        float offset = 4f;
        foreach (string type in monster.monsterData.GetAttackTypes())
        {
            string tmpType = type;
            ui = new UIElement();
            ui.SetLocation(UIScaler.GetHCenter(-4f), offset, 12, 2);
            ui.SetText(new StringKey("val", tmpType));
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(delegate { Attack(tmpType); });
            new UIButtonBackGround(ui, 1);
            offset += 2.5f;
        }

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-2f), offset, 8, 2);
        if (monster.damage == monster.GetHealth())
        {
            ui.SetText(CommonStringKeys.CANCEL, Color.gray);
        }
        else
        {
            ui.SetText(CommonStringKeys.CANCEL);
            ui.SetButton(Destroyer.Dialog);
        }
        ui.SetFontSize(UIScaler.GetMediumFont());
        new UIButtonBackGround(ui, 1);

        MonsterDialogMoM.DrawMonster(monster, true);
    }

    public void Attack(string type)
    {
        StringKey text = monster.monsterData.GetRandomAttack(type);
        attackText = EventManager.OutputSymbolReplace(text.Translate().Replace("{0}", monster.monsterData.name.Translate()));
        Game.Get().quest.log.Add(new Quest.LogEntry(attackText.Replace("\n", "\\n")));
        Attack();
    }

    public void Attack()
    {
        Destroyer.Dialog();

        UIElement ui = new UIElement();
        ui.SetLocation(18, 1.5f, UIScaler.GetWidthUnits() - 25, 10);
        ui.SetText(attackText);
        new UIElementBorderDialog(ui, CommonString.dialogOne);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(), 12, 12, 2);
        if (monster.damage == monster.GetHealth())
        {
            ui.SetText(CommonStringKeys.FINISHED, Color.gray);
        }
        else
        {
            ui.SetText(CommonStringKeys.FINISHED);
            ui.SetButton(Destroyer.Dialog);

        }
        ui.SetFontSize(UIScaler.GetMediumFont());
        new UIButtonBackGround(ui, 1);

        MonsterDialogMoM.DrawMonster(monster, true);
    }
}