using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.D2E
{
    public class UITitleBackGround_D2E : UITitleBackGround
    {
        protected GameObject bLine;

        public UITitleBackGround_D2E(UIElement element, string dialogType)
        {
            if (CommonString.title.Equals(dialogType))
            {
                CreateBgndTitle(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.description.Equals(dialogType))
            {
                CreateBgndDescription(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.image.Equals(dialogType))
            {
                CreateBgndImage(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.items.Equals(dialogType))
            {
                CreateItemsBar(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.itemTitle.Equals(dialogType))
            {
                CreateItemTittle(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
            else if (CommonString.puzzle.Equals(dialogType))
            {
                CreatePuzzleButton(element.GetTransform(), element.GetRectTransform(), element.GetTag());
            }
        }

        private void CreateBgndTitle(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndTitle_0")
            {
                tag = tag
            };
            float inset = rectTrans.rect.height / 14;
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.d2e_bar_menuTitle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -inset, 0, 1f, 1f });
        }

        private void CreateBgndDescription(Transform transform, RectTransform rectTrans, string tag)
        {
            float width = rectTrans.rect.width + 40f;
            float pixels = UIScaler.GetPixelsPerUnit() * 21;
            float inset = (UIScaler.GetHeightUnits() / 1.6f) + 20f;

            bLine = new GameObject("BgndTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.d2e_bgnd_pergam;
            SetRectTransformSimple(bLine, transform, rectTrans, new float[] { -inset, -20f, pixels, width });
        }

        private void CreateBgndImage(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_border_mansion;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -5f, -10f, 1.06f, 1.12f });
        }

        private void CreateItemsBar(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndTitle_0")
            {
                tag = tag
            };
            float inset = (rectTrans.rect.height * 1.15f) / 2;

            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_bar_items;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { inset, 10f, 0.85f, 0.95f });
        }

        private void CreateItemTittle(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_border_item_tittle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -9f, 9f, 1.72f, 0.9f });
        }

        private void CreatePuzzleButton(Transform transform, RectTransform rectTrans, string tag)
        {
            bLine = new GameObject("BgndPzlTitle_0")
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_btn_puzzle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -19f, 9f, 1.75f, 1f });
        }
    }
}
