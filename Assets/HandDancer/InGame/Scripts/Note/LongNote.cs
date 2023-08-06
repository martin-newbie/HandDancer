using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class LongNote : Note
{
    float holdDur;
    float holdCur;
    int curState; // 0: begin hold, 1: hold

    public void InitLongNote(float holdDur)
    {
        this.holdDur = holdDur;
        holdCur = holdDur;
    }

    public override void CheckState()
    {
        if(radius <= 4f)
        {
            if (DownCondition() && curState == 0)
            {
                curState = 1;
                InGameManager.Instance.SetAnimState(dir, EHitState.LONG);
                if (moveRoutine != null) StopCoroutine(moveRoutine);
            }
            if(HoldCondition() && curState == 1)
            {
                holdCur -= Time.deltaTime;
                InGameManager.Instance.SetAnimState(dir, EHitState.LONG, true);
            }
            if(curState == 1 && holdCur <= 0)
            {
                InGameManager.Instance.SetAnimState(dir, EHitState.LONG_END, true);
                pushNote("hold end");
            }
        }
        
        if(radius <= 2.85f && curState == 0)
        {
            // miss
            pushNote("miss");
        }
        if(!HoldCondition() && curState == 1)
        {
            // miss
            pushNote("miss");
        }
    }

    protected override void pushNote(string condition)
    {
        base.pushNote(condition);
        curState = 0;
    }

    bool DownCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightDown : InGameManager.Instance.isLeftDown;
    }

    bool HoldCondition()
    {
        return dir == 1  ? InGameManager.Instance.isRightHold : InGameManager.Instance.isLeftHold;
    }

    protected override Vector3[] GetRenderLine()
    {
        var line = base.GetRenderLine();


        int count = line.Length - 1;
        float t = holdCur / holdDur;
        int idx = Mathf.CeilToInt(t * count);
        int nextIdx = idx - 1;

        float gap = 1f / (float)count;
        float next_t = (float)(nextIdx) / (float)count;
        float t2 = (t - next_t) / gap;

        Vector3[] t_vertices = new Vector3[vertices];
        for (int i = line.Length - 1; i >= 0; i--)
        {
            if (i >= idx)
            {
                t_vertices[i] = Vector3.Lerp(line[nextIdx], line[idx], t2);
            }
            else
            {
                t_vertices[i] = line[i];
            }
        }

        return t_vertices;
    }

    protected override bool HitCondition()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator MoveRoutine(float dur)
    {
        throw new System.NotImplementedException();
    }
}
