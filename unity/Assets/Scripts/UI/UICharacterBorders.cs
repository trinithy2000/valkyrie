using Assets.Scripts.Content;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UICharacterBorders
    {
        protected GameObject[] bLine;
        private new readonly Transform transform;
        private new readonly RectTransform rectTrans;
        private new readonly string tag;


        public UICharacterBorders(UIElement element, StringKey content)
        {
            transform = element.GetTransform();
            rectTrans = element.GetRectTransform();
            tag = element.GetTag();

            if (GameUtils.IsMoMGameType())
                CreateBorderCharacter(content, CommonImageKeys.mom_border_character);
            else if (GameUtils.IsD2EGameType())
                CreateBorderCharacter(content, CommonImageKeys.d2e_border_character);
            else if (GameUtils.IsIAGameType())
                CreateBorderCharacterIA();
        }

        private void CreateBorderCharacter(StringKey content, Texture2D border)
        {
            bLine = new GameObject[2];

            bLine[0] = new GameObject("BorderCharTitle_" + content)
            {
                tag = tag
            };

            float height = rectTrans.rect.height * GameUtils.ReturnValueGameType<float>(1f, 1.55f, 1f);
            float width = rectTrans.rect.width * GameUtils.ReturnValueGameType<float>(1f, 1.3f, 1f);

            bLine[0].AddComponent<RawImage>().texture = border;
            bLine[0].transform.SetParent(transform);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, GameUtils.ReturnValueGameType<float>(0, -height / 3.75f, 0), height);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, GameUtils.ReturnValueGameType<float>(0, -width / 9f, 0), width);
            bLine[0].transform.SetAsFirstSibling();

            bLine[1] = new GameObject("NameChar_" + content)
            {
                tag = tag
            };

            float inset = rectTrans.rect.height / GameUtils.ReturnValueGameType<float>(6.9f, -10f, 6.9f);

            bLine[1].AddComponent<Text>().text = content.Translate();
            bLine[1].transform.SetParent(transform);
            bLine[1].GetComponent<Text>().color = Color.black;
            bLine[1].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            bLine[1].GetComponent<Text>().font = Game.Get().gameType.GetHeaderFont();
            bLine[1].GetComponent<Text>().fontSize = GameUtils.ReturnValueGameType<int>(UIScaler.GetSmallestFont(), UIScaler.GetSmallFont(), UIScaler.GetSmallestFont());
            bLine[1].GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            bLine[1].GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, 10f);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -2, rectTrans.rect.width);
            bLine[1].transform.SetAsLastSibling();
        }


        private void CreateBorderCharacterIA()
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
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTrans.rect.width *.53f);

            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 4);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTrans.rect.width);

            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTrans.rect.height + 4);
            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -4, 4); 

            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTrans.rect.height*.92f);
            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -4, 4);

            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 15.4f, 5.8f);
            bLine[4].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -9.5f, 70);
            CommonScriptFuntions.RotateGameObject(bLine[4], 35);
        }

    }

}
