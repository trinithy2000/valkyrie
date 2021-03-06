﻿using Assets.Scripts.UI.MOM;
using Assets.Scripts.UI.D2E;
using Assets.Scripts.UI.IA;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UITitleBackGround
    {
        protected Transform transform;
        protected RectTransform rectTrans;
        protected string tag;
        protected string internalName;

        public UITitleBackGround(UIElement element, string dialogType)
        {
            if (Game.Get().gameType is MoMGameType)
                new UITitleBackGround_MOM(element, dialogType);
            else if (Game.Get().gameType is D2EGameType)
                new UITitleBackGround_D2E(element, dialogType);
            else if (Game.Get().gameType is IAGameType)
                new UITitleBackGround_IA(element, dialogType);
        }
        public UITitleBackGround()
        {
        }
        protected void SetRectTransform(GameObject bline, Transform transform, RectTransform rectTrans, float[] values)
        {
            values[2] = rectTrans.rect.height * values[2];
            values[3] = rectTrans.rect.width * values[3];
            SetRectTransformSimple(bline, transform, values);
        }

        protected void SetRectTransformDown(GameObject bline, Transform transform, RectTransform rectTrans, float[] values)
        {
            values[2] = rectTrans.rect.height * values[2];
            values[3] = rectTrans.rect.width * values[3];
            SetRectTransformSimpleDown(bline, transform, values);
        }
        protected void SetRectTransformSimple(GameObject bline, Transform transform, float[] values)
        {
            bline.transform.SetParent(transform);
            bline.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, values[0], values[2]);
            bline.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, values[1], values[3]);
            bline.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();
        }

        protected void SetRectTransformSimpleDown(GameObject bline, Transform transform, float[] values)
        {
            bline.transform.SetParent(transform);
            bline.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, values[0], values[2]);
            bline.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, values[1], values[3]);
            bline.transform.SetAsFirstSibling();
            transform.SetAsLastSibling();
        }
    }
}