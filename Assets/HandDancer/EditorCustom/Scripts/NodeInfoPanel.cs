using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoPanel : MonoBehaviour
{
    [Header("Data")]
    public EditorNode linkedNode;
    public float nodeTimeSensitive= 0.01f;

    [Header("UI Setting Objects")]
    public Button rightButton;
    public Button leftButton;
    public InputField timeText;
    public NodeTypeDropdown dropDown;
    public GameObject nodeInfoPanelBackground;

    [Header("Sprite")]
    public Sprite rightSelect;
    public Sprite rightDeselect;
    public Sprite leftSelect;
    public Sprite leftDeselect;

    public void InitNode(EditorNode node)
    {
        linkedNode = node;
        nodeInfoPanelBackground.SetActive(true);
        dropDown.InitDropdownUI();

        InitUI();
    }


    public void DeselectNode()
    {
        linkedNode = null;
        nodeInfoPanelBackground.SetActive(false);
    }

    public void ChangeTime()
    {
        var nodeData = linkedNode.GetResultData();
        timeText.textComponent.text = nodeData.activeTime.ToString();
    }

    public void OnRightButtonClick()
    {
        var nodeData = linkedNode.GetResultData();
        switch (nodeData.nodePos)
        {
            case NodePos.LEFT:
                nodeData.nodePos = NodePos.BOTH;
                break;
            case NodePos.RIGHT:
                // error log : node have must more than one position
                break;
            case NodePos.BOTH:
                nodeData.nodePos = NodePos.LEFT;
                break;
        }

        linkedNode.InitNode(nodeData);
        InitUI();
    }
    public void OnLeftButtonClick()
    {
        var nodeData = linkedNode.GetResultData();
        switch (nodeData.nodePos)
        {
            case NodePos.LEFT:
                // error log : node have must more than one position
                break;
            case NodePos.RIGHT:
                nodeData.nodePos = NodePos.BOTH;
                break;
            case NodePos.BOTH:
                nodeData.nodePos = NodePos.RIGHT;
                break;
        }

        linkedNode.InitNode(nodeData);
        InitUI();
    }

    public void OnActiveTimeUp()
    {
        var nodeData = linkedNode.GetResultData();
        nodeData.ChangeTime(nodeData.activeTime + nodeTimeSensitive, EditorCustomManager.Instance.curMusic.length);

        linkedNode.InitNode(nodeData);
        InitUI();
    }
    public void OnActiveTimeDown()
    {
        var nodeData = linkedNode.GetResultData();
        nodeData.ChangeTime(nodeData.activeTime - nodeTimeSensitive, EditorCustomManager.Instance.curMusic.length);

        linkedNode.InitNode(nodeData);
        InitUI();
    }
    public void OnTimeInputChange()
    {
        var nodeData = linkedNode.GetResultData();
        float time = float.Parse(timeText.text);
        nodeData.activeTime = time;

        linkedNode.InitNode(nodeData);
    }
    
    public void OnDropdownValueChange()
    {
        var nodeData = linkedNode.GetResultData();
        int idx = dropDown.curIdx;
        nodeData.nodeType = (NodeType)idx;

        linkedNode.InitNode(nodeData);
    }

    void InitUI()
    {
        rightButton.image.sprite = linkedNode.GetResultData().nodePos == NodePos.RIGHT ? rightSelect : rightDeselect;
        leftButton.image.sprite = linkedNode.GetResultData().nodePos == NodePos.LEFT ? leftSelect : leftDeselect;

        timeText.text = linkedNode.GetResultData().activeTime.ToString();
        dropDown.InitDropdownMenu((int)linkedNode.GetResultData().nodeType);
    }
}
