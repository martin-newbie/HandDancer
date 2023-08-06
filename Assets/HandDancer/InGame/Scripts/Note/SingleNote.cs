using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNote : Note
{
    public override void CheckState()
    {
        if (radius <= 4f && HitCondition())
        {
            if (radius >= 3.11f && radius <= 3.3f)
            {
                // perfect
                InGameManager.Instance.SetAnimState(dir, type);
                pushNote("perfect");
            }
            else
            {
                // great
                InGameManager.Instance.SetAnimState(dir, type);
                pushNote("great");
            }

        }

        if (radius <= 2.85f)
        {
            // miss
            pushNote("miss");
        }

    }

    protected override bool HitCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightDown : InGameManager.Instance.isLeftDown;
    }

    protected override IEnumerator MoveRoutine(float dur)
    {
        float timer = 0f;
        while (timer < dur)
        {
            radius = Mathf.Lerp(startRad, targetRad, timer / dur);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
