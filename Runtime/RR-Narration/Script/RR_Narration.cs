using System.Collections.Generic;
using UnityEngine.Localization;

public class RR_Narration
{
    public RR_Dialogue dialogue;
    public List<RR_Dialogue> dialogues;
    private Dictionary<string, RR_Dialogue> dictDialogues;
    private LocalizedStringTable localizedDialogueTable;

    public RR_Narration() {
        this.dictDialogues = new Dictionary<string, RR_Dialogue>();
        this.localizedDialogueTable = new LocalizedStringTable(tableReference: "RR-Dialogue");
    }

    public void LoadDialogueTable(string tableKey) {
        LoadDialogueFile(localizedDialogueTable.GetTable()[tableKey].LocalizedValue);

    }
    public void LoadDialogueFile(string dialoguedata) {
        dictDialogues.Clear();
        dialogues = RR_DialogueTools_Functions.GetDialogues(dialoguedata.Split(new string[] { "||" }, System.StringSplitOptions.None));
        if (dialogues.Count < 1) {
            dialogues.Add(new RR_Dialogue());
        }
        for (int i = 0; i < dialogues.Count; i++) {
            dictDialogues[dialogues[i].tags + ";" + dialogues[i].index] = dialogues[i];
        }
        LoadDialogueData(dialogues[0].tags, dialogues[0].index);
    }
    public void LoadDialogueData(string tags, int index) {
        dialogue = dictDialogues[tags + ";" + index];
    }
}