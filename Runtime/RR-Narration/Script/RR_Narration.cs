using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine;
using Spine.Unity;

public class RR_Narration
{
    public RR_Dialogue dialogue;
    public List<RR_Dialogue> dialogues;
    private Dictionary<string, RR_Dialogue> dictDialogues = new Dictionary<string, RR_Dialogue>();
    private LocalizedStringTable localizedDialogueTable = new LocalizedStringTable(tableReference: "RR-Dialogue");

    public void LoadDialogueTable(string tableKey) {
        LoadDialogueFile(localizedDialogueTable.GetTable()[tableKey].LocalizedValue);

    }
    public void LoadDialogueFile(string _dialoguedata) {
        dictDialogues.Clear();
        dialogues = RR_NarrationFunctions.GetDialogues(_dialoguedata.Split(new string[] { "||" }, System.StringSplitOptions.None));
        if (dialogues.Count < 1) {
            dialogues.Add(new RR_Dialogue());
        }
        for (int i = 0; i < dialogues.Count; i++) {
            dictDialogues[dialogues[i].tags + ";" + dialogues[i].index] = dialogues[i];
        }
        LoadDialogueData(dialogues[0].tags, dialogues[0].index);
    }
    public void LoadDialogueData(string tags, int index, RR_Narration_AssetManagement rR_Narration_AssetManagement = null) {
        dialogue = dictDialogues[tags + ";" + index];
        if (rR_Narration_AssetManagement != null) {
            Debug.Log(rR_Narration_AssetManagement.dictActorSpine.Count);
            dialogue.sprite = rR_Narration_AssetManagement.getActorSprite(dialogue);
            dialogue.skeletonDataAsset = rR_Narration_AssetManagement.getActorSpine(dialogue).skeletonDataAsset;
        }
    }
}