using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorNodeScroll : MonoBehaviour
{
    public RectTransform nodeScroll;

    public void InitNodeScroll(float dur)
    {
        var defaultSize = nodeScroll.sizeDelta;
        defaultSize.y = dur * 1000f;

        nodeScroll.sizeDelta = defaultSize;
    }
}
