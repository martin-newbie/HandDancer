using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNotePattern : INotePattern
{
    Note subject;

    float rad => subject.rad;
    int dir => subject.dir;
    

    public SingleNotePattern(Note note)
    {
        subject = note;
    }

    public void CheckState()
    {
        if (rad <= 4f && DownCondition())
        {
            if (rad >= 3.11f && rad <= 3.3f)
            {
                // perfect
                InGameManager.Instance.SetAnimState(dir, EHitState.DOWN);
                subject.pushNote("perfect");
            }
            else
            {
                // great
                InGameManager.Instance.SetAnimState(dir, EHitState.DOWN);
                subject.pushNote("great");
            }

        }

        if (rad <= 2.85f)
        {
            // miss
            subject.pushNote("miss");
        }

    }

    public Vector3[] GetRenderLine()
    {
        return subject.GetRenderLine();
    }

    bool DownCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightDown : InGameManager.Instance.isLeftDown;
    }
}
