using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InGameState
{
    Play,
    Edit
}

public class ModeSelectPanel : MonoBehaviour
{
    public InGameState gameState;

    public void OnPlayButton()
    {
        gameState = InGameState.Play;
        MenuManager.Instance.OpenMapSelect();
    }

    public void OnEditButton()
    {
        gameState = InGameState.Edit;
        MenuManager.Instance.OpenMapSelect();
    }
}
