using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine.Unity;

public class RR_DialogueTools_AssetManagement
{
    public Dictionary<string, TextAsset> dialoguesData;
    public Dictionary<string, TextAsset> visualAssets;
    public Dictionary<string, Sprite> dictActorSprite;
    public Dictionary<string, SkeletonDataAsset> dictActorSpine;
    public Dictionary<string, AudioClip> dictActorBeep;
    public Dictionary<string, AudioClip> dictSfx;
    public Dictionary<string, AudioClip> dictBgm;
    public bool isLoaded = false;

    public RR_DialogueTools_AssetManagement() {
        this.dialoguesData = new Dictionary<string, TextAsset>();
        this.visualAssets = new Dictionary<string, TextAsset>();
    }

    public Sprite GetActorSprite(string name, string expression) {
        if (dictActorSprite.ContainsKey(name + ";;" + expression)) {
            return dictActorSprite[name + ";;" + expression];
        } else {
            return null;
        }
    }

    public SkeletonDataAsset GetActorSkeletonDataAsset(string actorName) {
        if (dictActorSpine.ContainsKey(actorName)) {
            return dictActorSpine[actorName];
        } else {
            return null;
        }
    }

    public AudioClip GetActorBeep(string actorName) {
        if (dictActorBeep.ContainsKey(actorName)) {
            return dictActorBeep[actorName];
        } else {
            return null;
        }
    }

    public AudioClip GetSfx(string sfxName) {
        if (dictSfx.ContainsKey(sfxName)) {
            return dictSfx[sfxName];
        } else {
            return null;
        }
    }

    public AudioClip GetBgm(string bgmName) {
        if (dictBgm.ContainsKey(bgmName)) {
            return dictBgm[bgmName];
        } else {
            return null;
        }
    }

    public IEnumerator LoadAssets(bool useGeneralAudio) {
        LoadActorData();
        if (useGeneralAudio) {
            LoadAudioGeneral();
            yield return new WaitUntil(() => IsGeneralAudioAssetLoaded());
        }
        yield return new WaitUntil(() => IsActorAssetLoaded());
        isLoaded = true;
    }

    private bool IsActorAssetLoaded() {
        return dictActorSprite != null && dictActorSpine != null && dictActorBeep != null;
    }

    private bool IsGeneralAudioAssetLoaded() {
        return dictSfx != null && dictBgm != null;
    }

    public void LoadActorData() {
        dictActorSprite = LoadActorSpriteData("spritePaths");
        dictActorSpine = LoadActorSkeletonDataAsset("spinePaths");
        dictActorBeep = LoadAudioData("beepPaths");
    }

    public void LoadAudioGeneral() {
        dictSfx = LoadAudioData("sfxPaths");
        dictBgm = LoadAudioData("bgmPaths");
    }

    private Dictionary<string, Sprite> LoadActorSpriteData(string pathsName) {
        Dictionary<string, Sprite> dictSprite = new Dictionary<string, Sprite>();
        string name = "";
        string expression = "";
        string[] spritePath = GetDataPaths(pathsName, ';');
        for (int i = 0; i < spritePath.Length; i++) {
            if (System.String.IsNullOrEmpty(spritePath[i])) {
                continue;
            }
            name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
            expression = spritePath[i].Substring(spritePath[i].LastIndexOf(',') + 1, spritePath[i].Length - spritePath[i].LastIndexOf(',') - 1);
            dictSprite.Add(name + ";;" + expression, Resources.Load<Sprite>(spritePath[i]));
        }

        return dictSprite;
    }

    private Dictionary<string, SkeletonDataAsset> LoadActorSkeletonDataAsset(string pathsName) {
        Dictionary<string, SkeletonDataAsset> dictRRSpine = new Dictionary<string, SkeletonDataAsset>();
        string name = "";
        string[] spinePath = GetDataPaths(pathsName, ';');
        for (int i = 0; i < spinePath.Length; i++) {
            if (System.String.IsNullOrEmpty(spinePath[i])) {
                continue;
            }
            name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            dictRRSpine.Add(name, Resources.Load<SkeletonDataAsset>("RR-Actors-Spine/" + name + "/skeleton_SkeletonData"));
        }

        return dictRRSpine;
    }

    private Dictionary<string, AudioClip> LoadAudioData(string pathsName) {
        Dictionary<string, AudioClip> dictAudio = new Dictionary<string, AudioClip>();
        string name = "";
        string[] audioDataPath = GetDataPaths(pathsName, ';');
        for (int i = 0; i < audioDataPath.Length; i++) {
            if (System.String.IsNullOrEmpty(audioDataPath[i])) {
                continue;
            }
            name = audioDataPath[i].Substring(audioDataPath[i].LastIndexOf('/') + 1);
            dictAudio.Add(name, Resources.Load<AudioClip>(audioDataPath[i]));
        }
        return dictAudio;
    }

    private string[] GetDataPaths(string pathsName, Char separator) {
        TextAsset textDataPath = Resources.Load<TextAsset>(pathsName);
        return textDataPath.text.Split(separator);
    }
}