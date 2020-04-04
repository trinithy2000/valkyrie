using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIElementBorder
    {
        protected GameObject[] bLine;
        protected Gradient g;

        public UIElementBorder(UIElement element)
        {
            CreateBorder(element.GetTransform(), element.GetRectTransform(), element.GetTag(), Color.white, 0.05f);
        }

        public UIElementBorder(UIElement element, Color color)
        {
            CreateBorder(element.GetTransform(), element.GetRectTransform(), element.GetTag(), color, 0.05f);
        }

        public UIElementBorder(Transform transform, RectTransform rectTrans, string tag, Color color)
        {
            CreateBorder(transform, rectTrans, tag, color, 0.05f);
        }
        public UIElementBorder(UIElement element, Color color, float thin)
        {
            CreateBorder(element.GetTransform(), element.GetRectTransform(), element.GetTag(), color, thin);
        }

        public UIElementBorder(UIElement element, float thin)
        {
            setGradient();
            Color color = g.Evaluate(.35f);
            CreateBorder(element.GetTransform(), element.GetRectTransform(), element.GetTag(), color, thin);
        }

        private void setGradient()
        {
            Color top = new Color(.56f, .42f, .16f, 1);
            Color mid = new Color(.56f, .42f, .16f, 1);
            Color bot = new Color(.56f, .42f, .16f, 1);

            GradientColorKey[] gck = new GradientColorKey[3];
            gck[0].color = top;
            gck[0].time = 1.0f;
            gck[1].color = mid;
            gck[1].time = 0.1f;
            gck[2].color = bot;
            gck[2].time = -1.0f;
            GradientAlphaKey[] gak = new GradientAlphaKey[3];
            gak[0].alpha = 0.9f;
            gak[0].time = 1f;
            gak[1].alpha = 0.9f;
            gak[1].time = 0f;
            gak[2].alpha = 0.9f;
            gak[2].time = -1f;
            g = new Gradient();
            g.SetKeys(gck, gak);
        }

        private void CreateBorder(Transform transform, RectTransform rectTrans, string tag, Color color, float thin)
        {
            bLine = new GameObject[4];

            // create 4 lines
            for (int i = 0; i < 4; i++)
            {
                bLine[i] = new GameObject("Border" + i)
                {
                    tag = tag
                };
                bLine[i].AddComponent<UnityEngine.UI.Image>().color = color;
                bLine[i].transform.SetParent(transform);
            }

            // Set the thickness of the lines
            float thick = thin * UIScaler.GetPixelsPerUnit();

            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, thick);
            bLine[0].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, rectTrans.rect.width + thick);

            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, thick);
            bLine[1].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, rectTrans.rect.width);

            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, rectTrans.rect.height);
            bLine[2].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -thick, thick);

            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -thick, rectTrans.rect.height);
            bLine[3].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, thick);
        }

        /// <summary>
        /// Set border color.</summary>
        /// <param name="color">Color to use.</param>
        public void SetColor(Color color)
        {
            foreach (GameObject line in bLine)
            {
                if (line != null)
                {
                    line.GetComponent<UnityEngine.UI.Image>().color = color;
                }
            }
        }
    }
}
