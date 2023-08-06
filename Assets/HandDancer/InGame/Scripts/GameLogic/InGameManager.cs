using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EGameState
{
    Intro,
    InGame,
    Outro
}

public enum EHitState
{
    SINGLE,
    LONG,
    LONG_END,
}


public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public SkeletonGraphic anim;

    [SerializeField] PlayUIManager gameUIManager;
    [SerializeField] InGameState gameModeState;
    [SerializeField] EGameState ingameState;


    int curStage;
    [HideInInspector] public float timer = 0f;

    IGameLogic curGameLogic;

    KeyCode[] leftKeyCode = new KeyCode[4] { KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A };
    KeyCode[] rightKeyCode = new KeyCode[4] { KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };

    Queue<Note> leftNoteQueue = new Queue<Note>();
    Queue<Note> rightNoteQueue = new Queue<Note>();

    [Header("Note")]
    [SerializeField] SingleNote singleNotePrefab;
    [SerializeField] LongNote longNotePrefab;
    [SerializeField] Transform noteParent;
    [SerializeField] int allocCnt;
    Stack<Note> singleNotePool = new Stack<Note>();
    Stack<Note> longNotePool = new Stack<Note>();


    void Start()
    {
        for (int i = 0; i < allocCnt; i++)
        {
            var singleN = Instantiate(singleNotePrefab, noteParent);
            var longN = Instantiate(longNotePrefab, noteParent);

            singleN.gameObject.SetActive(false);
            longN.gameObject.SetActive(false);
            singleNotePool.Push(singleN);
            longNotePool.Push(longN);
        }

        switch (gameModeState)
        {
            case InGameState.Play:
                curGameLogic = new PlayMode();
                break;
            case InGameState.Edit:
                curGameLogic = new EditMode();
                break;
        }

        curGameLogic.Start();
    }

    void Update()
    {
        switch (ingameState)
        {
            case EGameState.Intro:
                IntroLogic();
                break;
            case EGameState.InGame:
                InGameLogic();
                InputCheckObject();
                break;
            case EGameState.Outro:
                OutroLogic();
                break;
        }

        timer += Time.deltaTime;
    }

    public void SetAnimState(int dir, EHitState type, bool wait = false)
    {
        if ((wait && track.IsComplete) || !wait)
            aniTrigger = true;

        dirIdx += dir;
        hitType = type;
    }

    bool aniTrigger;
    int dirIdx;
    EHitState hitType; // 0 : single, 1 : hold, 2 : hold exit

    TrackEntry track = null;
    private void FixedUpdate()
    {
        if (aniTrigger)
        {
            dirIdx = Mathf.Clamp(dirIdx, -1, 1);
            string aniKey = getAniKey(hitType, dirIdx);
            track = anim.AnimationState.SetAnimation(0, aniKey, false);
            track.MixDuration = 0f;

            aniTrigger = false;
            dirIdx = 0;
            hitType = 0;
        }

        if (track == null || track.IsComplete)
        {
            track = anim.AnimationState.SetAnimation(0, "idling", true);
            track.MixDuration = 0f;
        }


        string getAniKey(EHitState state, int dir)
        {
            switch (state)
            {
                case EHitState.SINGLE:
                    if (dir == -1) return "Left_1";
                    if (dir == 1) return "Right_1";
                    if (dir == 0) return "Both";
                    break;
                case EHitState.LONG:
                    if (dir == -1) return "Left_H";
                    if (dir == 1) return "Right_H";
                    if (dir == 0) return "Both_H";
                    break;
                case EHitState.LONG_END:
                    if (dir == -1) return "Left_HE";
                    if (dir == 1) return "Right_HE";
                    if (dir == 0) return "Both_HE";
                    break;
            }

            return null;
        }
    }

    void IntroLogic()
    {
        if (timer >= 1f)
        {
            timer = 0f;
            StartCoroutine(TestGameLogic());
            ingameState = EGameState.InGame;
        }
    }

    IEnumerator TestGameLogic()
    {
        while (true)
        {
            ShowNote(EHitState.SINGLE, -1);
            yield return new WaitForSeconds(0.25f);
            ShowNote(EHitState.SINGLE, 1);
            yield return new WaitForSeconds(0.25f);
            ShowNote(EHitState.SINGLE, -1);
            ShowNote(EHitState.SINGLE, 1);
            yield return new WaitForSeconds(0.25f);
        }
    }

    public bool isRightDown;
    public bool isRightHold;

    public bool isLeftDown;
    public bool isLeftHold;
    void InGameLogic()
    {
        curGameLogic.Play();

        CheckInputCondition();
        CheckNoteCondition();
    }
    void CheckInputCondition()
    {
        for (int i = 0; i < 4; i++)
        {
            isRightDown = Input.GetKeyDown(rightKeyCode[i]);
            if (isRightDown) break;
        }

        for (int i = 0; i < 4; i++)
        {
            isLeftDown = Input.GetKeyDown(leftKeyCode[i]);
            if (isLeftDown) break;
        }

        for (int i = 0; i < 4; i++)
        {
            isRightHold = Input.GetKey(rightKeyCode[i]);
            if (isRightHold) break;
        }

        for (int i = 0; i < 4; i++)
        {
            isLeftHold = Input.GetKey(leftKeyCode[i]);
            if (isLeftHold) break;
        }
    }
    void CheckNoteCondition()
    {
        if (rightNoteQueue.Count > 0)
        {
            rightNoteQueue.First().CheckState();
        }

        if (leftNoteQueue.Count > 0)
        {
            leftNoteQueue.First().CheckState();
        }
    }

    void OutroLogic()
    {
        curGameLogic.Outro();
    }
    void InputCheckObject()
    {
        int dir = 0;
        bool active = false;
        if (isRightHold || isRightDown)
        {
            dir += 1;
            active = true;
        }
        if (isLeftHold || isLeftDown)
        {
            dir -= 1;
            active = true;
        }

        gameUIManager.SetInputCheckActive(dir, active);
    }

    public void ShowNote(EHitState state, int dir)
    {
        var note = PopNotePool();
        note.Init(state, dir, 3);
        (dir == 1 ? rightNoteQueue : leftNoteQueue).Enqueue(note);
    }
    public Note PopNotePool()
    {
        var result = singleNotePool.Pop();
        result.gameObject.SetActive(true);
        return result;
    }
    public void PushNote(Note note)
    {
        note.gameObject.SetActive(false);
        singleNotePool.Push(note);
    }
    public void RemoveQueue(int dir)
    {
        (dir == 1 ? rightNoteQueue : leftNoteQueue).Dequeue();
    }
}

public interface IGameLogic
{
    void Start();
    void Play();
    void Outro();
}