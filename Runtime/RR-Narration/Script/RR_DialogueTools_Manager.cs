using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class RR_DialogueTools_Manager : MonoBehaviour
{
    public Action<string, string, int> MidDialogueEvent;
    public Action<string, string, int> EndDialogueEvent;

    [SerializeField] private SkeletonGraphic skeletonGraphicsForActor;
    [SerializeField] private Image spriteForActor;
    [SerializeField] private Color dimColor;
    [SerializeField] private Color silhouetteColor;
    [SerializeField] private AudioSource beepAudioSource;
    [SerializeField] private AudioSource voiceActAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _dialogue;
    [SerializeField] private RectTransform shakingObject;
    [SerializeField] private RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager;
    [SerializeField] private RR_DialogueTools_ExtraVisual rR_DialogueTools_ExtraVisual;
    [SerializeField] private string defaultAnimation;
    [SerializeField] private string currentDialogue;
    [SerializeField] private string tags;
    [SerializeField] private int index;
    [SerializeField] private int maxIndex;
    [SerializeField] private int textSpeed;
    [SerializeField] private int shakeTotal;
    [SerializeField] private float minPitch = -0.2f;
    [SerializeField] private float maxPitch = 0.2f;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeRange;
    [SerializeField] private bool autoInitialize;
    [SerializeField] private bool refreshAfterLoad;
    [SerializeField] private bool disableOverrideTransform;
    [SerializeField] private bool useDim;
    [SerializeField] private bool useBeepAudio;
    [SerializeField] private bool useGeneralAudio;
    [SerializeField] private bool useLocalization;
    private RR_Narration rR_Narration;
    private Vector3 shakeDefaultPosition;
    private Vector3 shakeProgressPosition;
    private Coroutine textSpeedCoroutine;
    private float shakeProgress;
    private bool pauseTextSpeed;
    private bool stopCoroutine;

    private void Awake() {
        if (autoInitialize) {
            Init();
        }
    }

    public void Init() {
        rR_Narration = new RR_Narration();
        if (rR_DialogueTools_AssetManager == null) {
            rR_DialogueTools_AssetManager = RR_DialogueTools_AssetManager.Instance;
            rR_DialogueTools_AssetManager.LoadDialogueEvent = null;
        }
        if (shakingObject != null) {
            shakeDefaultPosition = shakingObject.anchoredPosition;
        }
        if (rR_DialogueTools_ExtraVisual != null) {
            rR_DialogueTools_ExtraVisual.SetActorSpriteEvent += SetActorSprite;
            rR_DialogueTools_ExtraVisual.SetActorSpineEvent += SetActorSpine;
            rR_DialogueTools_ExtraVisual.Init();
        }
        if (spriteForActor != null) {
            spriteForActor.color = Color.clear;
        }
        if (rR_DialogueTools_AssetManager != null) {
            rR_DialogueTools_AssetManager.LoadDialogueEvent += LoadNarration;
            rR_DialogueTools_AssetManager.Init();
        }
    }

    public void SetDialogueData(string dialogueTitle, string tag, int index, int maxIndex) {
        this.tags = tag;
        this.index = index;
        this.maxIndex = maxIndex;
        currentDialogue = dialogueTitle;
        LoadNarration();
    }

    public string GetCurrentDialogue() {
        return currentDialogue;
    }

    public string GetTag() {
        return tags;
    }

    public int GetIndex() {
        return index;
    }

    public int GetMaxIndex() {
        return maxIndex;
    }

    public void SetTag(string tag) {
        this.tags = tag;
    }

    public void SetIndex(int index) {
        this.index = index;
    }

    public void SetMaxIndex(int maxIndex) {
        this.maxIndex = maxIndex;
    }

    private void Update() {
        ShakeObject();
    }

    public void SetAndLoadCurrentDialogue(string dialogueTitle) {
        currentDialogue = dialogueTitle;
        LoadNarration();
    }

    public void LoadNarration() {
        LoadDialogue(currentDialogue);
        if (refreshAfterLoad) {
            Refresh();
        }
    }

    public void SkipNarration() {
        if (EndDialogueEvent != null) {
            EndDialogueEvent(currentDialogue, tags, maxIndex);
        }
    }

    public void LoadDialogue(string dialogueTitle) {
        SetDialogue(dialogueTitle);
        SetVisualAsset(dialogueTitle);
        LoadData(tags, index);
    }

    public void LoadData(string tag, int index) {
        rR_Narration.LoadDialogueData(tag, index);
        if (rR_DialogueTools_ExtraVisual != null) {
            rR_DialogueTools_ExtraVisual.LoadVisualData(tags, index);
        }
    }

    public void NextDialogue() {
        if (index < maxIndex) {
            index += 1;
            if (index <= maxIndex) {
                pauseTextSpeed = true;
                LoadData(tags, index);
                pauseTextSpeed = false;
                if (refreshAfterLoad) {
                    Refresh();
                }
            }
        } else if (EndDialogueEvent != null) {
            EndDialogueEvent(currentDialogue, tags, index);
        }
    }

    private void SetDialogue(string dialogueTitle) {
        if (!useLocalization) {
            rR_Narration.LoadDialogueFile(rR_DialogueTools_AssetManager.GetDialogueData(dialogueTitle));
        } else {
            rR_Narration.LoadDialogueTable(dialogueTitle);
        }
    }

    private void SetVisualAsset(string dialogueTitle) {
        if (rR_DialogueTools_ExtraVisual == null) return;

        rR_DialogueTools_ExtraVisual.LoadVisualAsset(rR_DialogueTools_AssetManager.GetVisualAsset(dialogueTitle));
    }

    public void Refresh() {
        SetDialogueText();
        ResetShakeObject();
        RunGeneralAudio();
        if (rR_DialogueTools_ExtraVisual == null) {
            SetActor(rR_Narration.dialogue);
        } else {
            rR_DialogueTools_ExtraVisual.ChangeAnimPos(rR_Narration.dialogue);
        }
        if (MidDialogueEvent != null) {
            MidDialogueEvent(currentDialogue, tags, index);
        }
    }

    private void TextSpeed() {
        if (textSpeedCoroutine != null) {
            StopCoroutine(textSpeedCoroutine);
        }
        textSpeedCoroutine = StartCoroutine(RunTextSpeed(false, rR_Narration.dialogue, beepAudioSource, 1 / textSpeed));
    }

    private void RunGeneralAudio() {
        if (!useGeneralAudio) return;

        RR_AudioPlayer.PlaySfx(rR_Narration.dialogue.voiceActID, false, voiceActAudioSource, rR_DialogueTools_AssetManager.GetAssetManagement());
        RR_AudioPlayer.PlaySfx(rR_Narration.dialogue.sfxID, false, sfxAudioSource, rR_DialogueTools_AssetManager.GetAssetManagement());
        RR_AudioPlayer.PlayBgm(rR_Narration.dialogue.bgmID, false, bgmAudioSource, rR_DialogueTools_AssetManager.GetAssetManagement());
    }

    private IEnumerator RunTextSpeed(bool pause, RR_Dialogue dialogue, AudioSource audioSource, float seconds = 0.1f) {
        int index = 0;
        int skipIndex = -1;
        bool scanDone = false;
        for (int i = 0; i < dialogue.dialogue.Length; i++) {
            if (pauseTextSpeed) {
                yield return new WaitUntil(() => !pauseTextSpeed);
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
        beepAudioSource.clip = rR_DialogueTools_AssetManager.GetActorBeep(rR_Narration.dialogue.actorName);
        if (textSpeed > 0) {
            TextSpeed();
        } else {
            _dialogue.text = rR_Narration.dialogue.dialogue;
        }

    }

    private void ResetShakeObject() {
        if (shakingObject == null) return;

        shakingObject.anchoredPosition = shakeDefaultPosition;
        shakeProgressPosition = Vector3.zero;
        if (rR_Narration.dialogue.useShake) {
            shakeProgress = 0;
        }
    }

    private void ShakeObject() {
        if (shakingObject == null) return;
        if (rR_Narration.dialogue == null) return;
        if (!rR_Narration.dialogue.useShake) return;
        if (shakeProgress >= 360 * shakeTotal) return;

        shakeProgress += ((1 / shakeDuration) * (360 * shakeTotal)) * Time.deltaTime;
        if (shakeProgress < 360 * shakeTotal) {
            shakeProgressPosition.x = Mathf.Sin(shakeProgress * Mathf.Deg2Rad);
            shakeProgressPosition.y = Mathf.Sin(shakeProgress * Mathf.Deg2Rad);
            shakingObject.anchoredPosition = shakeDefaultPosition + (shakeProgressPosition * shakeRange);
        } else {
            shakingObject.anchoredPosition = shakeDefaultPosition;
        }
    }

    private void SetActor(RR_Dialogue dialogue) {
        if (!disableOverrideTransform) {
            SetActorSprite(dialogue.actorName, dialogue.expression, spriteForActor, dialogue.actorPosition, dialogue.actorScale, dialogue.useSilhouette);
            SetActorSpine(dialogue.actorName, dialogue.expression, skeletonGraphicsForActor, dialogue.actorPosition, dialogue.actorScale, dialogue.useSilhouette, dialogue.animationLoop);
        } else {
            SetActorSprite(dialogue.actorName, dialogue.expression, spriteForActor, spriteForActor.rectTransform.anchoredPosition, spriteForActor.rectTransform.localScale, dialogue.useSilhouette);
            SetActorSpine(dialogue.actorName, dialogue.expression, skeletonGraphicsForActor, skeletonGraphicsForActor.rectTransform.anchoredPosition, skeletonGraphicsForActor.rectTransform.localScale, dialogue.useSilhouette, dialogue.animationLoop);
        }
    }
    private void SetActorSprite(string actorName, string animationName, Image actorSprite, Vector3 targetPos, Vector3 targetScale, bool usingSilhouette) {
        actorSprite.sprite = rR_DialogueTools_AssetManager.GetActorSprite(actorName, animationName);
        actorSprite.transform.SetAsLastSibling();
        if (usingSilhouette) {
            actorSprite = RR_DialogueTools_FunctionsVisual.SetActorSprite(actorSprite, GetActorPos(targetPos, targetScale, actorName), GetActorScale(targetScale, actorName), silhouetteColor);
        } else {
            actorSprite = RR_DialogueTools_FunctionsVisual.SetActorSprite(actorSprite, GetActorPos(targetPos, targetScale, actorName), GetActorScale(targetScale, actorName), Color.white);
        }
        if (actorSprite.sprite != null) {
            actorSprite.gameObject.SetActive(true);
            if (useDim) {
                if (actorName != rR_Narration.dialogue.actorName) {
                    actorSprite.transform.SetAsFirstSibling();
                    if (!usingSilhouette) {
                        actorSprite.color = dimColor;
                    }
                }
            }
        } else {
            actorSprite.gameObject.SetActive(false);
        }
    }

    private void SetActorSpine(string actorName, string animationName, SkeletonGraphic skeletonGraphic, Vector3 targetPos, Vector3 targetScale, bool usingSilhouette, bool isLoop) {
        skeletonGraphic.skeletonDataAsset = rR_DialogueTools_AssetManager.GetActorSkeletonDataAsset(actorName);
        skeletonGraphic.allowMultipleCanvasRenderers = true;
        skeletonGraphic.transform.SetAsLastSibling();
        if (usingSilhouette) {
            skeletonGraphic = RR_DialogueTools_FunctionsVisual.SetActorSkeletonGraphics(skeletonGraphic, animationName, GetActorPos(targetPos, targetScale, actorName), GetActorScale(targetScale, actorName), silhouetteColor, isLoop);
        } else {
            skeletonGraphic = RR_DialogueTools_FunctionsVisual.SetActorSkeletonGraphics(skeletonGraphic, animationName, GetActorPos(targetPos, targetScale, actorName), GetActorScale(targetScale, actorName), Color.white, isLoop);
        }
        if (skeletonGraphic.skeletonDataAsset != null) {
            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.Initialize(true);
            if (!String.IsNullOrEmpty(defaultAnimation) && !isLoop) {
                RR_DialogueTools_FunctionsVisual.SetResetAnim(skeletonGraphic, defaultAnimation);
            }
            if (useDim) {
                if (actorName != rR_Narration.dialogue.actorName) {
                    skeletonGraphic.transform.SetAsFirstSibling();
                    if (!usingSilhouette) {
                        skeletonGraphic.color = dimColor;
                    }
                }
            }
        } else {
            skeletonGraphic.gameObject.SetActive(false);
        }
    }

    private Vector3 GetActorPos(Vector3 targetPos, Vector3 targetScale, string actorName) {
        return RR_DialogueTools_FunctionsVisual.GetAdjustedActorPos(targetPos, targetScale, rR_DialogueTools_AssetManager.GetAGetActorRectDataPos(actorName));
    }

    private Vector3 GetActorScale(Vector3 targetScale, string actorName) {
        return RR_DialogueTools_FunctionsVisual.GetAdjustedActorScale(targetScale, rR_DialogueTools_AssetManager.GetAGetActorRectDataScale(actorName));
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
