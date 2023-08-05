using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditMode : IGameLogic
{

    [SerializeField] List<NodeData> nodeList = new List<NodeData>();
    Stack<NodeData> nodeDataStack = new Stack<NodeData>();
    float leftStartTime;
    float rightStartTime;
    bool isLeftDown;
    bool isRightDown;


    InGameManager I => InGameManager.Instance;

    public void Start()
    {
        nodeDataStack = new Stack<NodeData>(nodeList);
    }

    public void Play()
    {
    }

    public void Outro()
    {
    }
}
