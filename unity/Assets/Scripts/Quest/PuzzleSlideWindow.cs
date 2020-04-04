using Assets.Scripts.Content;
using Assets.Scripts.UI;
using UnityEngine;

public class PuzzleSlideWindow
{

    public EventManager.Event eventData;
    private readonly QuestData.Puzzle questPuzzle;
    public PuzzleSlide puzzle;
    public int lastMoves = 0;

    public PuzzleSlideWindow(EventManager.Event e)
    {
        eventData = e;
        Game game = Game.Get();

        questPuzzle = e.qEvent as QuestData.Puzzle;

        if (game.quest.puzzle.ContainsKey(questPuzzle.sectionName))
        {
            puzzle = game.quest.puzzle[questPuzzle.sectionName] as PuzzleSlide;
            lastMoves = puzzle.moves;
        }
        else
        {
            puzzle = new PuzzleSlide(questPuzzle.puzzleLevel);
        }

        CreateWindow();
    }

    public void CreateWindow()
    {
        Destroyer.Dialog();

        UIElement ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-18f), 2f, UIScaler.GetWidthUnits() / 1.7f, 25);
        ui.SetImage(CommonImageKeys.mom_bgnd_puzzle);

        // Puzzle goes here
        GameObject background = new GameObject("puzzleContent")
        {
            tag = Game.DIALOG
        };
        RectTransform transBg = background.AddComponent<RectTransform>();
        background.transform.SetParent(Game.Get().uICanvas.transform);
        transBg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, UIScaler.GetPixelsPerUnit() * 3.8f, 18f * UIScaler.GetPixelsPerUnit());
        transBg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, UIScaler.GetHCenter(-16f) * UIScaler.GetPixelsPerUnit(), 24f * UIScaler.GetPixelsPerUnit());

        DrawSlideFrame(background.transform, transBg);

        foreach (PuzzleSlide.Block b in puzzle.puzzle)
        {
            CreateBlock(b, transBg, b.target);
        }

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(2.6f), 17, 7, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.SKILL));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(7.5f), 17, 3, 2);
        ui.SetText(EventManager.OutputSymbolReplace(questPuzzle.skill));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(3.5f), 19, 7, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.MOVES));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(9.5f), 19, 3, 2);
        ui.SetText((puzzle.moves - lastMoves).ToString());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(3.2f), 21, 8.5f, 2);
        ui.SetText(new StringKey("val", "X_COLON", CommonStringKeys.TOTAL_MOVES));
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(10.5f), 21, 3, 2);
        ui.SetText(puzzle.moves.ToString());
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetBGColor(Color.clear);

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-15), 24f, 8, 2);
        ui.SetText(CommonStringKeys.CLOSE, puzzle.Solved() ? Color.grey : Color.white);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetImage(CommonImageKeys.mom_btn_menu);
        if (!puzzle.Solved())
        {
            ui.SetButton(Close);
        }

        ui = new UIElement();
        ui.SetLocation(UIScaler.GetHCenter(-5), 24f, 8, 2);
        ui.SetText(eventData.GetButtons()[0].GetLabel(), puzzle.Solved() ? Color.white : Color.grey);
        ui.SetFontSize(UIScaler.GetMediumFont());
        ui.SetImage(CommonImageKeys.mom_btn_menu);

        if (puzzle.Solved())
        {
            ui.SetButton(Finished);
        }
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

    public void DrawSlideFrame(Transform trans, RectTransform rectTrans, float scale = 3f)
    {
        GameObject bLine = new GameObject("PuzzleFrame")
        {
            tag = Game.DIALOG
        };
        bLine.AddComponent<RectTransform>();
        bLine.AddComponent<CanvasRenderer>();
        bLine.AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.mom_border_mechanical;
        bLine.transform.SetParent(trans);
        // Set the thickness of the lines

        bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -(1.09f * UIScaler.GetPixelsPerUnit()), rectTrans.rect.height * 1.12f);
        bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(1.08f * UIScaler.GetPixelsPerUnit()), rectTrans.rect.width * 1.095f);

    }

    public void CreateBlock(PuzzleSlide.Block block, RectTransform pos, bool target = false)
    {
        Texture2D bar = CommonImageKeys.mom_bar_gold;
        // Create object
        GameObject blockGO = new GameObject("puzzleBlock");

        if (block.target)
        {
            bar = CommonImageKeys.mom_bar_key;
        }
        else if (block.xlen > 1)
        {
            bar = CommonImageKeys.mom_bar_gold_ext;
        }
        else if (block.ylen > 1)
        {
            bar = CommonImageKeys.mom_bar_gold_ext_v;
        }
        else if (block.ylen == 1)
        {
            bar = CommonImageKeys.mom_bar_gold_v;
        }

        blockGO.tag = Game.DIALOG;

        //Game game = Game.Get();
        blockGO.transform.SetParent(pos);

        RectTransform transBg = blockGO.AddComponent<RectTransform>();
        transBg.pivot = Vector2.up;
        transBg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (block.ypos * 3f * UIScaler.GetPixelsPerUnit()) + 0.1f, ((block.ylen + 1) * 3f * UIScaler.GetPixelsPerUnit()) - 0.2f);
        transBg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (block.xpos * 3f * UIScaler.GetPixelsPerUnit()) + 0.1f, ((block.xlen + 1) * 3f * UIScaler.GetPixelsPerUnit()) - 0.2f);
        blockGO.AddComponent<CanvasRenderer>();

        UnityEngine.UI.RawImage uiImage = blockGO.AddComponent<UnityEngine.UI.RawImage>();
        uiImage.texture = bar;

        BlockSlider slider = blockGO.AddComponent<BlockSlider>();
        slider.block = block;
        slider.win = this;
    }
}

