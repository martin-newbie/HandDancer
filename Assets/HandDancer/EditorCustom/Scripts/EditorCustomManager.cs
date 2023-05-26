using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EditorCustomManager : MonoBehaviour
{
    public static EditorCustomManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    AudioSource audioSource;

    public AudioClip curMusic;
    [HideInInspector] public EditorNode curNode;

    public InputField timeInput;
    public EditorTimeScroll timeScroll;
    public EditorNodeScroll nodeScroll;
    public InputField curTimeTxt;

    [Header("UI")]
    public NodeInfoPanel nodeInfo;

    [Header("Long Node")]
    public Image longNodePrefab;
    List<Image> longNodeList = new List<Image>();

    [Header("Editor node")]
    public Transform nodeParent;
    public EditorNode nodePrefab;

    List<NodeData> nodeDataList = new List<NodeData>();
    public List<EditorNode> editorNodeList = new List<EditorNode>();

    private void Start()
    {
        // get audio from temp data
        // init node data list from temp data

        nodeDataList = TempData.Instance.editorNodeList;
        audioSource = GetComponent<AudioSource>();
        curMusic = TempData.Instance.curMusicData.audio;
        audioSource.clip = curMusic;

        timeScroll.InitScroll(curMusic.length);
        nodeScroll.InitNodeScroll(curMusic.length);

        foreach (var item in nodeDataList)
        {
            var temp = Instantiate(nodePrefab, nodeParent);
            temp.InitNode(item);
            editorNodeList.Add(temp);
        }
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            timeScroll.TimeScrolling(audioSource.time);
            curTimeTxt.text = audioSource.time.ToString();
        }

        InitLongNode();
    }

    public void ChangeNode(EditorNode newNode)
    {
        curNode?.UnselectNode();
        newNode.SelectNode();

        curNode = newNode;
        InitNodeInfo();
    }
    public void InitNodeInfo()
    {
        nodeInfo.InitNode(curNode);
    }

    public void InitLongNode()
    {
        var left = editorNodeList.FindAll((item) => item.GetResultData().nodePos == NodePos.LEFT);
        var right = editorNodeList.FindAll((item) => item.GetResultData().nodePos == NodePos.RIGHT);
        var both = editorNodeList.FindAll((item) => item.GetResultData().nodePos == NodePos.BOTH);

        int longNodeCount = 0;
        int state = 0;
        EditorNode startNode = null;
        EditorNode endNode = null;

        foreach (var item in left)
        {
            checkLongNode(item);
        }
        state = 0;

        foreach (var item in right)
        {
            checkLongNode(item);
        }
        for (int i = longNodeCount; i < longNodeList.Count; i++)
        {
            longNodeList[i].gameObject.SetActive(false);
        }

        void checkLongNode(EditorNode item)
        {
            switch (state)
            {
                case 0:
                    if (item.GetResultData().nodeType == NodeType.HOLD_START)
                    {
                        state = 1;
                        startNode = item;
                    }
                    else if (item.GetResultData().nodeType == NodeType.HOLD_END)
                    {
                        // error : end without start error
                        state = 0;
                        startNode = null;
                        endNode = null;
                    }
                    break;
                case 1:
                    if (item.GetResultData().nodeType == NodeType.HOLD_END)
                    {
                        state = 0;
                        endNode = item;

                        longNodeCount++;
                        SetLongNode(longNodeCount, startNode, endNode);

                        startNode = null;
                        endNode = null;
                    }
                    if (item.GetResultData().nodeType == NodeType.HOLD_START)
                    {
                        // error : start clashed with start
                        state = 0;
                        startNode = null;
                        endNode = null;
                    }
                    break;
            }
        }
    }

    void SetLongNode(int count, EditorNode start, EditorNode end)
    {
        if (longNodeList.Count < count)
        {
            var temp = Instantiate(longNodePrefab, nodeParent);
            longNodeList.Add(temp);
        }

        var current = longNodeList[count - 1];
        current.rectTransform.anchoredPosition = new Vector2(start.rect.anchoredPosition.x, start.rect.anchoredPosition.y + 100f);
        current.rectTransform.sizeDelta = new Vector2(350f, end.rect.anchoredPosition.y - start.rect.anchoredPosition.y - 100f);

    }

    public void OnPlayButton()
    {
        audioSource.time = timeScroll.curtime;
        audioSource.Play();
    }

    public void OnStopButton()
    {
        audioSource.Stop();
    }

    public void OnAddNodeButton()
    {
        NodeData data = new NodeData(float.Parse(timeInput.text), 0, 0);
        var temp = Instantiate(nodePrefab, nodeParent);
        temp.InitNode(data);
        editorNodeList.Add(temp);

        editorNodeList = editorNodeList.OrderBy((item) => item.curTime).ToList();
    }

    public void OnDestroyNodeButton()
    {
        if (curNode == null) return;

        editorNodeList.Remove(curNode);
        Destroy(curNode.gameObject);
        curNode = null;

        editorNodeList = editorNodeList.OrderBy((item) => item.curTime).ToList();
    }

    public void OnExportButton()
    {
        // check node error exist (return nothing when error exist)
        // 

        var orderedList = editorNodeList.OrderBy((item) => item.curTime).ToList();
        StringBuilder exportSb = new StringBuilder();
        foreach (var item in orderedList)
        {
            exportSb.Append(item.GetResultData().GetNodeDataStr());
        }


        StageData resultStage = new StageData(exportSb.ToString());
    }
}
