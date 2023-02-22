using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine;
using Spine.Unity;

public static class RR_Narration
{
    public static RRDialogue dialogue;
    public static List<RRDialogue> dialogues;
    public static Dictionary<string, TextAsset> dialoguesData = new Dictionary<string, TextAsset>();
    public static Dictionary<string, RRDialogue> dictDialogues = new Dictionary<string, RRDialogue>();
    public static Dictionary<string, Sprite> dictActorSprite = new Dictionary<string, Sprite>();
    public static Dictionary<string, RR_ActorSpine> dictActorSpine = new Dictionary<string, RR_ActorSpine>();
    public static Dictionary<string, AudioClip> dictActorBeep = new Dictionary<string, AudioClip>();
    public static LocalizedStringTable localizedDialogueTable = new LocalizedStringTable(tableReference: "RR-Dialogue");
    public static bool isLoaded = false;
    public static IEnumerator LoadActorData()
    {
        if (isLoaded) yield break;
        TextAsset spriteDataPath = Resources.Load<TextAsset>("spritePaths");
        TextAsset spineDataPath = Resources.Load<TextAsset>("spinePaths");
        TextAsset beepDataPath = Resources.Load<TextAsset>("beepPaths");
        string[] spritePath = spriteDataPath.text.Split(';');
        string[] spinePath = spineDataPath.text.Split(';');
        string[] beepPath = beepDataPath.text.Split(';');
        for (int i = 0; i < spritePath.Length; i++)
        {
            if (System.String.IsNullOrEmpty(spritePath[i])) continue;
            string name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
            string expression = spritePath[i].Substring(spritePath[i].LastIndexOf(',') + 1, spritePath[i].Length - spritePath[i].LastIndexOf(',') - 1);
            dictActorSprite.Add(name + ";;" + expression, Resources.Load<Sprite>(spritePath[i]));
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < spinePath.Length; i++)
        {
            if (System.String.IsNullOrEmpty(spinePath[i])) continue;
            string name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            dictActorSpine.Add(name, new RR_ActorSpine(name, "RR-Actors-Spine/" + name + "/skeleton_SkeletonData"));
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < beepPath.Length; i++)
        {
            if (System.String.IsNullOrEmpty(beepPath[i])) continue;
            string name = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1);
            dictActorBeep.Add(name, Resources.Load<AudioClip>(beepPath[i]));
            yield return new WaitForFixedUpdate();
        }
        isLoaded = true;
        yield break;
    }

    public static void LoadDialogueTable(string tableKey)
    {
        LoadDialogueFile(RR_Narration.localizedDialogueTable.GetTable()[tableKey].LocalizedValue);

    }
    public static void LoadDialogueFile(string _dialoguedata)
    {
        dictDialogues.Clear();
        dialogues = RR_NarrationManager.GetDialogues(_dialoguedata.Split(new string[] { "||" }, System.StringSplitOptions.None));
        if (dialogues.Count < 1) dialogues.Add(new RRDialogue());
        for (int i = 0; i < dialogues.Count; i++)
            dictDialogues[dialogues[i].tags + ";" + dialogues[i].index] = dialogues[i];
        LoadDialogueData(dialogues[0].tags, dialogues[0].index);
    }
    public static void LoadDialogueData(string tags, int index)
    {
        dialogue = dictDialogues[tags + ";" + index];
        if (dictActorSprite.ContainsKey(dialogue.name + ";;" + dialogue.expression)) dialogue.sprite = dictActorSprite[dialogue.name + ";;" + dialogue.expression];
        else dialogue.sprite = null;
        if (dictActorSpine.ContainsKey(dialogue.name))
        {
            dialogue.skeletonDataAsset = dictActorSpine[dialogue.name].skeletonDataAsset;
        }
        else dialogue.skeletonDataAsset = null;
    }
}