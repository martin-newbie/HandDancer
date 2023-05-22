using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum NodePos
{
    LEFT,
    RIGHT,
    BOTH
}

public enum NodeType
{
    COMMON,

    HOLD_START,
    HOLD_END,

    NANTA_START,
    NANTA_END,

    SPECIAL_HOLD_START,
    SPECIAL_HOLD_END
}

[Serializable]
public class MusicData
{
    public List<StageData> stages = new List<StageData>();
    public string songName;

    public int difficultyCount;
    public int theme;


    public List<string> difficultyName;
    public List<float> difficultyLevel;

    public AudioClip audio;
    public AudioClip preview;

    public IEnumerator LoadDatas(string songTitle, string path)
    {
        path += songTitle;
        path = path.Replace("\r", "");

        audio = Resources.Load<AudioClip>(path + "/audio");
        preview = Resources.Load<AudioClip>(path + "/preview");

        int idx = 0;
        if (!File.Exists(path + "/Data"))
        {
            StreamWriter stream = File.CreateText("Assets/Resources/" + path + "/Data.txt");
            yield return stream;
            stream.WriteLine(songTitle);
            stream.WriteLine(0);
            stream.WriteLine(0);
            stream.Close();
        }
        string strData = Resources.Load<TextAsset>(path + "/Data").text;
        var dataSplit = strData.Split('\n');

        songName = dataSplit[idx++];
        difficultyCount = int.Parse(dataSplit[idx++]);
        theme = int.Parse(dataSplit[idx++]);

        difficultyName = new List<string>(new string[difficultyCount]);
        difficultyLevel = new List<float>(new float[difficultyCount]);

        if (difficultyCount <= 0) yield break;

        var difNameSplit = dataSplit[idx++].Split(',');
        var difLevelSplit = dataSplit[idx++].Split(',');

        for (int i = 0; i < difficultyCount; i++)
        {
            difficultyName[i] = difNameSplit[i];
            difficultyLevel[i] = float.Parse(difLevelSplit[i]);
        }
    }
}

[Serializable]
public class StageData
{
    public List<NodeData> node_script; // Ã¤º¸

    public StageData(string script)
    {
        node_script = new List<NodeData>();

        var strSplit = script.Split('\n');

        int i = 0;

        for (; i < strSplit.Length; i++)
        {
            var item = strSplit[i];

            if (string.IsNullOrEmpty(item)) continue;

            string[] strData = item.Split('\t');
            NodeData nodeData = new NodeData(strData);
            node_script.Add(nodeData);
        }

        node_script = node_script.OrderBy((item) => item.activeTime).ToList();
    }

    public string GetJsonParse()
    {
        string result;
        StringBuilder sb = new StringBuilder();

        foreach (var item in node_script)
        {
            string time = item.activeTime.ToString();
            string nodePos = ((int)item.nodePos).ToString();
            string nodeType = ((int)item.nodeType).ToString();

            sb.Append(string.Format("{0}\t{1}\t{2}\n", time, nodePos, nodeType));
        }

        result = sb.ToString();
        return result;
    }
}

[Serializable]
public struct NodeData
{
    public float activeTime;
    public NodeType nodeType;
    public NodePos nodePos;

    public NodeData(float activeTime, int nodePos, int nodeType)
    {
        this.activeTime = activeTime;
        this.nodePos = (NodePos)nodePos;
        this.nodeType = (NodeType)nodeType;
    }

    public NodeData(string[] args)
    {
        int idx = 0;
        activeTime = float.Parse(args[idx++]);
        nodePos = (NodePos)int.Parse(args[idx++]);
        nodeType = (NodeType)int.Parse(args[idx++]);
    }

    public void ChangeTime(float time, float maxTime)
    {
        time = Mathf.Clamp(time, 0f, maxTime);
        activeTime = time;
    }
}