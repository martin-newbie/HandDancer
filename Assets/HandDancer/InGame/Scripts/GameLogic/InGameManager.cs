using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum IGameState
{
    Intro,
    InGame,
    Outro
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
    [SerializeField] IGameState ingameState;

    int curStage;
    [HideInInspector] public float timer = 0f;

    IGameLogic curGameLogic;

    bool[] leftInputState = new bool[4];
    bool[] rightInputState = new bool[4];

    KeyCode[] leftKeyCode = new KeyCode[4] { KeyCode.F, KeyCode.D, KeyCode.S, KeyCode.A };
    KeyCode[] rightKeyCode = new KeyCode[4] { KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon };


    void Start()
    {
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
            case IGameState.Intro:
                IntroLogic();
                break;
            case IGameState.InGame:
                InGameLogic();
                InputCheckObject();
                break;
            case IGameState.Outro:
                OutroLogic();
                break;
        }

        timer += Time.deltaTime;
    }

    public void GetAnimState(int dir, int type)
    {
        aniTrigger = true;
        dirIdx += dir;
        hitType = type;
    }

    bool aniTrigger;
    int dirIdx;
    int hitType; // 0 : single, 1 : hold, 2 : hold exit

    TrackEntry track = null;
    private void FixedUpdate()
    {
        if (aniTrigger)
        {
            string aniKey = getDirKey(dirIdx) + "_" + getTypeKey(hitType);
            track = anim.AnimationState.SetAnimation(0, aniKey, false);

            aniTrigger = false;
        }

        if (track == null || track.IsComplete)
        {
            track = anim.AnimationState.SetAnimation(0, "idling", true);
        }


        string getDirKey(int dir)
        {
            if (dir == 0)
            {
                return "Both";
            }
            else if (dir == -1)
            {
                return "Left";
            }
            else if (dir == 1)
            {
                return "Right";
            }

            return null;
        }

        string getTypeKey(int type)
        {
            if (type == 0)
            {
                return "1";
            }
            else if (type == 1)
            {
                return "H";
            }
            else if (type == 2)
            {
                return "HE";
            }

            return null;
        }
    }

    void CheckInput()
    {
        // left
        for (int i = 0; i < leftKeyCode.Length; i++)
        {
            bool cur = Input.GetKey(leftKeyCode[i]);
            bool prev = leftInputState[i];
            GetInputFunc(cur, prev, -1);
            leftInputState[i] = cur;
        }

        // right
        for (int i = 0; i < rightKeyCode.Length; i++)
        {
            bool cur = Input.GetKey(rightKeyCode[i]);
            bool prev = rightInputState[i];
            GetInputFunc(cur, prev, 1);
            rightInputState[i] = cur;
        }
    }

    void GetInputFunc(bool cur, bool prev, int dir)
    {
        if (cur == true && prev == false)
        {
            // input down
        }
        else if (cur == false && prev == true)
        {
            // input up
        }
        else if (cur == true && prev == true)
        {
            // input hold
        }
        else if (cur == false && prev == false)
        {
            // input unable
        }
    }


    void IntroLogic()
    {
        if (timer >= 1f)
        {
            timer = 0f;
            ingameState = IGameState.InGame;
        }
    }


    public bool isKeyDown;

    void InGameLogic()
    {
        CheckInput();
        curGameLogic.Play();
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
}

public interface IGameLogic
{
    void Start();
    void Play();
    void Outro();
}