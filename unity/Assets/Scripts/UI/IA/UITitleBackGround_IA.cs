using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.IA
{
    public class UITitleBackGround_IA : UITitleBackGround
    {
        protected GameObject bLine;

        public UITitleBackGround_IA(UIElement element, string dialogType)
        {
            transform = element.GetTransform();
            rectTrans = element.GetRectTransform();
            tag = element.GetTag();
            internalName = element.GetInternalName();

            if (CommonString.title.Equals(dialogType))
            {
                CreateBgndTitle();
            }
            else if (CommonString.items.Equals(dialogType))
            {
                CreateItemsBar();
            }
            else if (CommonString.description.Equals(dialogType))
            {
                CreateBgndDescription();
            }
            else if (CommonString.image.Equals(dialogType))
            {
                CreateBgndImage();
            }
        }

        private void CreateBgndTitle()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            float insetW = rectTrans.rect.width / 2;
            float insetH = rectTrans.rect.height / 14;
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.ia_bar_menuTitle;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -insetH, -insetW, 1f, 2f });
        }

        private void CreateItemsBar()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };

            float insetW = rectTrans.rect.width / 2.55f;
            float insetH = rectTrans.rect.height;

            bLine.AddComponent<RawImage>().texture = CommonImageKeys.ia_bgnd_down_bar;
            base.SetRectTransformDown(bLine, transform, rectTrans, new float[] { 0, -insetW, 1.1f, 1.8f });
        }

        private void CreateBgndImage()
        {
            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.ia_border_image;
            base.SetRectTransform(bLine, transform, rectTrans, new float[] { -5f, -18f, 1.06f, 1.12f });
        }

        private void CreateBgndDescription()
        {
            float width = rectTrans.rect.width + 30f;
            float pixels = UIScaler.GetPixelsPerUnit() * 18;

            bLine = new GameObject("BgndTitle_" + internalName)
            {
                tag = tag
            };
            bLine.AddComponent<RawImage>().texture = CommonImageKeys.ia_button_box_trans;
            SetRectTransformSimple(bLine, transform, new float[] { 0, -20f, pixels, width });
        }


    }
}
