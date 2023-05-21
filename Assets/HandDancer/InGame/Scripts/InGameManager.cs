using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    enum IGameState
    {
        Intro,
        InGame,
        Outro
    }

    public Node nodePrefab;
    public Stack<Node> nodeObjectPoolStack = new Stack<Node>();
    public int allocCount = 50;

    [SerializeField] InGameState gameModeState;
    [SerializeField] IGameState ingameState;

    AudioClip audio;
    AudioSource audioSource;
    MusicData curMusic;
    int curStage;

    [SerializeField] List<NodeData> nodeList = new List<NodeData>();
    Stack<NodeData> nodeDataStack = new Stack<NodeData>();
    [HideInInspector] public float timer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        curMusic = TempData.Instance.curMusicData;
        gameModeState = TempData.Instance.gameState;
        curStage = TempData.Instance.mapIdx;

        audio = curMusic.audio;
        nodeDataStack = new Stack<NodeData>(nodeList);
        nodeList = curMusic.stages[curStage].node_script;

        for (int i = 0; i < allocCount; i++)
        {
            var temp = Instantiate(nodePrefab, transform);
            temp.gameObject.SetActive(false);
            nodeObjectPoolStack.Push(temp);
        }

        audioSource.clip = audio;
    }

    void Update()
    {
        switch (ingameState)
        {
            case IGameState.Intro:
                IntroLogic();
                break;
            case IGameState.InGame:
                InGameLogic();
                break;
            case IGameState.Outro:
                OutroLogic();
                break;
        }

        timer += Time.deltaTime;
    }

    void IntroLogic()
    {
        if (timer >= 3f)
        {
            audioSource.Play();
            timer = 0f;
            ingameState = IGameState.InGame;
        }
    }

    // ingame state
    bool isLeftDown;
    bool isRightDown;

    float leftStartTime;
    float rightStartTime;

    bool isHighlight;
    bool isKeyDown;

    void InGameLogic()
    {
        // editor mode
        switch (gameModeState)
        {
            case InGameState.Play:
                break;
            case InGameState.Edit:
                InGameEditorMode();
                break;
            default:
                break;
        }

        if (timer >= audio.length + 1f)
        {
            ingameState = IGameState.Outro;
        }
    }

    void InGameEditorMode()
    {
        NormalInput();
    }
    void NormalInput()
    {
        switch (isLeftDown)
        {
            case true:

                if (GetLeftKeyUp())
                {
                    isLeftDown = false;
                    if (timer - leftStartTime > 0.5f)
                    {
                        // add long note start with leftStartTime at left
                        // add long note end with timer at left
                        nodeList.Add(new NodeData(leftStartTime, (int)NodePos.LEFT, (int)NodeType.HOLD_START));
                        nodeList.Add(new NodeData(timer, (int)NodePos.LEFT, (int)NodeType.HOLD_END));
                    }
                    else
                    {
                        // add short note at left
                        nodeList.Add(new NodeData(leftStartTime, (int)NodePos.LEFT, (int)NodeType.COMMON));
                    }
                }
                break;
            case false:

                if (GetLeftKeyDown())
                {
                    leftStartTime = timer;
                    isLeftDown = true;
                }

                break;
        }
        switch (isRightDown)
        {
            case true:

                if (GetRightKeyUp())
                {
                    isRightDown = false;
                    if (timer - rightStartTime > 0.5f)
                    {
                        // add long note start with rightStartTime at right
                        // add long note end with timer at left
                        nodeList.Add(new NodeData(rightStartTime, (int)NodePos.RIGHT, (int)NodeType.HOLD_START));
                        nodeList.Add(new NodeData(timer, (int)NodePos.RIGHT, (int)NodeType.HOLD_END));
                    }
                    else
                    {
                        // add short note at right
                        nodeList.Add(new NodeData(rightStartTime, (int)NodePos.RIGHT, (int)NodeType.COMMON));
                    }

                }

                break;
            case false:

                if (GetRightKeyDown())
                {
                    rightStartTime = timer;
                    isRightDown = true;
                }
                break;
        }
    }

    bool GetRightKeyDown()
    {
        return Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K);
    }
    bool GetRightKeyUp()
    {
        return !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.K);
    }

    bool GetLeftKeyDown()
    {
        return Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.D);
    }
    bool GetLeftKeyUp()
    {
        return !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.D);
    }

    void OutroLogic()
    {
        TempData.Instance.editorNodeList = nodeList;

        string sceneKey = gameModeState == InGameState.Play ? "End" : "EditorCustom";
        SceneManager.LoadScene(sceneKey);
    }
}
