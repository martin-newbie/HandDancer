using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectPanel : MonoBehaviour
{
    public MapSelectUnit unitPrefab;
    public RectTransform unitParent;
    public List<MapSelectUnit> unitList = new List<MapSelectUnit>();

    public GameObject addNewButton;

    MusicData linkedData;

    public void OpenWindow(MusicData data, InGameState state)
    {
        gameObject.SetActive(true);
        linkedData = data;

        for (int i = unitList.Count; i < linkedData.difficultyCount; i++)
        {
            var temp = Instantiate(unitPrefab, unitParent);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150 - ((250 + 75) * i));
            unitList.Add(temp);
        }

        for (int i = 0; i < unitList.Count; i++)
        {
            if (i < linkedData.difficultyCount)
            {
                unitList[i].InitUnit(i, linkedData.difficultyName[i], linkedData.difficultyLevel[i], this);
            }
            else
            {
                unitList[i].gameObject.SetActive(false);
            }
        }

        unitParent.sizeDelta = new Vector2(980f, (25 + 250) * linkedData.difficultyCount + 25);

        addNewButton.SetActive(state == InGameState.Edit);
    }

    /// <summary>
    /// add new map at last
    /// </summary>
    public void AddNewMap()
    {
        linkedData.difficultyCount++;
        linkedData.difficultyName.Add("Untitled");
        linkedData.difficultyLevel.Add(0.0f);
        linkedData.stages.Add(new StageData(""));

        MenuManager.Instance.StartGame(linkedData.difficultyCount - 1);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public void SelectMusic(int idx)
    {
        MenuManager.Instance.StartGame(idx);
    }
}
