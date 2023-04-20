using System.Collections.Generic;
using UnityEngine;

public class RR_DialogueTools_Visualization
{
    public RR_DialogueTools_Visual visual;
    public RR_DialogueTools_VisualData currentVisualData;
    public Dictionary<string, RR_DialogueTools_VisualData> dictVisualDatas;
    public RR_DialogueTools_Visualization() {
        this.visual = new RR_DialogueTools_Visual();
        this.currentVisualData = new RR_DialogueTools_VisualData();
        this.dictVisualDatas = new Dictionary<string, RR_DialogueTools_VisualData>();
    }
    public void LoadVisualAsset(RR_DialogueTools_Visual visualAsset) {
        dictVisualDatas.Clear();
        visual = visualAsset;
        for (int i = 0; i < visual.visualDatas.Count; i++) {
            dictVisualDatas[visual.visualDatas[i].tags + ";" + visual.visualDatas[i].index] = visual.visualDatas[i];
        }
    }
    public void LoadVisualData(string tags, int index) {
        currentVisualData = dictVisualDatas[tags + ";" + index];
    }
}