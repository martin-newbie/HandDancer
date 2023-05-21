using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{


    void Update()
    {
        GetInput();
    }

    void GetInput()
    {

        if(Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.K))
        {
            Debug.Log("both key (left keydown / right key)");
        }

    }
}
