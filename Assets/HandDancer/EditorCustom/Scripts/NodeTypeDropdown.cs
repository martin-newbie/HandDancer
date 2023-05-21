using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeTypeDropdown : MonoBehaviour
{
    public NodeInfoPanel infoPanel;

    // 스프라이트 받아서 추가하는거 필요함
    public Image graphic;
    public Transform itemContent;
    public Button itemPrefab;
    public ScrollRect dropdownObject;

    public int curIdx = 0;


    public void InitDropdownUI()
    {
        int typeCount = Enum.GetValues(typeof(NodeType)).Length;

        for (int i = 0; i < typeCount; i++)
        {
            var tempBtn = Instantiate(itemPrefab, itemContent);

            int idx = i;
            tempBtn.onClick.AddListener(() => OnDropdownButtonDown(idx));
        }
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
