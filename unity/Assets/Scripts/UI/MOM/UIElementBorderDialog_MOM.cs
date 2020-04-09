using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MOM
{
    public class UIElementBorderDialog_MOM : UIElementBorderDialog
    {
        protected GameObject[] bLine;

        public UIElementBorderDialog_MOM(UIElement element, string dialogType)
        {
            transform = element.GetTransform();
            rectTrans = element.GetRectTransform();
            tag = element.GetTag();
            internalName = element.GetInternalName();

            if (CommonString.dialog.Equals(dialogType))
            {
                CreateBorderDialog();
            }
            else if (CommonString.dialogOne.Equals(dialogType))
            {
                CreateBorderDialogOne();
            }
            else if (CommonString.dialogTwo.Equals(dialogType))
            {
                CreateBorderDialogTwo();
            }
        }
        private void CreateBorderDialog()
        {
            bLine = new GameObject[6];

            // create 4 lines
            for (int i = 0; i < 6; i++)
            {
                bLine[i] = new GameObject("BorderDialog_" + internalName + "_" + bNames[i])
                {
                    tag = tag
                };
                if (i == 4)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarTop") as Texture2D;
                }
                else if (i == 5)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarBottom") as Texture2D;
                }
                else if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarVer") as Texture2D;
                }
                else
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarHor") as Texture2D;
                }
                bLine[i].transform.SetParent(transform);
            }

            // rectangle

            DialogSimpleBox();
            // image 

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -19f, 40f);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -10f, rectTrans.rect.width + 19f);

            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -14f, 40f);
            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -10f, rectTrans.rect.width + 19f);

        }

        private void DialogSimpleBox()
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

        private void DialogSimpleBoxTwo()
        {
            float anchor = 40f;

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -10.3f, anchor);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -9.5f, anchor);

            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -10f, anchor);
            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -8.2f, anchor);

            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -8.2f, anchor);
            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -7.9f, anchor);

            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -8.2f, anchor);
            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -9.5f, anchor);

        }


        private void CreateBorderDialogOne()
        {
            bLine = new GameObject[8];
            int[] angles = { 0, -90, 180, 90 }; 

            // create 4 lines
            for (int i = 0; i < 8; i++)
            {
                bLine[i] = new GameObject("BorderDialog_" + internalName + "_" + bNames[i])
                {
                    tag = tag
                };
                if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarH1") as Texture2D;
                }
                else if (i == 2 || i == 3)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarV1") as Texture2D;
                }
                else
                {
                    bLine[i].AddComponent<RawImage>().texture = CommonImageKeys.mom_border_corner;
                    RectTransform rectTransform = bLine[i].GetComponent<RawImage>().rectTransform;
                    rectTransform.Rotate(new Vector3(0, 0, angles[i - 4]));
                }
                bLine[i].transform.SetParent(transform);
            }
            DialogSimpleBox();
            DialogSimpleBoxTwo();
        }


        private void CreateBorderDialogTwo()
        {
            bLine = new GameObject[9];
            int[] angles = { 0, -90, 180, 90 };

            // create 4 lines
            for (int i = 0; i < 9; i++)
            {
                bLine[i] = new GameObject("BorderDialog_" + internalName + "_" + bNames[i])
                {
                    tag = tag
                };
                if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarH1") as Texture2D;
                }
                else if (i == 2 || i == 3)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/mom/dlgBarV1") as Texture2D;
                }
                else if (i < 8)
                {
                    bLine[i].AddComponent<RawImage>().texture = CommonImageKeys.mom_border_corner;
                    RectTransform rectTransform = bLine[i].GetComponent<RawImage>().rectTransform;
                    rectTransform.Rotate(new Vector3(0, 0, angles[i - 4]));
                }
                else
                {
                    bLine[i].AddComponent<RawImage>().texture = CommonImageKeys.mom_arrow_dlg;
                    bLine[i].transform.SetParent(transform);
                }
                bLine[i].transform.SetParent(transform);
            }
            DialogSimpleBox();
            DialogSimpleBoxTwo();


            bLine[8].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -38f, 40f);
            bLine[8].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (rectTrans.rect.width / 2) - 40f, 80f);

        }
    }
}
