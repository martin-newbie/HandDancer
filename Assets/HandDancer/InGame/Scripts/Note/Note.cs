using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    Perfact,
    Miss,
    Great
}

public interface INotePattern
{
    void CheckState();
    Vector3[] GetRenderLine();
}

public class Note : MonoBehaviour
{

    LineRenderer line;
    INotePattern pattern;
    Coroutine moveRoutine;

    // static
    [HideInInspector] public int vertices = 15;
    [HideInInspector] public float startRad = 10f;
    [HideInInspector] public float targetRad = 0f;

    // dynamic
    [HideInInspector] public int dir;
    [HideInInspector] public float dur; // only used in hold note
    [HideInInspector] public float rad; // not edited in initailizer
    [HideInInspector] public ENoteType type;



    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = vertices;
    }

    public virtual void Init(ENoteType type, int dir, float speed, float dur)
    {
        this.type = type;
        this.dir = dir;
        this.dur = dur;

        // initialize pattern
        switch (type)
        {
            case ENoteType.SINGLE:
                pattern = new SingleNotePattern(this);
                break;
            case ENoteType.LONG:
                pattern = new LongNotePattern(this);
                break;
        }

        rad = startRad;
        moveRoutine = StartCoroutine(MoveRoutine(speed));
    }

    protected IEnumerator MoveRoutine(float speed)
    {
        float dur = startRad / speed;
        float timer = 0f;
        while (timer < dur)
        {
            rad = Mathf.Lerp(startRad, targetRad, timer / dur);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }



    public void CheckState()
    {
        pattern.CheckState();
    }

    public virtual void pushNote(string condition)
    {
        TryStopMoveCoroutine();

        rad = startRad;
        InGameManager.Instance.RemoveQueue(dir);
        InGameManager.Instance.PushNote(this);
    }

    private void Update()
    {
        RenderLine(pattern.GetRenderLine());
    }

    public Vector3[] GetRenderLine()
    {
        Vector3[] verticePos = new Vector3[vertices];
        float angle = 90f;
        float gap = angle / (vertices - 1);
        for (int i = vertices - 1; i >= 0; i--)
        {
            verticePos[i] = transform.position + new Vector3(Mathf.Cos((gap * i- (angle / 2f)) * Mathf.Deg2Rad) * rad * dir, Mathf.Sin((gap * i - (angle / 2f)) * Mathf.Deg2Rad) * rad);
        }

        return verticePos;
    }

    public void RenderLine(Vector3[] verticePos)
    {
        line.SetPositions(verticePos);
    }

    public void TryStopMoveCoroutine()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
    }
}
