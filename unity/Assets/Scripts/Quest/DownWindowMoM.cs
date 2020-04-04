using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
public class DownWindowMoM
{
    public DownWindowMoM()
    {
        Update();
    }

    public virtual void Update()
    {
        Destroyer.Dialog();
        Game game = Game.Get();

        UIElementScrollHorizontal scrollArea = new UIElementScrollHorizontal();
        scrollArea.SetLocation(UIScaler.GetHCenter(-UIScaler.GetRelWidth(4.745f)), UIScaler.GetBottom(-UIScaler.GetRelHeight(4.5f)), UIScaler.GetRelWidth(1.74f), UIScaler.GetRelHeight(3.8f));
        scrollArea.SetBGColor(Color.clear);
        new UIDialogBackGround(scrollArea, 0);
        scrollArea.GetTransform().SetAsLastSibling();
        scrollArea.SetScrollSize(LoadItemsInScroll(game, scrollArea));
    }

    protected virtual float LoadItemsInScroll(Game game, UIElementScrollHorizontal scrollArea)
    {
        return 0;
    }

    protected void FindButtonToAsingAction(Texture2D texture)
    {
        foreach (GameObject elem in GameObject.FindGameObjectsWithTag(Game.UIPHASE))
        {
            Texture2D img = elem.GetComponent<Image>().sprite.texture;
            if (img.Equals(texture))
            {
                SetButton(delegate { Destroy(elem.GetComponent<Button>()); }, elem.GetComponent<Button>());
                break;
            }
        }
    }

    public void Inspect(string item)
    {
        Destroyer.Dialog();
        Game.Get().quest.Save();
        Game.Get().quest.eManager.QueueEvent(Game.Get().quest.itemInspect[item]);
    }


    private void Destroy(Button btn)
    {
        Destroyer.Dialog();
        SetButton(Returns, btn);
    }

    protected virtual void Returns()
    {
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }
        new DownWindowMoM();
    }

    private void SetButton(UnityEngine.Events.UnityAction call, Button uiButton)
    {
        if (uiButton != null)
        {
            uiButton.onClick.RemoveAllListeners();
        }

        uiButton.onClick.AddListener(call);
    }
}
