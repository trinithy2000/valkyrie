using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UICharactersScroll : UIElementScrollHorizontal
    {
        public UICharactersScroll(string t = "") : base(t)
        {
        }

        public UICharactersScroll(Transform parent, string t = "", string n = "") : base(parent, t, n)
        {
        }

        public UICharactersScroll(Transform parent, string name) : base(parent, name)
        {
        }

        protected override void CreateBG(Transform parent)
        {
            base.CreateBG(parent);
        }
    }
}