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

public enum ENoteType
{
    SINGLE,
    LONG,
}

public enum EHitState
{
    DOWN,
    HOLD,
    HOLD_END,
}


public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    // about animation
    [SerializeField] SkeletonGraphic anim;
    IGameLogic curGameLogic;

    [SerializeField] PlayUIManager gameUIManager;

    // about info
    [Header("Info")]
    [SerializeField] InGameState gameModeState;
    [SerializeField] EGameState ingameState;


    // about input
    [Header("Input State")]
    public bool isRightDown;
    public bool isRightHold;

    public bool isLeftDown;
    public bool isLeftHold;

    KeyCode[] leftKeyCode = new KeyCode[4] { KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A };
    KeyCode[] rightKeyCode = new KeyCode[4] { KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };


    // about note
    [Header("Note")]
    [SerializeField] Note notPrefab;
    [SerializeField] Transform noteParent;
    [SerializeField] int allocCnt;

    Stack<Note> notePool = new Stack<Note>();
    Queue<Note> leftNoteQueue = new Queue<Note>();
    Queue<Note> rightNoteQueue = new Queue<Note>();

    float cool;

    // default function
    void Start()
    {
        for (int i = 0; i < allocCnt; i++)
        {
            var temp = Instantiate(notPrefab, noteParent);

            temp.gameObject.SetActive(false);
            notePool.Push(temp);
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
        StartCoroutine(TestGameLogic());
    }
    void Update()
    {
        InGameLogic();
        InputCheckObject();

        if (cool > 0f) cool -= Time.deltaTime;
    }

    // about animatino function
    string prevKey = "";
    int prevDir;
    EHitState prevType;
    TrackEntry curTrack;
    public void SetAnimState(int dir, EHitState type, bool wait = false)
    {
        if (cool <= 0f)
        { // check without cool time
            string aniKey = GetAniKey(type, dir);
            if (!wait || (wait && prevKey == aniKey && anim.AnimationState.Tracks.ElementAt(0).IsComplete))
            {
                curTrack = anim.AnimationState.SetAnimation(0, aniKey, false);
                curTrack.MixDuration = 0;
            }
        }
        else
        { // check with cool time
            if (prevType == type && prevDir != dir)
            {
                string aniKey = GetAniKey(type, 0);
                curTrack = anim.AnimationState.SetAnimation(0, aniKey, false);
                curTrack.MixDuration = 0;
            }
            else
            {
                string aniKey = GetAniKey(type, dir);
                curTrack = anim.AnimationState.SetAnimation(0, aniKey, false);
                curTrack.MixDuration = 0;
            }
        }

        prevDir = dir;
        prevType = type;
        cool = 0.25f;


    }

    string GetAniKey(EHitState state, int dir)
    {
        switch (state)
        {
            case EHitState.DOWN:
                if (dir == -1) return "Left_1";
                if (dir == 1) return "Right_1";
                if (dir == 0) return "Both";
                break;
            case EHitState.HOLD:
                if (dir == -1) return "Left_H";
                if (dir == 1) return "Right_H";
                if (dir == 0) return "Both_H";
                break;
            case EHitState.HOLD_END:
                if (dir == -1) return "Left_HE";
                if (dir == 1) return "Right_HE";
                if (dir == 0) return "Both_HE";
                break;
        }

        return null;
    }

    // about game logic function
    IEnumerator TestGameLogic()
    {
        while (true)
        {
            ShowNote(ENoteType.LONG, -1, 5f, 1f);
            yield return new WaitForSeconds(3f);
            ShowNote(ENoteType.LONG, 1, 5f, 1f);
            yield return new WaitForSeconds(3f);
            ShowNote(ENoteType.LONG, -1, 5f, 1.5f);
            ShowNote(ENoteType.LONG, 1, 5f, 1.5f);
            yield return new WaitForSeconds(3f);
        }
    }
    void InGameLogic()
    {
        curGameLogic.Play();

        CheckInputCondition();
        CheckNoteCondition();
    }

    // about input function
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

    // about note function
    public void ShowNote(ENoteType state, int dir, float speed, float dur = 0f)
    {
        var note = PopNotePool();
        note.Init(state, dir, speed, dur);
        (dir == 1 ? rightNoteQueue : leftNoteQueue).Enqueue(note);
    }
    public Note PopNotePool()
    {
        var result = notePool.Pop();
        result.gameObject.SetActive(true);
        return result;
    }
    public void PushNote(Note note)
    {
        note.gameObject.SetActive(false);
        notePool.Push(note);
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