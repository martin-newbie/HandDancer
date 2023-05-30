using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeTypeDropdown : MonoBehaviour
{
    public NodeInfoPanel infoPanel;

    [Header("Sprite")]
    public Sprite[] nodeSprites;

    [Header("UI")]
    public Image graphic;
    public Transform itemContent;
    public Button itemPrefab;
    public ScrollRect dropdownObject;

    public int curIdx = 0;


    public void InitDropdownUI(int curIdx)
    {
        int typeCount = Enum.GetValues(typeof(NodeType)).Length;

        for (int i = 0; i < typeCount; i++)
        {
            var tempBtn = Instantiate(itemPrefab, itemContent);

            int idx = i;
            tempBtn.onClick.AddListener(() => OnDropdownButtonDown(idx));
            tempBtn.image.sprite = nodeSprites[i];
        }
        graphic.sprite = nodeSprites[curIdx];
        itemPrefab.gameObject.SetActive(false);
    }

    public void InitDropdownMenu(int openIdx = 0)
    {
        dropdownObject.gameObject.SetActive(true);
        dropdownObject.verticalScrollbar.value = 1f;

        OnDropdownButtonDown(openIdx);
    }
    public void OnDropdownButton()
    {
        if (dropdownObject.gameObject.activeInHierarchy)
        {
            CloseDropdown();
        }
        else
        {
            dropdownObject.gameObject.SetActive(true);
        }
    }
    public void CloseDropdown()
    {
        dropdownObject.gameObject.SetActive(false);
    }

    void OnDropdownButtonDown(int idx)
    {
        curIdx = idx;
        infoPanel.OnDropdownValueChange();
        CloseDropdown();
    }
}
