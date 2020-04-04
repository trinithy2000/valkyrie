using Assets.Scripts.Content;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UICharacterBorders
    {
        protected GameObject[] bLine;
        private readonly GameType gameType = new MoMGameType();

        public UICharacterBorders(UIElement element, StringKey content)
        {
            if (Game.Get().gameType is MoMGameType)
                CreateBorderCharacter(element.GetTransform(), element.GetRectTransform(), element.GetTag(), content, CommonImageKeys.mom_border_character);
            else if (Game.Get().gameType is D2EGameType)
                CreateBorderCharacter(element.GetTransform(), element.GetRectTransform(), element.GetTag(), content, CommonImageKeys.d2e_border_character);
        }


        private void CreateBorderCharacter(Transform transform, RectTransform rectTrans, string tag, StringKey content, Texture2D border)
        {
            bLine = new GameObject[2];

            bLine[0] = new GameObject("BorderCharTitle_0")
            {
                tag = tag
            };

            float height = rectTrans.rect.height * GameUtils.ReturnValueGameType<float>(1f, 1.55f);
            float width = rectTrans.rect.width * GameUtils.ReturnValueGameType<float>(1f, 1.3f);
         
            bLine[0].AddComponent<UnityEngine.UI.RawImage>().texture = border;
            bLine[0].transform.SetParent(transform);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, GameUtils.ReturnValueGameType<float>(0, -height/3.75f), height);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, GameUtils.ReturnValueGameType<float>(0, -width/ 9f), width);
            bLine[0].transform.SetAsFirstSibling();

            bLine[1] = new GameObject("NameChar_0")
            {
                tag = tag
            };

            float inset = rectTrans.rect.height / GameUtils.ReturnValueGameType<float>(6.9f,-10f);

            bLine[1].AddComponent<UnityEngine.UI.Text>().text = content.Translate();
            bLine[1].transform.SetParent(transform);
            bLine[1].GetComponent<UnityEngine.UI.Text>().color = Color.black;
            bLine[1].GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
            bLine[1].GetComponent<UnityEngine.UI.Text>().font = gameType.GetHeaderFont();
            bLine[1].GetComponent<UnityEngine.UI.Text>().fontSize = GameUtils.ReturnValueGameType<int>(UIScaler.GetSmallestFont(),UIScaler.GetSmallFont());
            bLine[1].GetComponent<UnityEngine.UI.Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            bLine[1].GetComponent<UnityEngine.UI.Text>().verticalOverflow = VerticalWrapMode.Overflow;
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, 10f);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -2, rectTrans.rect.width);
            bLine[1].transform.SetAsLastSibling();
        }
    }
}