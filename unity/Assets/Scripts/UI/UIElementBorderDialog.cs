using Assets.Scripts.UI.MOM;
using Assets.Scripts.UI.D2E;

namespace Assets.Scripts.UI
{
    public class UIElementBorderDialog
    {

        public UIElementBorderDialog(UIElement element, string dialogType)
        {
            if (Game.Get().gameType is MoMGameType)
                new UIElementBorderDialog_MOM(element, dialogType);
            else if (Game.Get().gameType is D2EGameType)
                new UIElementBorderDialog_D2E(element, dialogType);
        }

        public UIElementBorderDialog()
        {

        }
    }
}
