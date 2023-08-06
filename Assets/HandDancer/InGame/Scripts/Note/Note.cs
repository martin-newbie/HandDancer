using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    Perfact,
    Miss,
    Great
}

public abstract class Note : MonoBehaviour
{
    [HideInInspector] public int vertices = 15;

    protected EHitState type;
    protected LineRenderer line;
    protected float startRad = 10f;
    protected float targetRad = 0f;

    protected int dir = 0;
    protected float radius;

    protected bool isInit;
    protected Coroutine moveRoutine;

    public virtual void Init(EHitState type, int dir, float dur)
    {
        this.type = type;
        this.dir = dir;

        isInit = true;
        radius = startRad;
        moveRoutine = StartCoroutine(MoveRoutine(dur));
    }

    protected abstract IEnumerator MoveRoutine(float dur);


    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = vertices;
    }

    public abstract void CheckState();
    protected virtual void pushNote(string condition)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        radius = startRad;
        InGameManager.Instance.RemoveQueue(dir);
        InGameManager.Instance.PushNote(this);
    }

    protected abstract bool HitCondition();

    private void Update()
    {
        RenderLine(GetRenderLine());
    }

    protected virtual Vector3[] GetRenderLine()
    {
        // if (!isInit) return;

        Vector3[] verticePos = new Vector3[vertices + 1];
        float angle = 180f / vertices;
        for (int i = 0; i <= vertices; i++)
        {
            verticePos[i] = transform.position + new Vector3(Mathf.Cos((angle * i * dir - 90f) * Mathf.Deg2Rad) * radius, Mathf.Sin((angle * i * dir - 90f) * Mathf.Deg2Rad) * radius);
        }

        return verticePos;
    }

    protected void RenderLine(Vector3[] verticePos)
    {
        line.SetPositions(verticePos);
    }

}
