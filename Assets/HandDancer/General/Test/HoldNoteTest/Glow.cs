using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public Sprite[] sprites;
    int idx;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sr.sprite = sprites[idx];

        idx++;
        if (idx > sprites.Length - 1) idx = 0;
    }

    public void SetRot(Vector3 cur, Vector3 next, float t)
    {
        transform.position = Vector3.Lerp(cur, next, t);

        var dir = next - cur;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }
}
