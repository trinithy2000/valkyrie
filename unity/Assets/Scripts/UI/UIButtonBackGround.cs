using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIButtonBackGround
    {

        protected GameObject bLine;
        public UIButtonBackGround(UIElement element, int dialogType)
        {
            if (dialogType == 0)
            {
                CreateDialogButton(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (dialogType == 1)
            {
                CreateActionButton(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (dialogType == 2)
            {
                CreateQuotaButtons(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
        }


        private void CreateDialogButton(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndDlgTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.mom_btn_dialog;
            bLine.transform.SetParent(transform);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -4f, rectTrans.rect.height);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rectTrans.rect.width + 2);
            bLine.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();
        }

        private void CreateActionButton(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndActTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.mom_btn_action;
            bLine.transform.SetParent(transform);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -4f, rectTrans.rect.height);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(rectTrans.rect.width / 9.8f), rectTrans.rect.width * 1.12f);
            bLine.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();
        }
        private void CreateQuotaButtons(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndQuotTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.mom_btn_quota;
            bLine.transform.SetParent(transform);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -(rectTrans.rect.height / 0.625f), rectTrans.rect.height * 2.65f);
            bLine.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -(rectTrans.rect.width / 2.5f), rectTrans.rect.width * 1.85f);
            bLine.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();

        }
    }
}