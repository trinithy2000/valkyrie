using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.D2E
{
    public class UITitleBackGround_D2E : UITitleBackGround
    {
        protected GameObject bLine;

        public UITitleBackGround_D2E(UIElement element, string dialogType)
        {
            transform = element.GetTransform();
            rectTrans = element.GetRectTransform();
            tag = element.GetTag();
            internalName = element.GetInternalName();

            if (CommonString.title.Equals(dialogType))
            {
                CreateBgndTitle();
            }
            else if (CommonString.description.Equals(dialogType))
            {
                CreateBgndDescription();
            }
            else if (CommonString.image.Equals(dialogType))
            {
                CreateBgndImage();
            }
            else if (CommonString.items.Equals(dialogType))
            {
                CreateItemsBar();
            }
            else if (CommonString.itemTitle.Equals(dialogType))
            {
                CreateItemTittle();
            }
            else if (CommonString.puzzle.Equals(dialogType))
            {
                CreatePuzzleButton();
            }
        }

        private void CreateBgndTitle()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            float inset = rectTrans.rect.height / 14;
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.d2e_bar_menuTitle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -inset, 0, 1f, 1f });
        }

        private void CreateBgndDescription()
        {
            float width = rectTrans.rect.width + 40f;
            float pixels = UIScaler.GetPixelsPerUnit() * 21;
            float inset = (UIScaler.GetHeightUnits() / 1.6f) + 20f;

            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.d2e_bgnd_pergam;
            SetRectTransformSimple(bLine, transform, new float[] { -inset, -20f, pixels, width });
        }

        private void CreateBgndImage()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_border_mansion;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -5f, -10f, 1.06f, 1.12f });
        }

        private void CreateItemsBar()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            float inset = (rectTrans.rect.height * 1.15f) / 2;

            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_bar_items;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { inset, 10f, 0.85f, 0.95f });
        }

        private void CreateItemTittle()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_border_item_tittle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -9f, 9f, 1.72f, 0.9f });
        }

        private void CreatePuzzleButton()
        {
            bLine = new GameObject("BgndPzlTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.mom_btn_puzzle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -19f, 9f, 1.75f, 1f });
        }
    }
}
