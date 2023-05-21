using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public SoundVisual soundVisual;

    [Header("Key Input Interface")]
    public ButtonLayoutGroup musicLayout;
    public ModeSelectPanel modeSelect;
    public MapSelectPanel mapSelect;

    int curMusicIdx;

    private void Start()
    {
        ChangeKeyInput(musicLayout);
        musicLayout.SetAbleObject();
    }

    public void ChangeSelectedMusic(int idx)
    {
        curMusicIdx = idx;
        soundVisual.ChangeMusic(UserData.Instance.musicDataList[curMusicIdx].preview);
    }

    void ChangeKeyInput(IKeyInput keyInput)
    {
        KeyInputManager.ChangeKeyInput(keyInput);
    }

    public void OpenMapSelect()
    {
        mapSelect.OpenWindow(musicLayout.GetCurrentMusicData(), modeSelect.gameState);
    }

    public void StartGame(int mapIdx)
    {
        var musicData = musicLayout.GetCurrentMusicData();
        var gameState = modeSelect.gameState;

        TempData.Instance.curMusicData = musicData;
        TempData.Instance.gameState = gameState;
        TempData.Instance.mapIdx = mapIdx;

        SceneManager.LoadScene("InGame");
    }
}
