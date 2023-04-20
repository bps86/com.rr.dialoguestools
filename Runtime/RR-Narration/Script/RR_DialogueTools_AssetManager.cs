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
    [SerializeField] private bool autoInitialize;
    [SerializeField] private bool useAsSingleton;
    [SerializeField] private bool useGeneralAudio;
    private RR_DialogueTools_AssetManagement assetManagement;

    private void Awake() {
        if (useAsSingleton) {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this);
                if (autoInitialize) {
                    Init();
                }
            } else {
                Destroy(this);
            }
        }
    }

    public void Init() {
        if (assetManagement == null) {
            assetManagement = new RR_DialogueTools_AssetManagement();
        }
        if (!assetManagement.assetLoaded) {
            for (int i = 0; i < _dialogues.Count; i++) {
                assetManagement.dialoguesData.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/" + _dialogues[i]));
                assetManagement.visualAssets.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Visual/" + _dialogues[i]));
            }
            StartCoroutine(Load());
        }
    }

    private IEnumerator Load() {
        StartCoroutine(assetManagement.LoadAssets(useGeneralAudio));
        yield return new WaitUntil(() => assetManagement.assetLoaded);
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

    public Vector3 GetAGetActorRectDataPos(string actorName) {
        return assetManagement.GetActorRectDataPos(actorName);
    }

    public Vector3 GetAGetActorRectDataScale(string actorName) {
        return assetManagement.GetActorRectDataScale(actorName);
    }

    public RR_DialogueTools_Visual GetVisualAsset(string visualAssetName) {
        RR_DialogueTools_Visual visual = new RR_DialogueTools_Visual();
        JsonUtility.FromJsonOverwrite(assetManagement.visualAssets[visualAssetName].text, visual);
        if (visual.animMode == RR_DialogueTools_TransitionMode.MovePosition) {
            for (int i = 0; i < visual.visualDatas.Count; i++) {
                visual.visualDatas[i] = GetAdjustedVisualData(visual.visualDatas[i], visual.actorCount);
            }
        }
        if (visual.visualDatas.Count < 1) {
            visual.visualDatas.Add(new RR_DialogueTools_VisualData());
        }
        return visual;
    }

    public RR_DialogueTools_VisualData GetAdjustedVisualData(RR_DialogueTools_VisualData visualData, int actorCount) {
        for (int i = 0; i < actorCount; i++) {
            visualData.startPos[i] = RR_DialogueTools_FunctionsVisual.GetAdjustedActorPos(visualData.startPos[i], GetAGetActorRectDataPos(visualData.actorName[i]));
            visualData.endPos[i] = RR_DialogueTools_FunctionsVisual.GetAdjustedActorPos(visualData.endPos[i], GetAGetActorRectDataPos(visualData.actorName[i]));
            visualData.startScale[i] = RR_DialogueTools_FunctionsVisual.GetAdjustedActorScale(visualData.startScale[i], GetAGetActorRectDataScale(visualData.actorName[i]));
            visualData.endScale[i] = RR_DialogueTools_FunctionsVisual.GetAdjustedActorScale(visualData.endScale[i], GetAGetActorRectDataScale(visualData.actorName[i]));
        }
        return visualData;
    }

    public string GetDialogueData(string dialogueName) {
        return assetManagement.dialoguesData[dialogueName].text;
    }

    public bool CheckActorBeepExist(string actorName) {
        return assetManagement.dictActorBeep.ContainsKey(actorName);
    }

    public bool IsAssetLoaded() {
        return assetManagement.assetLoaded;
    }

    public bool IsUsingAudio() {
        return useGeneralAudio;
    }
}
