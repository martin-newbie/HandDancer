using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUnit : MonoBehaviour
{
    public Text difficultyTitle;
    public Text difficultyLevel;
    public int thisIdx;

    MapSelectPanel managerPanel;

    public void InitUnit(int idx, string title, float level, MapSelectPanel panel)
    {
        difficultyTitle.text = title;
        difficultyLevel.text = level.ToString();

        thisIdx = idx;
        managerPanel = panel;
    }

    public void OnSelectButton()
    {
        managerPanel.SelectMusic(thisIdx);
    }
}
