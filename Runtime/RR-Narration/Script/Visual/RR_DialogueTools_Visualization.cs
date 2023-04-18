using System.Collections.Generic;
using UnityEngine;

public class RR_DialogueTools_Visualization
{
    public RR_DialogueTools_Visual visual;
    public RR_DialogueTools_VisualData visualData;
    public Dictionary<string, RR_DialogueTools_VisualData> dictVisualDatas;
    public RR_DialogueTools_Visualization() {
        this.visual = new RR_DialogueTools_Visual();
        this.visualData = new RR_DialogueTools_VisualData();
        this.dictVisualDatas = new Dictionary<string, RR_DialogueTools_VisualData>();
    }
    public void LoadVisualAsset(string visualAsset) {
        dictVisualDatas.Clear();
        JsonUtility.FromJsonOverwrite(visualAsset, visual);
        if (visual.visualDatas.Count < 1) {
            visual.visualDatas.Add(new RR_DialogueTools_VisualData());
        }
        for (int i = 0; i < visual.visualDatas.Count; i++) {
            dictVisualDatas[visual.visualDatas[i].tags + ";" + visual.visualDatas[i].index] = visual.visualDatas[i];
        }
    }
    public void LoadVisualData(string tags, int index) {
        visualData = dictVisualDatas[tags + ";" + index];
    }
}