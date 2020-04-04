using Assets.Scripts.Content;
using Assets.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using ValkyrieTools;

// Class for creation of a dialog window with buttons and handling button press
// This is used for display of event information
public class DialogWindow
{
    // The even that raises this dialog
    public EventManager.Event eventData;
    // An event can have a list of selected heroes
    public List<Quest.Hero> heroList;

    public int quota = 0;

    public string text = "";

    // Create from event
    public DialogWindow(EventManager.Event e)
    {
        eventData = e;
        heroList = new List<Quest.Hero>();
        Game game = Game.Get();
        text = eventData.GetText();

        // hero list can be populated from another event
        if (!eventData.qEvent.heroListName.Equals(""))
        {
            // Try to find the event
            if (!game.quest.heroSelection.ContainsKey(eventData.qEvent.heroListName))
            {
                ValkyrieDebug.Log("Warning: Hero selection in event: " + eventData.qEvent.sectionName + " from event " + eventData.qEvent.heroListName + " with no data.");
                game.quest.log.Add(new Quest.LogEntry("Warning: Hero selection in event: " + eventData.qEvent.sectionName + " from event " + eventData.qEvent.heroListName + " with no data.", true));
            }
            else
            {
                // Get selection data from other event
                foreach (Quest.Hero h in game.quest.heroSelection[eventData.qEvent.heroListName])
                {
                    h.selected = true;
                }
            }
        }
        // Update selection status
        game.heroCanvas.UpdateStatus();

        if (eventData.qEvent.quota > 0 || eventData.qEvent.quotaVar.Length > 0)
        {
            if (eventData.qEvent.quotaVar.Length > 0)
            {
                quota = Mathf.RoundToInt(game.quest.vars.GetValue(eventData.qEvent.quotaVar));
            }
            CreateQuotaWindow();
        }
        else
        {
            CreateWindow();
        }

        DrawItem();
    }

    public void CreateWindow()
    {
        // Draw text
        UIElement ui = new UIElement();
        float offset = ui.GetStringHeight(text, 28);
        if (offset < 5)
        {
            offset = 5;
        }
        ui.SetLocation(UIScaler.GetHCenter(-14), UIScaler.GetVCenter(-5.5f), 28, offset);
        ui.SetText(text);
        new UIElementBorderDialog(ui, CommonString.dialogOne);
        offset += UIScaler.GetRelWidth(6) + 1f;


        // Determine button size
        float buttonWidth = 10;
        float buttonHeight = 2f;
        float hOffset = UIScaler.GetRelWidth(8);
        float hOffsetCancel = 11;
        float offsetCancel = offset;

        List<DialogWindow.EventButton> buttons = eventData.GetButtons();

        int num = 1;

        foreach (EventButton eb in buttons)
        {
            if (ui.GetStringHeight(eb.GetLabel().Translate(), buttonWidth, UIScaler.GetMediumFont()) > buttonHeight)
            {
                buttonHeight = ui.GetStringHeight(eb.GetLabel().Translate(), buttonWidth, UIScaler.GetMediumFont());
            }

            int numTmp = num++;

            if (buttons.Count > 3 || (buttons.Count == 1 && !eventData.qEvent.cancelable))
            {
                hOffset = UIScaler.GetHCenter(-(buttonWidth / 2));
            }
            else
            {
                if ((numTmp % 2) != 0)
                {
                    hOffset = UIScaler.GetHCenter(1);
                }
                else
                {
                    hOffset = UIScaler.GetHCenter(-(buttonWidth + 1));
                }
            }
            ui = new UIElement();
            ui.SetLocation(hOffset, offset, buttonWidth, buttonHeight);
            ui.SetText(eb.GetLabel(), eb.colour);
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(delegate { onButton(numTmp); });
            ui.SetBGColor(Color.clear);
            new UIButtonBackGround(ui, 1);
            if (buttons.Count > 3)
            {
                offset += buttonHeight + .5f;
            }
        }

        // Do we have a cancel button?
        if (eventData.qEvent.cancelable)
        {
            if (buttons.Count == 1)
            {
                hOffsetCancel = UIScaler.GetHCenter(-(buttonWidth + 1));
                offsetCancel = UIScaler.GetRelWidth(6) + 6f;
            }
            else
            {
                hOffsetCancel = UIScaler.GetHCenter(-2);
                offsetCancel = offset + (2.5f * buttons.Count);
            }
            ui = new UIElement();
            ui.SetLocation(hOffsetCancel, offsetCancel, buttonWidth, 2);
            ui.SetText(CommonStringKeys.CANCEL);
            ui.SetFontSize(UIScaler.GetMediumFont());
            ui.SetButton(onCancel);
            ui.SetBGColor(Color.clear);
            new UIButtonBackGround(ui, 1);
        }
    }