public class BlockSlider : MonoBehaviour
{
    public bool sliding = false;
    public Vector2 mouseStart;
    public Vector2 transStart;
    public PuzzleSlide.Block block;
    public PuzzleSlideWindow win;
    private RectTransform trans;

    // Use this for initialization (called at creation)
    private void Start()
    {
        trans = gameObject.GetComponent<RectTransform>();
        // Get the image attached to this game object
    }

    // Update is called once per frame
    private void Update()
    {
        if (!sliding && !Input.GetMouseButtonDown(0))
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < trans.position.x)
            {
                return;
            }

            if (Input.mousePosition.y < trans.position.y - trans.rect.height)
            {
                return;
            }

            if (Input.mousePosition.x > trans.position.x + trans.rect.width)
            {
                return;
            }

            if (Input.mousePosition.y > trans.position.y)
            {
                return;
            }

            sliding = true;
            mouseStart = Input.mousePosition;
            transStart = trans.anchoredPosition;
        }

        if (!sliding)
        {
            return;
        }

        if (block.rotation)
        {
            float yTarget = -transStart.y + mouseStart.y - Input.mousePosition.y;
            float yTargetSq = yTarget / (3f * UIScaler.GetPixelsPerUnit());
            int yLimit = GetNegativeLimit();
            if (yTargetSq < yLimit)
            {
                yTargetSq = yLimit;
            }
            yLimit = GetPositiveLimit();
            if (yTargetSq > yLimit)
            {
                yTargetSq = yLimit;
            }
            yTarget = (yTargetSq * 3f * UIScaler.GetPixelsPerUnit());
            float nearestFit = (yTargetSq * 3f * UIScaler.GetPixelsPerUnit());
            if (Mathf.Abs(yTarget - nearestFit) < (UIScaler.GetPixelsPerUnit() * 1f))
            {
                yTarget = nearestFit;
            }
            Vector3 pos = trans.anchoredPosition;
            pos.y = -yTarget;
            trans.anchoredPosition = pos;
        }
        else
        {
            float xTarget = transStart.x + Input.mousePosition.x - mouseStart.x;
            float xTargetSq = xTarget / (3f * UIScaler.GetPixelsPerUnit());
            int xLimit = GetNegativeLimit();
            if (xTargetSq < xLimit)
            {
                xTargetSq = xLimit;
            }
            xLimit = GetPositiveLimit();
            if (xTargetSq > xLimit)
            {
                xTargetSq = xLimit;
            }
            xTarget = xTargetSq * 3f * UIScaler.GetPixelsPerUnit();
            float nearestFit = Mathf.Round(xTargetSq) * 3f * UIScaler.GetPixelsPerUnit();
            if (Mathf.Abs(xTarget - nearestFit) < (UIScaler.GetPixelsPerUnit() * 1f))
            {
                xTarget = nearestFit;
            }
            Vector3 pos = trans.anchoredPosition;
            pos.x = xTarget;
            trans.anchoredPosition = pos;
        }

        if (!Input.GetMouseButton(0))
        {
            sliding = false;
            int newXPos = Mathf.RoundToInt(trans.anchoredPosition.x / (3f * UIScaler.GetPixelsPerUnit()));
            int newYPos = Mathf.RoundToInt(-trans.anchoredPosition.y / (3f * UIScaler.GetPixelsPerUnit()));
            if (newXPos != block.xpos || newYPos != block.ypos)
            {
                win.puzzle.moves++;
                block.xpos = newXPos;
                block.ypos = newYPos;
            }
            // Update
            win.CreateWindow();
        }
    }

    public int GetNegativeLimit()
    {
        int posx = block.xpos;
        int posy = block.ypos;

        do
        {
            if (block.rotation)
            {
                posy--;
            }
            else
            {
                posx--;
            }
        } while (PuzzleSlide.Empty(win.puzzle.puzzle, posx, posy));

        if (block.rotation)
        {
            return posy + 1;
        }
        if (block.target && posx == 4)
        {
            return 6;
        }
        return posx + 1;
    }

    public int GetPositiveLimit()
    {
        int posx = block.xpos;
        int posy = block.ypos;

        if (block.rotation)
        {
            posy += block.ylen + 1;
        }
        else
        {
            posx += block.xlen + 1;
        }

        while (PuzzleSlide.Empty(win.puzzle.puzzle, posx, posy))
        {
            if (block.rotation)
            {
                posy++;
            }
            else
            {
                posx++;
            }
        }

        if (block.rotation)
        {
            return posy - (1 + block.ylen);
        }
        return posx - (1 + block.xlen);
    }
}
