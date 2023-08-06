using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    bool left, right;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !right) right = true;
        if (Input.GetKeyDown(KeyCode.F) && !left) left = true;
    }

    void FixedUpdate()
    {
        if(right && left)
        {
            Debug.Log("is both down");

        }
        else if(right && !left)
        {
            Debug.Log("is right down");
        }
        else if(!right && left)
        {
            Debug.Log("is left down");
        }

        right = false;
        left = false;
    }

}