    public void CreateQuotaWindow()
    {
        // Draw text
        UIElement ui = new UIElement();
        float offset = ui.GetStringHeight(text, 28);
        if (offset < 10f)
        {
            offset = 10;
        }
        ui.SetLocation(UIScaler.GetHCenter(-14f), 10f, 28, 4);
        ui.SetText(text);
        new UIElementBorderDialog(ui, CommonString.dialogOne);
        offset += 1;

        // Only one button, action depends on quota
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-3), offset * 1.6f, 6, 2);
        ui.SetText(eventData.GetButtons()[0].GetLabel());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetButton(onQuota);
        new UIButtonBackGround(ui, 2);

        // Text quota
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-1.4f), offset * 1.35f, 3, 3);
        ui.SetText(quota.ToString());
        ui.SetFontSize(UIScaler.GetLargeFont());
        ui.SetFontStyle(FontStyle.Bold);
        ui.SetBGColor(Color.clear);

        // button -
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-5.2f), offset * 1.32f, 3, 3);
        ui.SetBGColor(Color.clear);
        if (quota != 0)
        {
            ui.SetButton(quotaDec);
        }

        // button +
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(2.4f), offset * 1.32f, 3, 3);
        ui.SetBGColor(Color.clear);
        if (quota < 10)
        {
            ui.SetButton(quotaInc);
        }

        // Do we have a cancel button?
        //  if (eventData.qEvent.cancelable)
        //  {
        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-3.3f), offset * 1.82f, 7.1f, 1.65f);
        ui.SetText(CommonStringKeys.CANCEL);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetButton(onCancel);
        new UIButtonBackGround(ui, 1);
        //  }
    }

    public void DrawItem()
    {
        if (eventData.qEvent.highlight)
        {
            return;
        }

        string item = "";
        int items = 0;
        foreach (string s in eventData.qEvent.addComponents)
        {
            if (s.IndexOf("QItem") == 0)
            {
                item = s;
                items++;
            }
        }
        if (items != 1)
        {
            return;
        }

        Game game = Game.Get();

        if (!game.quest.itemSelect.ContainsKey(item))
        {
            return;
        }

        Texture2D tex = ContentData.FileToTexture(game.cd.items[game.quest.itemSelect[item]].image);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1, 0, SpriteMeshType.FullRect);

        UIElement ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-21), 0.5f, 6, 6);
        ui.SetImage(sprite);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-22.5f), 6.5f, 9, 1);
        ui.SetText(game.cd.items[game.quest.itemSelect[item]].name);
    }

    public void quotaDec()
    {
        quota--;
        Destroyer.Dialog();
        CreateQuotaWindow();
    }

    public void quotaInc()
    {
        quota++;
        Destroyer.Dialog();
        CreateQuotaWindow();
    }

    public void onQuota()
    {
        Game game = Game.Get();
        if (eventData.qEvent.quotaVar.Length > 0)
        {
            game.quest.vars.SetValue(eventData.qEvent.quotaVar, quota);
            onButton(1);
            return;
        }

        if (game.quest.eventQuota.ContainsKey(eventData.qEvent.sectionName))
        {
            game.quest.eventQuota[eventData.qEvent.sectionName] += quota;
        }
        else
        {
            game.quest.eventQuota.Add(eventData.qEvent.sectionName, quota);
        }
        if (game.quest.eventQuota[eventData.qEvent.sectionName] >= eventData.qEvent.quota)
        {
            game.quest.eventQuota.Remove(eventData.qEvent.sectionName);
            onButton(1);
        }
        else
        {
            onButton(2);
        }
    }

    // Cancel cleans up
    public void onCancel()
    {
        Destroyer.Dialog();
        Game.Get().quest.eManager.currentEvent = null;
        // There may be a waiting event
        Game.Get().quest.eManager.TriggerEvent();
    }

    public void onButton(int num)
    {
        // Do we have correct hero selection?
        if (!checkHeroes())
        {
            return;
        }

        Game game = Game.Get();
        // Destroy this dialog to close
        Destroyer.Dialog();

        // If the user started this event button is undoable
        if (eventData.qEvent.cancelable)
        {
            game.quest.Save();
        }

        // Add this to the log
        game.quest.log.Add(new Quest.LogEntry(text.Replace("\n", "\\n")));

        // Add this to the eventList
        game.quest.eventList.Add(eventData.qEvent.sectionName);

        // Event manager handles the aftermath
        game.quest.eManager.EndEvent(num - 1);
    }

    // Check that the correct number of heroes are selected
    public bool checkHeroes()
    {
        Game game = Game.Get();

        heroList = new List<Quest.Hero>();

        // List all selected heroes
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.selected)
            {
                heroList.Add(h);
            }
        }

        // Check that count matches
        if (eventData.qEvent.maxHeroes < heroList.Count && eventData.qEvent.maxHeroes != 0)
        {
            return false;
        }

        if (eventData.qEvent.minHeroes > heroList.Count)
        {
            return false;
        }

        // Clear selection
        foreach (Quest.Hero h in game.quest.heroes)
        {
            h.selected = false;
        }

        // If this event has previous selected heroes clear the data
        if (game.quest.heroSelection.ContainsKey(eventData.qEvent.sectionName))
        {
            game.quest.heroSelection.Remove(eventData.qEvent.sectionName);
        }
        // Add this selection to the quest
        game.quest.heroSelection.Add(eventData.qEvent.sectionName, heroList);

        // Update hero image state
        game.heroCanvas.UpdateStatus();

        // Selection OK
        return true;
    }

    public class EventButton
    {
        private readonly StringKey label = StringKey.NULL;
        public Color32 colour = Color.white;

        public EventButton(StringKey newLabel, string newColour)
        {
            label = newLabel;
            string colorRGB = ColorUtil.FromName(newColour);

            // Check format is valid
            if ((colorRGB.Length != 7 && colorRGB.Length != 9) || (colorRGB[0] != '#'))
            {
                Game.Get().quest.log.Add(new Quest.LogEntry("Warning: Button color must be in #RRGGBB format or a known name", true));
            }

            // Hexadecimal to float convert (0x00-0xFF -> 0.0-1.0)
            colour.r = System.Convert.ToByte(colorRGB.Substring(1, 2), 16);
            colour.g = System.Convert.ToByte(colorRGB.Substring(3, 2), 16);
            colour.b = System.Convert.ToByte(colorRGB.Substring(5, 2), 16);

            if (colorRGB.Length == 9)
            {
                colour.a = System.Convert.ToByte(colorRGB.Substring(7, 2), 16);
            }
            else
            {
                colour.a = 255; // opaque by default
            }
        }

        public StringKey GetLabel()
        {
            return new StringKey(null, EventManager.OutputSymbolReplace(EventManager.Event.ReplaceComponentText(label.Translate())), false);
        }
    }
}
