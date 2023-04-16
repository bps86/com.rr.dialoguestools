using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Spine.Unity;

public class RR_DialogueTools_AssetManager : MonoBehaviour
{
    public Action LoadDialogueEvent;

    public static RR_DialogueTools_AssetManager Instance;
    [SerializeField] private List<string> _dialogues = new List<string>();
    [SerializeField] private bool autoLoadDialogue;
    [SerializeField] private bool useSingleton;
    [SerializeField] private bool useGeneralAudio;
    private RR_DialogueTools_AssetManagement assetManagement;
    private bool assetLoaded;

    private void Awake() {
        if (useSingleton) {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this);
                Init();
            } else {
                Destroy(this);
            }
        }
    }

    public void Init() {
        if (assetLoaded) return;

        assetManagement = new RR_DialogueTools_AssetManagement();
        for (int i = 0; i < _dialogues.Count; i++) {
            assetManagement.dialoguesData.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/" + _dialogues[i]));
            assetManagement.visualAssets.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Visual/" + _dialogues[i]));
        }
        StartCoroutine(Load());
    }

    private IEnumerator Load() {
        StartCoroutine(assetManagement.LoadAssets(useGeneralAudio));
        yield return new WaitUntil(() => assetManagement.isLoaded);
        assetLoaded = true;
        if (LoadDialogueEvent != null && autoLoadDialogue) {
            LoadDialogueEvent();
        }
    }

    public RR_DialogueTools_AssetManagement GetAssetManagement() {
        return assetManagement;
    }

    public AudioClip GetActorBeep(string actorName) {
        return assetManagement.GetActorBeep(actorName);
    }

    public Sprite GetActorSprite(string actorName, string actorExpression) {
        return assetManagement.GetActorSprite(actorName, actorExpression);
    }

    public SkeletonDataAsset GetActorSkeletonDataAsset(string actorName) {
        return assetManagement.GetActorSkeletonDataAsset(actorName);
    }

    public string GetDialogueData(string dialogueName) {
        return assetManagement.dialoguesData[dialogueName].text;
    }

    public string GetVisualAsset(string visualAssetName) {
        return assetManagement.visualAssets[visualAssetName].text;
    }

    public bool CheckActorSpriteExist(string actorName, string actorExpression) {
        return assetManagement.dictActorSprite.ContainsKey(actorName + ";;" + actorExpression);
    }

    public bool CheckActorSpineExist(string actorName) {
        return assetManagement.dictActorSpine.ContainsKey(actorName);
    }

    public bool CheckActorBeepExist(string actorName) {
        return assetManagement.dictActorBeep.ContainsKey(actorName);
    }

    public bool IsAssetLoaded() {
        return assetLoaded;
    }

    public bool IsUsingAudio() {
        return useGeneralAudio;
    }
}
