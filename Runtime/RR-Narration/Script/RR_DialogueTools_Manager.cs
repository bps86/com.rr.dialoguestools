using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class RR_DialogueTools_Manager : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonGraphics;
    [SerializeField] private Image sprite;
    [SerializeField] private AudioSource beepAudioSource;
    [SerializeField] private AudioSource voiceActAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _dialogue;
    [SerializeField] private RectTransform shakingObject;
    [SerializeField] private RR_DialogueTools_ExtraVisual rR_DialogueTools_ExtraVisual;
    [SerializeField] private List<string> _dialogues = new List<string>();
    [SerializeField] private string currentDialogue;
    [SerializeField] private string defaultAnimation;
    [SerializeField] private string tags;
    [SerializeField] private int index;
    [SerializeField] private int textSpeed;
    [SerializeField] private int shakeTotal;
    [SerializeField] private float minPitch = -0.2f;
    [SerializeField] private float maxPitch = 0.2f;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeRange;
    [SerializeField] private bool useBeepAudio;
    [SerializeField] private bool useGeneralAudio;
    [SerializeField] private bool useLocalization;
    private RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager;
    private RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    private RR_Narration rR_Narration;
    private Vector3 shakeDefaultPosition;
    private Vector3 shakeProgressPosition;
    private float shakeProgress;
    private bool stop = false;
    private void Awake() {
        rR_Narration = new RR_Narration();
        rR_DialogueTools_AssetManager = new RR_DialogueTools_AssetManager();
        rR_DialogueTools_Visualization = new RR_DialogueTools_Visualization();
        sprite.color = Color.clear;
        if (shakingObject != null) {
            shakeProgress = shakeTotal * 360;
            shakeDefaultPosition = shakingObject.localPosition;
        }
        for (int i = 0; i < _dialogues.Count; i++) {
            rR_DialogueTools_AssetManager.dialoguesData.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/" + _dialogues[i]));
            if (rR_DialogueTools_ExtraVisual != null) {
                rR_DialogueTools_AssetManager.visualAssets.Add(_dialogues[i], Resources.Load<TextAsset>("RR-Visual/" + _dialogues[i]));
            }
        }
        StartCoroutine(Load());
    }



    IEnumerator Load() {
        StartCoroutine(rR_DialogueTools_AssetManager.LoadAssets(useGeneralAudio));
        yield return new WaitUntil(() => rR_DialogueTools_AssetManager.isLoaded);
        LoadDialogue(currentDialogue);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            NextDialogue();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SetDialogue(currentDialogue);
            stop = true;
            rR_Narration.LoadDialogueData(tags, index, rR_DialogueTools_AssetManager);
            stop = false;
            Refresh();
        }
        ShakeObject();
    }

    public void NextDialogue() {
        if (index < rR_Narration.dialogues.Count - 1) {
            index += 1;
        }
        if (index < rR_Narration.dialogues.Count) {
            stop = true;
            rR_Narration.LoadDialogueData(tags, index, rR_DialogueTools_AssetManager);
            if (rR_DialogueTools_ExtraVisual != null) {
                rR_DialogueTools_Visualization.LoadVisualData(tags, index);
            }
            stop = false;
        }
        Refresh();
    }

    private void LoadDialogue(string dialogueTitle) {
        SetDialogue(dialogueTitle);
        SetVisualAsset(dialogueTitle);
        rR_Narration.LoadDialogueData(tags, index, rR_DialogueTools_AssetManager);
        Refresh();
    }

    private void SetDialogue(string dialogueTitle) {
        if (!useLocalization) {
            rR_Narration.LoadDialogueFile(rR_DialogueTools_AssetManager.dialoguesData[dialogueTitle].text);
        } else {
            rR_Narration.LoadDialogueTable(dialogueTitle);
        }
    }

    private void SetVisualAsset(string dialogueTitle) {
        if (rR_DialogueTools_ExtraVisual == null) return;

        rR_DialogueTools_Visualization.LoadVisualAsset(rR_DialogueTools_AssetManager.visualAssets[dialogueTitle].text);
    }

    private void Refresh() {
        SetDialogueText();
        ResetShakeObject();
        RunGeneralAudio();
        if (rR_DialogueTools_ExtraVisual == null) {
            if (rR_DialogueTools_AssetManager.dictActorSprite.ContainsKey(rR_Narration.dialogue.actorName + ";;" + rR_Narration.dialogue.expression)) {
                sprite = SetActorSprite(sprite, rR_Narration.dialogue);
            } else {
                sprite = RR_DialogueTools_FunctionsVisual.ResetActorSprite(sprite);
            }
            if (rR_Narration.dialogue.skeletonDataAsset != null) {
                skeletonGraphics.gameObject.SetActive(true);
                skeletonGraphics = SetActorSpine(skeletonGraphics, rR_Narration.dialogue);
                skeletonGraphics.Initialize(true);
                if (!String.IsNullOrEmpty(defaultAnimation) && !rR_Narration.dialogue.animationLoop) {
                    RR_DialogueTools_FunctionsVisual.SetResetAnim(skeletonGraphics, defaultAnimation);
                }
            } else {
                skeletonGraphics.gameObject.SetActive(false);
                return;
            }
        } else {
            rR_DialogueTools_ExtraVisual.ChangeAnimPos(rR_Narration, rR_DialogueTools_AssetManager, rR_DialogueTools_Visualization);
        }
    }

    private void TextSpeed() {
        StartCoroutine(RunTextSpeed(false, rR_Narration.dialogue, beepAudioSource, 1 / textSpeed));
    }

    private void RunGeneralAudio() {
        if (!useGeneralAudio) return;

        RR_AudioPlayer.PlaySfx(rR_Narration.dialogue.voiceActID, false, voiceActAudioSource, rR_DialogueTools_AssetManager);
        RR_AudioPlayer.PlaySfx(rR_Narration.dialogue.sfxID, false, sfxAudioSource, rR_DialogueTools_AssetManager);
        RR_AudioPlayer.PlayBgm(rR_Narration.dialogue.bgmID, false, bgmAudioSource, rR_DialogueTools_AssetManager);
    }

    private IEnumerator RunTextSpeed(bool pause, RR_Dialogue dialogue, AudioSource audioSource, float seconds = 0.1f) {
        int index = 0;
        int skipIndex = -1;
        bool scanDone = false;
        for (int i = 0; i < dialogue.dialogue.Length; i++) {
            if (stop) {
                yield return null;
            }
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
            if (useBeepAudio && audioSource.clip != null) {
                RR_AudioPlayer.PlayBeep(audioSource, minPitch, maxPitch);
            }
            _dialogue.text = dialogue.dialogue.Substring(0, index + 1);
            yield return new WaitForSeconds(seconds);
            yield return new WaitUntil(() => !pause);
            index += 1;
        }
        yield break;
    }

    private void SetDialogueText() {
        _name.text = rR_Narration.dialogue.displayName;
        beepAudioSource.clip = null;
        if (rR_DialogueTools_AssetManager.dictActorBeep.ContainsKey(rR_Narration.dialogue.actorName)) {
            beepAudioSource.clip = rR_DialogueTools_AssetManager.GetActorBeep(rR_Narration.dialogue.actorName);
        }
        if (textSpeed > 0) {
            TextSpeed();
        } else {
            _dialogue.text = rR_Narration.dialogue.dialogue;
        }

    }

    private void ResetShakeObject() {
        if (shakingObject == null) return;
        if (!rR_Narration.dialogue.useShake) return;

        shakingObject.localPosition = shakeDefaultPosition;
        shakeProgressPosition = Vector3.zero;
        shakeProgress = 0;
    }

    private void ShakeObject() {
        if (shakeProgress < 360 * shakeTotal) {
            shakeProgress += ((1 / shakeDuration) * (360 * shakeTotal)) * Time.deltaTime;
            shakeProgressPosition.x = Mathf.Sin(shakeProgress * Mathf.Deg2Rad);
            shakeProgressPosition.y = Mathf.Sin(shakeProgress * Mathf.Deg2Rad);
            shakingObject.localPosition = shakeDefaultPosition + (shakeProgressPosition * shakeRange);
        }
    }

    Image SetActorSprite(Image actorSprite, RR_Dialogue dialogue) {
        return RR_DialogueTools_FunctionsVisual.SetActorSprite(
            actorImage: actorSprite,
            targetSprite: dialogue.sprite,
            targetPosition: dialogue.actorPosition,
            targetScale: dialogue.actorScale
            );
    }

    SkeletonGraphic SetActorSpine(SkeletonGraphic targetSkeletonGraphic, RR_Dialogue dialogue) {
        return RR_DialogueTools_FunctionsVisual.SetActorSkeletonGraphics(
            targetSkeletonGraphic,
            dialogue.skeletonDataAsset,
            dialogue.expression,
            dialogue.actorPosition,
            dialogue.actorScale,
            dialogue.animationLoop
            );
    }

    int getIndex(int index, string dialogue, ref bool scanDone) {
        for (int i = index; i < dialogue.Length; i++) {
            if (dialogue[i].ToString() == ">") {
                scanDone = true;
                return i;
            }
        }
        scanDone = true;
        return -1;
    }
}
