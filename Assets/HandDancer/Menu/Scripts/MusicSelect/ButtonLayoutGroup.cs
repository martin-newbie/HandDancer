using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLayoutGroup : MonoBehaviour, IKeyInput
{
    public int allocCount = 7;
    public MusicButton btnPrefab;
    public Transform layoutGroup;
    public RectTransform disk;

    List<MusicButton> buttons = new List<MusicButton>();
    RectTransform rect;
    float radius;

    public int startIdx = 0;

    int topUnitIdx = 0;
    int highIdx; // highlighted index
    int totalCount = 100;

    int topIdx;
    int bottomIdx;


    IEnumerator Start()
    {
        rect = GetComponent<RectTransform>();
        radius = rect.sizeDelta.x / 2f + 50f;
        totalCount = UserData.Instance.musicDataList.Count;

        for (int i = 0; i < allocCount; i++)
        {
            var temp = Instantiate(btnPrefab, layoutGroup);
            buttons.Add(temp);
        }
        btnPrefab.gameObject.SetActive(false);
        yield return null;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].rect.anchoredPosition = GetCircularPos(i);
        }

        highIdx = (int)(allocCount / 2);
        int half = buttons.Count / 2;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < highIdx)
            {
                int calc = Mathf.Abs(totalCount - (half - i) + startIdx) % totalCount;
                if (i == 0) topIdx = calc;
                buttons[i].ChangeIndex(calc);
            }
            else
            {
                int calc = Mathf.Abs(i - highIdx + startIdx) % totalCount;
                if (i == buttons.Count - 1) bottomIdx = calc;
                buttons[i].ChangeIndex(calc);
            }
        }

        buttons[highIdx].SetHighlight();
    }

    float inputDelay = 0.075f, curDelay;

    bool upKey, downKey;

    void Update()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == highIdx)
            {
                buttons[i].rect.sizeDelta = Vector2.Lerp(buttons[i].rect.sizeDelta, new Vector2(900f, 300f), Time.deltaTime * 15f);
            }
            else
            {
                buttons[i].rect.sizeDelta = Vector2.Lerp(buttons[i].rect.sizeDelta, new Vector2(700f, 200f), Time.deltaTime * 15f);
            }
        }


        disk.Rotate(Vector3.forward * 20f * Time.deltaTime);
    }

    void KeyInput()
    {
        bool t_upKey = Input.GetKey(KeyCode.UpArrow);
        bool t_downKey = Input.GetKey(KeyCode.DownArrow);

        if (curDelay >= inputDelay)
        {
            int dir = t_upKey ? 1 : -1;
            ButtonMove(dir);
            inputDelay = 0.075f;
            curDelay = 0f;
        }

        if ((!t_upKey && upKey) || (!t_downKey && downKey)) curDelay = 0f;

        if ((t_upKey && upKey) || (t_downKey && downKey)) curDelay += Time.deltaTime;

        if ((t_upKey && !upKey) || (t_downKey && !downKey))
        {
            int dir = t_upKey ? 1 : -1;
            inputDelay = 0.25f;
            ButtonMove(dir);
        }

        upKey = t_upKey;
        downKey = t_downKey;

    }

    void ButtonMove(int dir)
    {
        int calc = topUnitIdx + dir;
        if (calc < 0) calc += buttons.Count;
        topUnitIdx = calc;

        int high = highIdx - dir;
        if (high < 0) high += buttons.Count;
        highIdx = high % buttons.Count;

        int topCalc = topIdx - dir;
        if (topCalc < 0) topCalc += totalCount;
        topIdx = topCalc % totalCount;

        int bottomCalc = bottomIdx - dir;
        if (bottomCalc < 0) bottomCalc += totalCount;
        bottomIdx = bottomCalc % totalCount;

        int degIdx;

        for (int i = 0; i < buttons.Count; i++)
        {
            degIdx = (topUnitIdx + i) % buttons.Count;
            Vector2 circularPos = GetCircularPos(degIdx);

            if (dir > 0 && degIdx == 0)
            {
                buttons[i].SetPos(circularPos);
                buttons[i].ChangeIndex(topIdx);
            }
            else if (dir < 0 && degIdx == buttons.Count - 1)
            {
                buttons[i].SetPos(circularPos);
                buttons[i].ChangeIndex(bottomIdx);
            }
            else buttons[i].SetMoveTargetPos(circularPos);
        }

        buttons[highIdx].SetHighlight();
    }

    Vector2 GetCircularPos(int idx)
    {
        float deg = 180f / (buttons.Count - 1);
        float startDeg = 90f - (deg * idx);

        return new Vector2(Mathf.Cos(startDeg * Mathf.Deg2Rad) * radius, Mathf.Sin(startDeg * Mathf.Deg2Rad) * radius);
    }

    public void OnUpdateKey()
    {
        KeyInput();
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    public void SetAbleObject()
    {
        gameObject.SetActive(true);
    }

    public MusicData GetCurrentMusicData()
    {
        return UserData.Instance.GetMusicData(buttons[highIdx].curIdx);
    }
}
