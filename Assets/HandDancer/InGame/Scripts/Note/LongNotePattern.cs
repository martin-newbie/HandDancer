using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNotePattern : INotePattern
{
    // about subject
    private Note subject;
    int verticesCount => subject.vertices;
    float rad => subject.rad;
    int dir => subject.dir;



    // local uses
    float holdDur;
    float holdCur;
    int curState;

    public LongNotePattern(Note note)
    {
        subject = note;
        holdDur = subject.dur;
        holdCur = holdDur;
    }

    public void CheckState()
    {
        if (rad <= 4f)
        {
            if (DownCondition() && curState == 0)
            {
                curState = 1;
                InGameManager.Instance.SetAnimState(dir, EHitState.HOLD);
                subject.TryStopMoveCoroutine();
            }
            if (HoldCondition() && curState == 1)
            {
                holdCur -= Time.deltaTime;
                InGameManager.Instance.SetAnimState(dir, EHitState.HOLD, true);
            }
            if (curState == 1 && holdCur <= 0)
            {
                InGameManager.Instance.SetAnimState(dir, EHitState.HOLD_END);
                subject.pushNote("hold end");
            }
        }

        if (rad <= 2.85f && curState == 0)
        {
            subject.pushNote("miss");
        }
        if (!HoldCondition() && curState == 1)
        {
            subject.pushNote("miss");
        }
    }

    public Vector3[] GetRenderLine()
    {
        var vertices = subject.GetRenderLine();
        int count = vertices.Length - 1;
        float t = holdCur / holdDur;
        int idx = Mathf.CeilToInt(t * count);
        int nextIdx = idx - 1;

        float gap = 1f / (float)count;
        float next_t = (float)(nextIdx) / (float)count;
        float gap_t = (t - next_t) / gap;

        Vector3[] resultVertices = new Vector3[verticesCount];
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            if (i >= idx)
            {
                resultVertices[i] = Vector3.Lerp(vertices[nextIdx], vertices[idx], gap_t);
            }
            else
            {
                resultVertices[i] = vertices[i];
            }
        }

        return resultVertices;
    }

    bool DownCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightDown : InGameManager.Instance.isLeftDown;
    }

    bool HoldCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightHold : InGameManager.Instance.isLeftHold;
    }
}
