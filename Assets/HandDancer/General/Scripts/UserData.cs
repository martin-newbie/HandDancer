using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    private static UserData _instance = null;
    public static UserData Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public List<string> activeSongsList = new List<string>();
    public List<MusicData> musicDataList = new List<MusicData>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitSongList());
    }

    IEnumerator InitSongList()
    {
        activeSongsList = new List<string>();
        musicDataList = new List<MusicData>();

        string str = Resources.Load<TextAsset>("BeatmapList").text;
        foreach (var item in str.Split('\n'))
        {
            activeSongsList.Add(item);
        }

        foreach (var item in activeSongsList)
        {
            var musicData = new MusicData();
            yield return StartCoroutine(musicData.LoadDatas(item, "Beatmaps/"));
            musicDataList.Add(musicData);
        }

        SceneManager.LoadScene("Menu");
    }

    public MusicData GetMusicData(int idx)
    {
        return musicDataList[idx];
    }
}
