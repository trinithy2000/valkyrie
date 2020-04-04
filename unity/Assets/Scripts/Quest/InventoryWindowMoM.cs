using Assets.Scripts.UI;
using UnityEngine;

// Next stage button is used by MoM to move between investigators and monsters
public class InventoryWindowMoM : DownWindowMoM
{
    public override void Update()
    {
        base.Update();
        base.FindButtonToAsingAction(CommonImageKeys.mom_btn_bag);
    }

    protected override float LoadItemsInScroll(Game game, UIElementScrollHorizontal scrollArea)
    {
        float xOffset = 1;
        foreach (string s in game.quest.itemInspect.Keys)
        {
            string tmp = s;

            Texture2D itemTex = ContentData.FileToTexture(game.cd.items[s].image);
            Sprite itemSprite = Sprite.Create(itemTex, new Rect(0, 0, itemTex.width, itemTex.height), Vector2.zero, 1, 0, SpriteMeshType.FullRect);
            UIElement ui = new UIElement(scrollArea.GetScrollTransform());
            ui.SetLocation(xOffset, .18f, 5.4f, 5.4f);
            ui.SetButton(delegate { Inspect(tmp); });
            ui.SetImage(itemSprite);

            ui = new UIElement(scrollArea.GetScrollTransform());
            ui.SetLocation(xOffset, 5.5f, 5.4f, 1);
            ui.SetButton(delegate { Inspect(tmp); });
            ui.SetBGColor(Color.black);
            ui.SetText(game.cd.items[s].name, Color.white);

            xOffset += 9;
        }
        return xOffset;
    }
    protected override void Returns()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }
        new InventoryWindowMoM();
    }
}
