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
    HOLD,
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

    bool[] leftInputState = new bool[4];
    bool[] rightInputState = new bool[4];

    KeyCode[] leftKeyCode = new KeyCode[4] { KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A };
    KeyCode[] rightKeyCode = new KeyCode[4] { KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };

    Queue<Note> leftNoteQueue = new Queue<Note>();
    Queue<Note> rightNoteQueue = new Queue<Note>();

    [Header("Note")]
    [SerializeField] Note prefab;
    [SerializeField] Transform noteParent;
    [SerializeField] int allocCnt;
    Stack<Note> notePool = new Stack<Note>();


    void Start()
    {
        for (int i = 0; i < allocCnt; i++)
        {
            var temp = Instantiate(prefab, noteParent);
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

    public void SetAnimState(int dir, EHitState type)
    {
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
                case EHitState.HOLD:
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
            yield return new WaitForSeconds(2f);
        }
    }

    public bool isRightDown;
    public bool isRightHold;
    public bool isRightUp;

    public bool isLeftDown;
    public bool isLeftHold;
    public bool isLeftUp;
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
            bool cur = Input.GetKey(rightKeyCode[i]);
            bool prev = rightInputState[i];
            isRightDown = IsDown(cur, prev);
            rightInputState[i] = cur;

            if (isRightDown) break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool cur = Input.GetKey(leftKeyCode[i]);
            bool prev = leftInputState[i];
            isLeftDown = IsDown(cur, prev);
            leftInputState[i] = cur;

            if (isLeftDown) break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool cur = Input.GetKey(rightKeyCode[i]);
            isRightHold = IsHold(cur, rightInputState);
            rightInputState[i] = cur;

            if (isRightHold) break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool cur = Input.GetKey(leftKeyCode[i]);
            isLeftHold = IsHold(cur, leftInputState);
            leftInputState[i] = cur;

            if (isLeftHold) break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool cur = Input.GetKey(rightKeyCode[i]);
            isRightUp = IsUp(cur, rightInputState);
            rightInputState[i] = cur;

            if (isRightUp) break;
        }

        for (int i = 0; i < 4; i++)
        {
            bool cur = Input.GetKey(leftKeyCode[i]);
            isLeftUp = IsUp(cur, leftInputState);
            leftInputState[i] = cur;

            if (isLeftUp) break;
        }
    }
    void CheckNoteCondition()
    {
        if(rightNoteQueue.Count > 0)
        {
            rightNoteQueue.First().CheckState();
        }

        if(leftNoteQueue.Count > 0)
        {
            leftNoteQueue.First().CheckState();
        }
    }

    bool IsDown(bool cur, bool prev)
    {
        return cur && !prev;
    }
    bool IsHold(bool cur, bool[] total)
    {
        return cur && total.Contains(true);
    }
    bool IsUp(bool cur, bool[] total)
    {
        return !cur && !total.Contains(true);
    }


    void OutroLogic()
    {
        curGameLogic.Outro();
    }
    void InputCheckObject()
    {
        int dir = 0;
        bool active = false;
        if (GetRightDown())
        {
            dir += 1;
            active = true;
        }
        if (GetLeftDown())
        {
            dir -= 1;
            active = true;
        }

        gameUIManager.SetInputCheckActive(dir, active);
    }
    bool GetRightDown()
    {
        foreach (var item in rightInputState)
        {
            if (item) return true;
        }
        return false;
    }
    bool GetLeftDown()
    {
        foreach (var item in leftInputState)
        {
            if (item) return true;
        }
        return false;
    }

    public void ShowNote(EHitState state, int dir)
    {
        var note = PopNotePool();
        note.Init(state, dir, 3);
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