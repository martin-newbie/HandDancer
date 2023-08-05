using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUIManager : MonoBehaviour
{
    public Image rightInputCheck;
    public Image leftInputCheck;

    public RectTransform judgeLine;

    void Start()
    {
        
    }

    private void Update()
    {
        judgeLine.Rotate(Vector3.forward * Time.deltaTime * 25f);
    }

    public void SetInputCheckActive(int dir, bool active = true)
    {
        switch (dir)
        {
            case -1:
                leftInputCheck.gameObject.SetActive(active);
                rightInputCheck.gameObject.SetActive(false);
                break;
            case 1:
                rightInputCheck.gameObject.SetActive(active);
                leftInputCheck.gameObject.SetActive(false);
                break;
            case 0:
                leftInputCheck.gameObject.SetActive(active);
                rightInputCheck.gameObject.SetActive(active);
                break;
        }
    }
}
