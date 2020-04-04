using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UICharactersScroll : UIElementScrollHorizontal
    {
        public UICharactersScroll(string t = "") : base(t)
        {
        }

        protected override void CreateBG(Transform parent)
        {
            base.CreateBG(parent);
        }
    }
}