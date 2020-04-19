using Assets.Scripts.UI.MOM;
using Assets.Scripts.UI.D2E;
using Assets.Scripts.UI.IA;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIElementBorderDialog
    {
        protected Transform transform;
        protected RectTransform rectTrans;
        protected string tag;
        protected string internalName;
        protected readonly string[] bNames = new string[] { "bottom", "top", "left", "right", "upLeft", "upRight", "downRight", "downLeft", "arrow" };

        public UIElementBorderDialog(UIElement element, string dialogType)
        {
            if (Game.Get().gameType is MoMGameType)
                new UIElementBorderDialog_MOM(element, dialogType);
            else if (Game.Get().gameType is D2EGameType)
                new UIElementBorderDialog_D2E(element, dialogType);
            else if (Game.Get().gameType is IAGameType)
                new UIElementBorderDialog_IA(element, dialogType);
        }

        public UIElementBorderDialog()
        {

        }
    }
}
