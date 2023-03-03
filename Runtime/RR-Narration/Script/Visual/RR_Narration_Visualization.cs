using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RR_Narration_Visualization
{
    public RR_NarrationVisual visual = new RR_NarrationVisual();
    public RR_NarrationVisualData visualData = new RR_NarrationVisualData();
    public Dictionary<string, RR_NarrationVisualData> dictVisualDatas = new Dictionary<string, RR_NarrationVisualData>();
    public void LoadVisualAsset(string visualAsset) {
        dictVisualDatas.Clear();
        JsonUtility.FromJsonOverwrite(visualAsset, visual);
        if (visual.visualDatas.Count < 1) visual.visualDatas.Add(new RR_NarrationVisualData());
        for (int i = 0; i < visual.visualDatas.Count; i++)
            dictVisualDatas[visual.visualDatas[i].tags + ";" + visual.visualDatas[i].index] = visual.visualDatas[i];
        LoadVisualData(visual.visualDatas[0].tags, visual.visualDatas[0].index);
    }
    public void LoadVisualData(string tags, int index) {
        visualData = dictVisualDatas[tags + ";" + index];
    }
}