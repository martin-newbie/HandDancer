using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTimeScroll : MonoBehaviour
{
    public Slider slider;
    public InputField timeInput;

    float totalDur = 0f;
    public float curtime;

    public void InitScroll(float dur)
    {
        this.totalDur = dur;
    }

    public void OnValueChange()
    {
        curtime = totalDur * slider.value;
        timeInput.text = curtime.ToString();
    }

    public void OnTimeInputValueChange()
    {
        float tempTime = float.Parse(timeInput.text);
        tempTime = Mathf.Clamp(tempTime, 0f, totalDur);
        TimeScrolling(tempTime);
    }

    public void TimeScrolling(float time)
    {
        curtime = time;
        slider.value = time / totalDur;
    }
}
