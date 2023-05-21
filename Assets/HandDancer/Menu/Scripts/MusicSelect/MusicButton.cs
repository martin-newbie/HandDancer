using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    public Text indexTxt;
    public Text nameTxt;
    public Text timeTxt;

    [HideInInspector] public Vector2 targetPos;
    [HideInInspector] public RectTransform rect;

    float moveSpeed = 25f;
    bool isInit = false;
    [HideInInspector] public int curIdx = 0;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isInit) return;

        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
    }

    public void SetMoveTargetPos(Vector2 _targetPos)
    {
        if (!isInit) isInit = true;

        targetPos = _targetPos;
    }

    public void SetPos(Vector2 _taretPos)
    {
        if (!isInit) isInit = true;

        targetPos = _taretPos;
        rect.anchoredPosition = targetPos;
    }

    public void ChangeIndex(int idx)
    {
        indexTxt.text = (idx + 1).ToString();

        nameTxt.text = UserData.Instance.musicDataList[idx].songName;
        var clip = UserData.Instance.musicDataList[idx].audio;

        float minute = (int)clip.length / 60;
        float second = (int)clip.length % 60;
        StringBuilder sb = new StringBuilder();
        sb.Append(minute.ToString());
        sb.Append(":");

        string s_second = second >= 10f ? second.ToString() : $"0{second.ToString()}";
        sb.Append(s_second);
        timeTxt.text = sb.ToString();

        curIdx = idx;
    }

    public void SetHighlight()
    {
        MenuManager.Instance.ChangeSelectedMusic(curIdx);
    }
}
