using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputManager : MonoBehaviour
{
    private static KeyInputManager _instance;
    public static KeyInputManager Instance => _instance;
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void ChangeKeyInput(IKeyInput keyInterface)
    {
        Instance.curKeyInterface = keyInterface;
    }

    public static void RemoveKeyInput()
    {
        Instance.curKeyInterface = null;
    }

    public IKeyInput curKeyInterface;

    private void Update()
    {
        curKeyInterface?.OnUpdateKey();
    }

}

public interface IKeyInput
{
    public void OnUpdateKey();
}