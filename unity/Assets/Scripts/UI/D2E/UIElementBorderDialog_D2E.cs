using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.D2E
{
    public class UIElementBorderDialog_D2E
    {
        protected GameObject[] bLine;

        public UIElementBorderDialog_D2E(UIElement element, string dialogType)
        {
            if (CommonString.dialog.Equals(dialogType))
            {
                CreateBorderDialog(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.dialogOne.Equals(dialogType))
            {
                CreateBorderDialogOne(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }

        }
        private void CreateBorderDialog(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject[6];

            // create 4 lines
            for (int i = 0; i < 6; i++)
            {
                bLine[i].tag = tag;
                if (i == 4)
                {
                    bLine[i].AddComponent<UnityEngine.UI.RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarTop") as Texture2D;
                }
                else if (i == 5)
                {
                    bLine[i].AddComponent<UnityEngine.UI.RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarBottom") as Texture2D;
                }
                else if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<UnityEngine.UI.RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarVer") as Texture2D;
                }
                else
                {
                    bLine[i].AddComponent<UnityEngine.UI.RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarHor") as Texture2D;
                }
                bLine[i].transform.SetParent(transform);
            }

            // rectangle

            DialogSimpleBox(rectTrans);
            // image 

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -19f, 40f);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -10f, rectTrans.rect.width + 19f);

            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -14f, 40f);
            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -10f, rectTrans.rect.width + 19f);

        }

        private void DialogSimpleBox(RectTransform rectTrans)
        {
            float thick = 0.2f * UIScaler.GetPixelsPerUnit();
            float anchor = 5f;

            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, anchor);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, rectTrans.rect.width);

            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, anchor);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, rectTrans.rect.width);

            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, rectTrans.rect.height);
            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, anchor);

            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, rectTrans.rect.height);
            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, anchor);

        }

        private void DialogSimpleBoxTwo(RectTransform rectTrans)
        {
            float anchor = 40f;

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -9.3f, anchor);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -8.5f, anchor);

            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -9.3f, anchor);
            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -5.2f, anchor);

            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5.2f, anchor);
            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -5.2f, anchor);

            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5.2f, anchor);
            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -8.5f, anchor);

        }


        private void CreateBorderDialogOne(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject[8];
            int[] angles = { 180, 0, 0, 180 };
            int[] anglesCorner = { 0, -90, 180, 90 };

            // create 4 lines
            for (int i = 0; i < 8; i++)
            {
                bLine[i] = new GameObject("BorderDialog" + i)
                {
                    tag = tag
                };
                if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<RawImage>().texture = CommonImageKeys.d2e_dlg_border_hor;
                    RectTransform rectTransform = bLine[i].GetComponent<RawImage>().rectTransform;
                    rectTransform.Rotate(new Vector3(0, 0, angles[i]));
                }
                else if (i == 2 || i == 3)
                {
                    bLine[i].AddComponent<RawImage>().texture = CommonImageKeys.d2e_dlg_border_ver;
                    RectTransform rectTransform = bLine[i].GetComponent<RawImage>().rectTransform;
                    rectTransform.Rotate(new Vector3(0, 0, angles[i]));
                }
                else if (i < 8)
                {
                    bLine[i].AddComponent<UnityEngine.UI.RawImage>().texture = CommonImageKeys.d2e_border_corner;
                    RectTransform rectTransform = bLine[i].GetComponent<UnityEngine.UI.RawImage>().rectTransform;
                    rectTransform.Rotate(new Vector3(0, 0, anglesCorner[i - 4]));
                }

                bLine[i].transform.SetParent(transform);
            }
            DialogSimpleBox(rectTrans);
            DialogSimpleBoxTwo(rectTrans);
        }

    }
}
