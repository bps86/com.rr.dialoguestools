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
    public Dictionary<string, SkeletonDataAsset> dictActorSkeletonDataAsset;
    public Dictionary<string, RR_DialogueTools_RectData> dictActorRectData;
    public Dictionary<string, AudioClip> dictActorBeep;
    public Dictionary<string, AudioClip> dictSfx;
    public Dictionary<string, AudioClip> dictBgm;
    public bool assetLoaded;

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
        if (dictActorSkeletonDataAsset.ContainsKey(actorName)) {
            return dictActorSkeletonDataAsset[actorName];
        } else {
            return null;
        }
    }

    public Vector3 GetActorRectDataPos(string actorName) {
        if (dictActorSkeletonDataAsset.ContainsKey(actorName)) {
            return dictActorRectData[actorName].ActorPivot;
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 GetActorRectDataScale(string actorName) {
        if (dictActorSkeletonDataAsset.ContainsKey(actorName)) {
            return dictActorRectData[actorName].ActorScale;
        } else {
            return Vector3.one;
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
        assetLoaded = false;
        LoadActorData();
        if (useGeneralAudio) {
            LoadAudioGeneral();
            yield return new WaitUntil(() => IsGeneralAudioAssetLoaded());
        }
        yield return new WaitUntil(() => IsActorAssetLoaded());
        assetLoaded = true;
    }

    private bool IsActorAssetLoaded() {
        return dictActorSprite != null && dictActorSkeletonDataAsset != null && dictActorBeep != null && dictActorRectData != null;
    }

    private bool IsGeneralAudioAssetLoaded() {
        return dictSfx != null && dictBgm != null;
    }

    public void LoadActorData() {
        dictActorSprite = LoadActorSpriteData("spritePaths");
        dictActorSkeletonDataAsset = LoadActorSkeletonDataAsset("spinePaths");
        dictActorBeep = LoadAudioData("beepPaths");
        LoadActorRectData("rectDataPaths");
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
        Dictionary<string, SkeletonDataAsset> dictSkeletonAsset = new Dictionary<string, SkeletonDataAsset>();
        string name = "";
        string[] spinePath = GetDataPaths(pathsName, ';');
        for (int i = 0; i < spinePath.Length; i++) {
            if (System.String.IsNullOrEmpty(spinePath[i])) {
                continue;
            }
            name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            dictSkeletonAsset.Add(name, Resources.Load<SkeletonDataAsset>("RR-Actors-Spine/" + name + "/skeleton_SkeletonData"));
        }

        return dictSkeletonAsset;
    }

    private void LoadActorRectData(string pathsName) {
        List<TextAsset> textAssets = new List<TextAsset>();
        string[] rectDataPath = GetDataPaths(pathsName, ';');
        for (int i = 0; i < rectDataPath.Length; i++) {
            if (System.String.IsNullOrEmpty(rectDataPath[i])) {
                continue;
            }
            textAssets.Add(Resources.Load<TextAsset>(rectDataPath[i]));
        }
        dictActorRectData = LoadDictRectData(textAssets);
    }

    private Dictionary<string, RR_DialogueTools_RectData> LoadDictRectData(List<TextAsset> rectDatas) {
        Dictionary<string, RR_DialogueTools_RectData> dictRectData = new Dictionary<string, RR_DialogueTools_RectData>();
        string[] rectData = null;
        for (int i = 0; i < rectDatas.Count; i++) {
            rectData = rectDatas[i].text.Split(';');
            dictRectData.Add(rectDatas[i].name, new RR_DialogueTools_RectData(rectData));
        }
        return dictRectData;
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