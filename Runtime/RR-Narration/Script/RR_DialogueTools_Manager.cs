using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

[RequireComponent(typeof(AudioSource))]
public class RR_DialogueTools_Manager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private SkeletonGraphic skeletonGraphics;
    [SerializeField] private Image sprite;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _dialogue;
    [SerializeField] private RR_DialogueTools_Extra rR_DialogueTools_Extra;
    [SerializeField] private IsInverted isInverted = IsInverted.False;
    [SerializeField] private List<string> _dialogues = new List<string>();
    [SerializeField] private string currentDialogue;
    [SerializeField] private string tags;
    [SerializeField] private int index;
    [SerializeField] private int beepPerSeconds;
    [SerializeField] private float minPitch = -0.2f;
    [SerializeField] private float maxPitch = 0.2f;
    [SerializeField] private float Range = 0;
    [SerializeField] private bool useExtra_Visual;
    [SerializeField] private bool useBeep;
    [SerializeField] private bool useLocalization;
    private RR_Narration_AssetManagement rR_Narration_AssetManagement;
    private RR_Narration_Visualization rR_Narration_Visualization;
    private RR_Narration rR_Narration;
    private AudioSource audioSource;
    private Vector3 spriteV3;
    private Vector3 spineV3;
    private Vector3 spriteV3scale;
    private Vector3 spineV3scale;
    bool stop = false;
    void Awake() {
        rR_Narration = new RR_Narration();
        rR_Narration_AssetManagement = new RR_Narration_AssetManagement();
        rR_Narration_Visualization = new RR_Narration_Visualization();
        sprite.color = Color.clear;
        audioSource = gameObject.GetComponent<AudioSource>();
        spriteV3 = sprite.gameObject.transform.position;
        spineV3 = skeletonGraphics.gameObject.transform.position;
        spriteV3scale = sprite.gameObject.transform.localScale;
        spineV3scale = skeletonGraphics.gameObject.transform.localScale;
        for (int i = 0; i < _dialogues.Count; i++) {
            rR_Narration_AssetManagement.dialoguesData.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/" + _dialogues[i]));
            if (useExtra_Visual) {
                rR_Narration_AssetManagement.visualAssets.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Visual/" + _dialogues[i]));
            }
        }
        StartCoroutine(Load());
        button.onClick.AddListener(delegate { NextDialogue(); });
    }

    IEnumerator Load() {
        // Debug.Log(rR_Narration_AssetManagement.isLoaded);
        StartCoroutine(rR_Narration_AssetManagement.LoadActorData());
        yield return new WaitUntil(() => rR_Narration_AssetManagement.isLoaded);
        if (!useLocalization) {
            rR_Narration.LoadDialogueFile(rR_Narration_AssetManagement.dialoguesData[currentDialogue].text);
        } else {
            rR_Narration.LoadDialogueTable(currentDialogue);
        }
        if (useExtra_Visual) {
            rR_Narration_Visualization.LoadVisualAsset(rR_Narration_AssetManagement.visualAssets[currentDialogue].text);
        }
        rR_Narration.LoadDialogueData(tags, index, rR_Narration_AssetManagement);
        Refresh(useBeep);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            NextDialogue();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            if (!useLocalization) {
                rR_Narration.LoadDialogueFile(rR_Narration_AssetManagement.dialoguesData[currentDialogue].text);
            } else {
                rR_Narration.LoadDialogueTable(currentDialogue);
            }
            stop = true;
            rR_Narration.LoadDialogueData(tags, index, rR_Narration_AssetManagement);
            stop = false;
            Refresh(useBeep);
        }
    }
    public void NextDialogue() {
        if (index < rR_Narration.dialogues.Count - 1) {
            index += 1;
        }
        if (index < rR_Narration.dialogues.Count) {
            stop = true;
            rR_Narration.LoadDialogueData(tags, index, rR_Narration_AssetManagement);
            if (useExtra_Visual) {
                rR_Narration_Visualization.LoadVisualData(tags, index);
            }
            stop = false;
        }
        Refresh(useBeep);
    }
    void Refresh(bool isBeep = false) {
        // Debug.Log(rR_Narration.dialogue.nameShown);
        _name.text = rR_Narration.dialogue.nameShown;
        audioSource.clip = null;
        if (rR_Narration_AssetManagement.dictActorBeep.ContainsKey(rR_Narration.dialogue.name)) {
            audioSource.clip = rR_Narration_AssetManagement.dictActorBeep[rR_Narration.dialogue.name];
        }
        if (isBeep) {
            // Debug.Log("Beep Used");
            Beep();
        } else {
            _dialogue.text = rR_Narration.dialogue.dialogue;
        }
        if (!useExtra_Visual) {
            sprite.color = Color.clear;
            if (rR_Narration_AssetManagement.dictActorSprite.ContainsKey(rR_Narration.dialogue.name + ";;" + rR_Narration.dialogue.expression)) {
                sprite.sprite = rR_Narration_AssetManagement.dictActorSprite[rR_Narration.dialogue.name + ";;" + rR_Narration.dialogue.expression];
                sprite.color = Color.white;
                sprite.transform.position = spriteV3 + new Vector3((float)rR_Narration.dialogue.charPos * Range, 1, 1);
                sprite.gameObject.transform.localScale = Vector3.Scale(spriteV3scale, new Vector3((float)isInverted, 1, 1));
            } else {
                sprite.sprite = null;
            }
            // Debug.Log(rR_Narration.dialogue.skeletonDataAsset.name);
            if (rR_Narration.dialogue.skeletonDataAsset == null) {
                skeletonGraphics.gameObject.SetActive(false);
                return;
            }
            if (!skeletonGraphics.gameObject.activeSelf) {
                skeletonGraphics.gameObject.SetActive(true);
            }
            // skeletonAnimation.skeletonDataAsset.Clear();
            skeletonGraphics.skeletonDataAsset = rR_Narration.dialogue.skeletonDataAsset;
            skeletonGraphics.startingAnimation = rR_Narration.dialogue.expression;
            skeletonGraphics.Initialize(true);
            skeletonGraphics.rectTransform.localPosition = spineV3 + new Vector3((float)rR_Narration.dialogue.charPos * Range, 1, 1);
            skeletonGraphics.rectTransform.localScale = Vector3.Scale(spineV3scale, new Vector3((float)isInverted, 1, 1));
            skeletonGraphics.color = Color.white;
        }
        if (useExtra_Visual) {
            rR_DialogueTools_Extra.ChangeAnimPos(rR_Narration, rR_Narration_AssetManagement, rR_Narration_Visualization);
        }
    }
    void Beep() {
        if (audioSource.clip == null) return;
        StartCoroutine(PlayBeep(false, rR_Narration.dialogue, audioSource, 1 / beepPerSeconds));
    }

    public IEnumerator PlayBeep(bool pause, RR_Dialogue dialogue, AudioSource audioSource, float seconds = 0.1f) {
        int index = 0;
        int skipIndex = -1;
        bool scanDone = false;
        // Debug.Log("dialogue length: " + dialogue.dialogue.Length);
        for (int i = 0; i < dialogue.dialogue.Length; i++) {
            if (stop) yield break;
            if (index < skipIndex) {
                index += 1;
                continue;
            }
            if (dialogue.dialogue[index].ToString() == "<") {

                scanDone = false;
                skipIndex = getIndex(index, dialogue.dialogue, ref scanDone);
                yield return new WaitUntil(() => scanDone);
                index += 1;
                continue;
            }
            RR_AudioPlayer.PlayBeep(audioSource, minPitch, maxPitch);
            _dialogue.text = dialogue.dialogue.Substring(0, index + 1);
            yield return new WaitForSeconds(seconds);
            yield return new WaitUntil(() => !pause);
            index += 1;
        }
        yield break;
    }
    int getIndex(int index, string dialogue, ref bool scanDone) {
        for (int i = index; i < dialogue.Length; i++) {
            if (dialogue[i].ToString() == ">") {
                // Debug.Log(dialogue.Substring(index, i - index + 1));
                scanDone = true;
                return i;
            }
        }
        scanDone = true;
        return -1;
    }
}
