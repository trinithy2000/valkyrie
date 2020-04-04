using UnityEngine;
namespace Assets.Scripts.UI
{
    public class UIDialogBackGround
    {
        protected GameObject bLine;


        public UIDialogBackGround(UIElement element, int dialogType)
        {
            if (dialogType == 0)
            {
                CreateBgndDialog(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
        }

        private void CreateBgndDialog(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndDialog_0")
            {
                tag = tag
            };

            float insetW = rectTrans.rect.width / 25f;
            float insetH = rectTrans.rect.height / 4.8f;

            bLine.AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.mom_bgnd_downbar as Texture2D;
            bLine.transform.SetParent(transform);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -insetH, rectTrans.rect.height * 1.2f);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -insetW, rectTrans.rect.width * 1.07f);
            bLine.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();
        }
    }
}
