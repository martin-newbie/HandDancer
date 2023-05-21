using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempData : MonoBehaviour
{
    public static TempData Instance = null;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public List<NodeData> editorNodeList = new List<NodeData>();
    public MusicData curMusicData;
    public InGameState gameState;
    public int mapIdx;

    [Header("Setting")]
    public float startOffset;
}
