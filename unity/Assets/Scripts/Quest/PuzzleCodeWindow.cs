using Assets.Scripts.Content;
using Assets.Scripts.UI;
using Assets.Scripts.UI.MOM;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCodeWindow
{
    private readonly StringKey PUZZLE_GUESS = new StringKey("val", "PUZZLE_GUESS");
    private readonly Texture2D[] codes = new Texture2D[] {
        CommonImageKeys.mom_number_one,
        CommonImageKeys.mom_number_one,
        CommonImageKeys.mom_number_two,
        CommonImageKeys.mom_number_three,
        CommonImageKeys.mom_number_four,
        CommonImageKeys.mom_number_five,
        CommonImageKeys.mom_number_six
    };

    public EventManager.Event eventData;
    private readonly QuestData.Puzzle questPuzzle;
    public PuzzleCode puzzle;
    public List<int> guess;
    public int previousMoves = 0;
    public List<ButtonInfo> buttons;


    public PuzzleCodeWindow(EventManager.Event e)
    {
        eventData = e;
        Game game = Game.Get();

        guess = new List<int>();
        questPuzzle = e.qEvent as QuestData.Puzzle;
        buttons = GetButtons();

        if (game.quest.puzzle.ContainsKey(questPuzzle.sectionName))
        {
            // This puzzle was played before. Load up the previous moves.
            puzzle = game.quest.puzzle[questPuzzle.sectionName] as PuzzleCode;
            previousMoves = puzzle.guess.Count;
        }
        else
        {
            // Initialize a new puzzle
            puzzle = new PuzzleCode(questPuzzle.puzzleLevel, questPuzzle.puzzleAltLevel, questPuzzle.puzzleSolution);
        }

        CreateWindow();
    }

    public void CreateWindow()
    {
        Destroyer.Dialog();
        UIElement ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-18f), 2f, UIScaler.GetWidthUnits() / 1.5f, 25);
        ui.SetImage(CommonImageKeys.mom_bgnd_puzzle);

        // Puzzle goes here
        float hPos = UIScaler.GetHCenter(-13f);
        if (!puzzle.Solved())
        {
            for (int i = 1; i <= questPuzzle.puzzleAltLevel; i++)
            {
                int tmp = i;
                ui = new UIElement();
                ui.SetLocation(hPos, 2.5f, 2, 2);
                ui.SetText(buttons[i].label, Color.black);
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetButton(delegate { GuessAdd(tmp); });
                ui.SetImage(buttons[i].image);
                hPos += 2.5f;
            }
            hPos = UIScaler.GetHCenter(-13f);
            for (int i = 1; i <= questPuzzle.puzzleLevel; i++)
            {
                ui = new UIElement();
                ui.SetLocation(hPos, 5, 2, 2);

                if (guess.Count >= i)
                {
                    int tmp = i - 1;
                    ui.SetButton(delegate { GuessRemove(tmp); });
                    ui.SetImage(buttons[guess[tmp]].image);
                }
                else
                {
                    ui.SetImage(CommonImageKeys.mom_border_mini_picture);
                }
                hPos += 2.5f;
            }
        }

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(8), 4.75f, 5, 2);
        ui.SetText(PUZZLE_GUESS);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetButton(Guess);
        ui.SetBGColor(Color.clear);
        new UITitleBackGround_MOM(ui, CommonString.puzzle);

        // Guesses window
        UIElementScrollVertical scrollArea = new UIElementScrollVertical();
        scrollArea.SetLocation(UIScaler.GetHCenter(-13.5f), 8.5f, 27, 13f);
        new UIElementBorder(scrollArea, .37f);
        scrollArea.SetScrollSize(1 + (puzzle.guess.Count * 2.5f));

        float vPos = 0.5f;
        foreach (PuzzleCode.CodeGuess g in puzzle.guess)
        {
            hPos = 0.5f;
            foreach (int i in g.guess)
            {
                ui = new UIElement(scrollArea.GetScrollTransform());
                ui.SetLocation(hPos, vPos, 2, 2);
                ui.SetText(buttons[i].label, Color.black);
                ui.SetBGColor(new Color(1, 1, 1, 0.9f));
                ui.SetFontSize(UIScaler.GetMediumFont());
                ui.SetImage(buttons[i].image);
                new UIElementBorder(ui);
                hPos += 2.5f;
            }

            hPos = 13.25f;
            for (int i = 0; i < g.CorrectSpot(); i++)
            {
                ui = new UIElement(scrollArea.GetScrollTransform());
                ui.SetLocation(hPos, vPos, 2, 2);
                ui.SetImage(CommonImageKeys.mom_icon_success);
                hPos += 2.5f;
            }
            for (int i = 0; i < g.CorrectType(); i++)
            {
                ui = new UIElement(scrollArea.GetScrollTransform());
                ui.SetLocation(hPos, vPos, 2, 2);
                ui.SetImage(CommonImageKeys.mom_icon_investigation);
                hPos += 2.5f;
            }
            vPos += 2.5f;
        }
        scrollArea.SetScrollPosition(1 + (puzzle.guess.Count * 2.5f) * UIScaler.GetPixelsPerUnit());


        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-5f), 22f, 7, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.SKILL));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(1f), 22f, 3, 2);
        ui.SetText(EventManager.OutputSymbolReplace(questPuzzle.skill));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);


        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(5f), 22f, 7, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.MOVES));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(12), 22, 3, 2);
        ui.SetText((puzzle.guess.Count - previousMoves).ToString());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(4f), 24f, 10, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.TOTAL_MOVES));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(12), 24, 3, 2);
        ui.SetText(puzzle.guess.Count.ToString());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-15), 24f, 8, 2);
        ui.SetText(CommonStringKeys.CLOSE, puzzle.Solved() ? Color.gray : Color.white);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetImage(CommonImageKeys.mom_btn_menu);

        if (!puzzle.Solved())
        {
            ui.SetButton(Close);
        }

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-15), 22f, 8, 2);
        ui.SetText(eventData.GetButtons()[0].GetLabel(), !puzzle.Solved() ? Color.grey : Color.white);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetImage(CommonImageKeys.mom_btn_menu);

        if (puzzle.Solved())
        {
            ui.SetButton(Finished);
        }
    }

    public void GuessAdd(int symbolType)
    {
        if (guess.Count >= questPuzzle.puzzleLevel)
        {
            return;
        }
        float hPos = UIScaler.GetHCenter(-13f) + (guess.Count * 2.5f);
        guess.Add(symbolType);

        int tmp = guess.Count - 1;
        UIElement ui = new UIElement();
        ui.SetLocation(hPos, 5, 2, 2);
        ui.SetText(buttons[symbolType].label, Color.black);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetButton(delegate { GuessRemove(tmp); });
        ui.SetImage(buttons[symbolType].image);
    }

    public void GuessRemove(int symbolPos)
    {
        guess.RemoveAt(symbolPos);
        CreateWindow();
    }

    public List<ButtonInfo> GetButtons()
    {
        List<ButtonInfo> buttons = new List<ButtonInfo>();
        for (int i = 0; i <= questPuzzle.puzzleAltLevel; i++)
        {
            if (questPuzzle.imageType.Equals("symbol"))
            {
                Texture2D dupeTex = CommonImageKeys.ObtMomDupe(i);
                if (dupeTex != null)
                {
                    buttons.Add(new ButtonInfo(Sprite.Create(dupeTex, new Rect(0, 0, dupeTex.width, dupeTex.height), Vector2.zero, 1, 0, SpriteMeshType.FullRect)));
                }
                else
                {
                    buttons.Add(new ButtonInfo(new StringKey(null, i.ToString(), false)));
                }
            }
            else if (questPuzzle.imageType.Equals("element"))
            {
                Texture2D dupeTex = CommonImageKeys.ObtMomElem(i);

                if (dupeTex != null)
                {
                    buttons.Add(new ButtonInfo(Sprite.Create(dupeTex, new Rect(0, 0, dupeTex.width, dupeTex.height), Vector2.zero, 1, 0, SpriteMeshType.FullRect)));
                }
                else
                {
                    buttons.Add(new ButtonInfo(new StringKey(null, i.ToString(), false)));
                }
            }
            else
            {
                if (questPuzzle.puzzleClass.Equals("code"))
                {
                    buttons.Add(new ButtonInfo(Sprite.Create(codes[i], new Rect(0, 0, codes[i].width, codes[i].height), Vector2.zero, 1)));
                }
                else
                {
                    buttons.Add(new ButtonInfo(new StringKey(null, i.ToString(), false)));
                }
            }
        }
        return buttons;
    }

    public class ButtonInfo
    {
        public Sprite image = null;
        public StringKey label = StringKey.NULL;

        public ButtonInfo(Sprite s)
        {
            image = s;
        }
        public ButtonInfo(StringKey l)
        {
            label = l;
        }
    }

    public void Guess()
    {
        if (guess.Count < questPuzzle.puzzleLevel)
        {
            return;
        }
        puzzle.AddGuess(guess);
        guess = new List<int>();
        CreateWindow();
    }

    public void Close()
    {
        Destroyer.Dialog();
        Game game = Game.Get();
        if (game.quest.puzzle.ContainsKey(questPuzzle.sectionName))
        {
            game.quest.puzzle.Remove(questPuzzle.sectionName);
        }
        game.quest.puzzle.Add(questPuzzle.sectionName, puzzle);

        game.quest.eManager.currentEvent = null;
        game.quest.eManager.currentEvent = null;
        game.quest.eManager.TriggerEvent();
    }

    public void Finished()
    {
        Destroyer.Dialog();
        Game game = Game.Get();
        if (game.quest.puzzle.ContainsKey(questPuzzle.sectionName))
        {
            game.quest.puzzle.Remove(questPuzzle.sectionName);
        }

        game.quest.eManager.EndEvent();
    }
}
