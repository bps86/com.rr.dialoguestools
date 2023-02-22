using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class RR_NarrationVisualization
{
    public static RR_NarrationVisual visual = new RR_NarrationVisual();
    public static RR_NarrationVisualData visualData = new RR_NarrationVisualData();
    public static Dictionary<string, TextAsset> visualAssets = new Dictionary<string, TextAsset>();
    public static Dictionary<string, RR_NarrationVisualData> dictVisualDatas = new Dictionary<string, RR_NarrationVisualData>();
    public static void LoadVisualAsset(string visualAsset)
    {
        dictVisualDatas.Clear();
        JsonUtility.FromJsonOverwrite(visualAsset, visual);
        if (visual.visualDatas.Count < 1) visual.visualDatas.Add(new RR_NarrationVisualData());
        for (int i = 0; i < visual.visualDatas.Count; i++)
            dictVisualDatas[visual.visualDatas[i].tags + ";" + visual.visualDatas[i].index] = visual.visualDatas[i];
        LoadVisualData(visual.visualDatas[0].tags, visual.visualDatas[0].index);
    }
    public static void LoadVisualData(string tags, int index)
    {
        visualData = dictVisualDatas[tags + ";" + index];
    }
}