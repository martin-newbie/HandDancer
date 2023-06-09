using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    Perfact,
    Miss,
    Great
}

public class Node : MonoBehaviour
{
    int vertices = 15;

    NodeType type;
    LineRenderer line;
    float startRad;
    float targetRad;
    int dir = 0;

    float radius;

    bool isInit;

    public void Init(NodeType type, int dir, float startRad, float targetRad)
    {
        line = GetComponent<LineRenderer>();

        this.type = type;
        this.dir = dir;
        this.startRad = startRad;
        this.targetRad = targetRad;


        isInit = true;
        StartCoroutine(MoveRoutine(3f));
    }

    IEnumerator MoveRoutine(float dur)
    {
        Vector3 start = new Vector3(5f * dir, 0);
        Vector3 end = Vector3.zero;
        transform.position = start;

        float timer = 0f;
        while (timer < dur)
        {
            transform.position = Vector3.Lerp(start, end, timer / dur);
            radius = Mathf.Lerp(startRad, targetRad, timer / dur);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    private void Update()
    {
        RenderLine();
    }

    void RenderLine()
    {
        if (!isInit) return;

        Vector3[] verticePos = new Vector3[vertices + 1];
        float angle = 180f / vertices;
        for (int i = 0; i <= vertices; i++)
        {
            verticePos[i] = transform.position + new Vector3(Mathf.Cos((angle * i * dir - 90f * dir) * Mathf.Deg2Rad) * radius, Mathf.Sin((angle * i * dir - 90f * dir) * Mathf.Deg2Rad) * radius);
        }
        line.SetPositions(verticePos);

    }

    
}
