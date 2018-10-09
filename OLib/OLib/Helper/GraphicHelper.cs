using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OLib
{
    public partial class GraphicHelper
    {
        public static void getSpriteColors(Transform transform, Dictionary<string, Color> colors, bool isUi = false)
        {
            if (isUi)
            {
                Image img = transform.gameObject.GetComponent<Image>();
                if (null != img)
                {
                    if (!colors.ContainsKey(img.name))
                        colors.Add(img.name, img.color);

                }
                else
                {
                    Text text = transform.gameObject.GetComponent<Text>();
                    {
                        if (null != text)
                        {
                            if (!colors.ContainsKey(text.name))
                                colors.Add(text.name, text.color);
                        }
                    }
                }
            }
            else
            {
                SpriteRenderer sr = transform.gameObject.GetComponent<SpriteRenderer>();
                if (null != sr)
                {
                    if (!colors.ContainsKey(sr.name))
                        colors.Add(sr.name, sr.color);
                }
            }

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                getSpriteColors(transform.GetChild(i), colors, isUi);
            }
        }

        public static void setSpriteColors(Transform transform, Dictionary<string, Color> colors, bool isUi = false)
        {
            if (isUi)
            {
                Image img = transform.gameObject.GetComponent<Image>();
                if (null != img)
                {
                    Color color;
                    if (colors.TryGetValue(img.name, out color))
                    {
                        img.color = color;
                    }
                }
                else
                {
                    Text text = transform.gameObject.GetComponent<Text>();
                    {
                        if (null != text)
                        {
                            Color color;
                            if (colors.TryGetValue(text.name, out color))
                            {
                                text.color = color;
                            }
                        }
                    }
                }
            }
            else
            {
                SpriteRenderer sr = transform.gameObject.GetComponent<SpriteRenderer>();
                if (null != sr)
                {
                    Color color;
                    if (colors.TryGetValue(sr.name, out color))
                    {
                        sr.color = color;
                    }
                }
            }

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                setSpriteColors(transform.GetChild(i), colors, isUi);
            }
        }
    }
}
