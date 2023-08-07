using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundIcon : MonoBehaviour
{
    public Material targetMat;
    public float speed = 1f;
    Vector2 offset;

    void Start()
    {
        
    }

    void Update()
    {
        offset.x += Time.deltaTime * speed;
        offset.y += Time.deltaTime * speed;
        targetMat.SetTextureOffset("_MainTex", offset);
    }
}
