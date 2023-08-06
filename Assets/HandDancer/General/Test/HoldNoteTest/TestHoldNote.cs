using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;

public class TestHoldNote : MonoBehaviour
{
    public Glow glow;
    LineRenderer lineRenderer;
    Vector3[] vertices;

    [SerializeField] int verticesCount = 5;
    [SerializeField, Range(0, 5)] float radius;
    [SerializeField, Range(-1, 1)] int dir;

    float dur = 5f;
    float cur = 5f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = verticesCount;
        vertices = new Vector3[verticesCount];
    }

    void Update()
    {
        float angle = 90f / (vertices.Length - 1);
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            vertices[i] = transform.position + new Vector3(Mathf.Cos((angle * i - 45f) * Mathf.Deg2Rad) * radius * dir, Mathf.Sin((angle * i - 45f) * Mathf.Deg2Rad) * radius);
        }

        cur -= Time.deltaTime;
        if (cur < 0) cur = dur;

        int count = vertices.Length - 1;
        float t = cur / dur;
        int idx = Mathf.CeilToInt(t * count);
        int nextIdx = idx - 1;

        float gap = 1f / (float)count;
        float next_t = (float)(nextIdx) / (float)count;
        float t2 = (t - next_t) / gap;

        Vector3[] t_vertices = new Vector3[verticesCount];
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            if (i >= idx)
            {
                t_vertices[i] = Vector3.Lerp(vertices[nextIdx], vertices[idx], t2);
            }
            else
            {
                t_vertices[i] = vertices[i];
            }
        }

        glow.SetRot(vertices[nextIdx], vertices[idx], t2);
        lineRenderer.SetPositions(t_vertices);
    }
}
