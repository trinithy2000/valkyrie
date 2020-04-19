using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.IA
{
    public class UIElementBorderDialog_IA : UIElementBorderDialog
    {
        protected GameObject[] bLine;


        public UIElementBorderDialog_IA(UIElement element, string dialogType)
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
                element.SetImage(CommonImageKeys.ia_bgnd_autor);
                CreateBorderDialogOne();
            }

        }
        private void CreateBorderDialog()
        {
            bLine = new GameObject[6];

            // create 4 lines
            for (int i = 0; i < 6; i++)
            {
                bLine[i].tag = tag;
                if (i == 4)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarTop") as Texture2D;
                }
                else if (i == 5)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarBottom") as Texture2D;
                }
                else if (i == 0 || i == 1)
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarVer") as Texture2D;
                }
                else
                {
                    bLine[i].AddComponent<RawImage>().texture = Resources.Load("sprites/borders/d2e/dlgBarHor") as Texture2D;
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

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -9.3f, anchor);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -8.5f, anchor);

            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -9.3f, anchor);
            bLine[5].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -5.2f, anchor);

            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5.2f, anchor);
            bLine[6].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -5.2f, anchor);

            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -5.2f, anchor);
            bLine[7].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -8.5f, anchor);

        }


        private void CreateBorderDialogOne()
        {

            bLine = new GameObject[5];

            // create 4 lines
            for (int i = 0; i < 5; i++)
            {
                bLine[i] = new GameObject("Border" + i)
                {
                    tag = tag
                };

                bLine[i].AddComponent<Image>().color = Color.white;
                bLine[i].transform.SetParent(transform);
                bLine[i].transform.SetAsLastSibling();
            }


            // Set the thickness of the lines
            float thick = 1 * UIScaler.GetPixelsPerUnit();

            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -4, 4);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTrans.rect.width * .75f);

            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 4);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTrans.rect.width);

            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTrans.rect.height + 4);
            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -4, 4);

            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTrans.rect.height * .70f);
            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -4, 4);

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 18.5f, 5.7f);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -8f, 101);
            CommonScriptFuntions.RotateGameObject(bLine[4], 27);
           
        }

    }

}
