using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorNode : MonoBehaviour, IDragHandler
{
    [Header("Sprite")]
    public Sprite nodeSelect;
    public Sprite nodeUnselect;

    [Header("UI")]
    public Image nodeImg;
    public float curTime => thisData.activeTime;
    NodeData thisData;

    [HideInInspector] public RectTransform rect;

    public void InitNode(NodeData data)
    {
        rect = GetComponent<RectTransform>();
        
        thisData = data;

        float xPos;
        switch (thisData.nodePos)
        {
            case NodePos.LEFT:
                xPos = -200f;
                break;
            case NodePos.RIGHT:
                xPos = 200f;
                break;
            default:
                xPos = 0f;
                break;
        }
        rect.anchoredPosition = new Vector2(xPos, curTime * 1000f);
    }

    public void SelectNode()
    {
        nodeImg.sprite = nodeSelect;
    }
    public void UnselectNode()
    {
        nodeImg.sprite = nodeUnselect;
    }

    Vector2 interval;
    public void OnSelectButton()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        interval = mousePos - transform.position;

        EditorCustomManager.Instance.ChangeNode(this);
    }

    public void ChangeTime(float time)
    {
        thisData.activeTime = time;
        rect.anchoredPosition = new Vector2(0, curTime * 100f);
    }

    public NodeData GetResultData()
    {
        return thisData;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (EditorCustomManager.Instance.curNode != this) return;

        // step 1. follow pos
        // step 2. set time as pos
        // step 3. initialize info ui

        Vector3 pos = transform.position;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector3(pos.x, (mousePos - (Vector3)interval).y);
        transform.position = pos;

        thisData.activeTime = rect.anchoredPosition.y / 1000f;
        EditorCustomManager.Instance.nodeInfo.ChangeTime();
    }
}
