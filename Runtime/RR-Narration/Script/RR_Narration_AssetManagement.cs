using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RR_Narration_AssetManagement
{
    public Dictionary<string, TextAsset> dialoguesData = new Dictionary<string, TextAsset>();
    public Dictionary<string, Sprite> dictActorSprite = new Dictionary<string, Sprite>();
    public Dictionary<string, RR_ActorSpine> dictActorSpine = new Dictionary<string, RR_ActorSpine>();
    public Dictionary<string, AudioClip> dictActorBeep = new Dictionary<string, AudioClip>();
    public bool isLoaded = false;

    public IEnumerator LoadActorData() {
        if (isLoaded) yield break;
        string name = "";
        string expression = "";
        TextAsset spriteDataPath = Resources.Load<TextAsset>("spritePaths");
        TextAsset spineDataPath = Resources.Load<TextAsset>("spinePaths");
        TextAsset beepDataPath = Resources.Load<TextAsset>("beepPaths");
        string[] spritePath = spriteDataPath.text.Split(';');
        string[] spinePath = spineDataPath.text.Split(';');
        string[] beepPath = beepDataPath.text.Split(';');
        for (int i = 0; i < spritePath.Length; i++) {
            if (System.String.IsNullOrEmpty(spritePath[i])) {
                continue;
            }
            name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
            expression = spritePath[i].Substring(spritePath[i].LastIndexOf(',') + 1, spritePath[i].Length - spritePath[i].LastIndexOf(',') - 1);
            dictActorSprite.Add(name + ";;" + expression, Resources.Load<Sprite>(spritePath[i]));
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < spinePath.Length; i++) {
            if (System.String.IsNullOrEmpty(spinePath[i])) {
                continue;
            }
            name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            dictActorSpine.Add(name, new RR_ActorSpine(name, "RR-Actors-Spine/" + name + "/skeleton_SkeletonData"));
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < beepPath.Length; i++) {
            if (System.String.IsNullOrEmpty(beepPath[i])) {
                continue;
            }
            name = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1);
            dictActorBeep.Add(name, Resources.Load<AudioClip>(beepPath[i]));
            yield return new WaitForFixedUpdate();
        }
        isLoaded = true;
        yield break;
    }

    public Sprite getActorSprite(RR_Dialogue dialogue) {
        if (dictActorSprite.ContainsKey(dialogue.name + ";;" + dialogue.expression)) {
            return dictActorSprite[dialogue.name + ";;" + dialogue.expression];
        } else {
            return null;
        }
    }

    public RR_ActorSpine getActorSpine(RR_Dialogue dialogue) {
        if (dictActorSprite.ContainsKey(dialogue.name + ";;" + dialogue.expression)) {
            return dictActorSpine[dialogue.name + ";;" + dialogue.expression];
        } else {
            return null;
        }
    }
}