using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    Perfact,
    Miss,
    Great
}

public class Note : MonoBehaviour
{
    int vertices = 15;

    EHitState type;
    LineRenderer line;
    float startRad = 10f;
    float targetRad = 0f;

    [SerializeField] int dir = 0;
    [SerializeField] float radius;

    bool isInit;

    public void Init(EHitState type, int dir, float dur)
    {
        this.type = type;
        this.dir = dir;

        isInit = true;
        StartCoroutine(MoveRoutine(dur));
    }

    IEnumerator MoveRoutine(float dur)
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

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void CheckState()
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

        void pushNote(string condition)
        {
            Debug.Log(condition);

            StopAllCoroutines();
            InGameManager.Instance.RemoveQueue(dir);
            InGameManager.Instance.PushNote(this);
        }
    }

    bool HitCondition()
    {
        return dir == 1 ? InGameManager.Instance.isRightDown : InGameManager.Instance.isLeftDown;
    }

    private void Update()
    {
        RenderLine();
    }

    void RenderLine()
    {
        // if (!isInit) return;

        Vector3[] verticePos = new Vector3[vertices + 1];
        float angle = 180f / vertices;
        for (int i = 0; i <= vertices; i++)
        {
            verticePos[i] = transform.position + new Vector3(Mathf.Cos((angle * i * dir - 90f * dir) * Mathf.Deg2Rad) * radius, Mathf.Sin((angle * i * dir - 90f * dir) * Mathf.Deg2Rad) * radius);
        }
        line.SetPositions(verticePos);

    }
}
