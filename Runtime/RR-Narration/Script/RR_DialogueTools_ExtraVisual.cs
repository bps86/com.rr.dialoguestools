using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;

public class RR_DialogueTools_ExtraVisual : MonoBehaviour
{
    [SerializeField] private Color dimColor;
    [SerializeField] private bool disableAutoSetAnchor;
    [SerializeField] private string defaultAnimation;
    [SpineEvent] private string defaultSpineAnim;
    public List<Image> images;
    public List<SkeletonGraphic> skeletonGraphics;
    public bool isDim;
    private TransitionMode transitionMode;
    private List<Vector3> startPos;
    private List<Vector3> startScale;
    private List<Vector3> endPos;
    private List<Vector3> endScale;
    private float currentTransition;
    private float transitionDuration;
    private int actorCount;
    private bool isTransitioning;

    // Start is called before the first frame update
    private void Awake() {
        if (!disableAutoSetAnchor) {
            for (int i = 0; i < images.Count; i++) {
                images[i].rectTransform.anchorMin = new Vector2(0.5f, 0);
                images[i].rectTransform.anchorMax = new Vector2(0.5f, 0);
            }
            for (int i = 0; i < images.Count; i++) {
                skeletonGraphics[i].rectTransform.anchorMin = new Vector2(0.5f, 0);
                skeletonGraphics[i].rectTransform.anchorMax = new Vector2(0.5f, 0);
            }
        }
    }

    private void Update() {
        MoveActorTransition();
    }

    public void ChangeAnimPos(RR_Narration rR_Narration, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager, RR_DialogueTools_Visualization rR_DialogueTools_Visualization) {
        transitionMode = rR_DialogueTools_Visualization.visual.animMode;
        ChangeAnimPos_MovePosition(rR_Narration.dialogue, rR_DialogueTools_Visualization.visual, rR_DialogueTools_Visualization.visualData, rR_DialogueTools_AssetManager);
    }

    private void ChangeAnimPos_MovePosition(RR_Dialogue dialogue, RR_DialogueTools_Visual visual, RR_DialogueTools_VisualData visualData, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        if (visualData.actorName.Count <= 0) return;

        if (transitionMode == TransitionMode.MovePosition) {
            isTransitioning = false;
            currentTransition = 0;
        }
        SetActorVisualPosition(dialogue, visual, visualData, rR_DialogueTools_AssetManager);
        if (transitionMode == TransitionMode.MovePosition) {
            startPos = visualData.startPos;
            startScale = visualData.startScale;
            endPos = visualData.endPos;
            endScale = visualData.endScale;
            transitionDuration = visualData.transitionDuration;
            isTransitioning = true;
        }
    }

    private void SetActorVisualPosition(RR_Dialogue dialogue, RR_DialogueTools_Visual visual, RR_DialogueTools_VisualData visualData, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        string name = "";
        string expression = "";
        for (int i = 0; i < visual.actorCount; i++) {
            name = visualData.actorName[i];
            expression = visualData.expression[i];
            images[i] = SetActorSprite(images[i], name, expression, i, visualData, rR_DialogueTools_AssetManager);
            skeletonGraphics[i] = SetActorSpine(skeletonGraphics[i], name, expression, i, visualData, rR_DialogueTools_AssetManager);
            if (images[i].sprite != null) {
                if (visualData.actorName[i] != dialogue.actorName) {
                    images[i].color = dimColor;
                }
            } else {
                images[i] = RR_DialogueTools_FunctionsVisual.ResetActorSprite(images[i]);
            }
            if (skeletonGraphics[i].skeletonDataAsset != null) {
                skeletonGraphics[i].gameObject.SetActive(true);
                skeletonGraphics[i].Initialize(true);

                SetResetAnim(skeletonGraphics[i]);

                if (isDim) {
                    if (visualData.actorName[i] != dialogue.actorName) {
                        skeletonGraphics[i].color = dimColor;
                    }
                }
            } else {
                skeletonGraphics[i].gameObject.SetActive(false);
                continue;
            }
        }
    }

    private void MoveActorTransition() {
        if (transitionMode != TransitionMode.MovePosition) return;
        if (!isTransitioning) return;
        if (currentTransition >= 1) return;

        currentTransition += (1 / transitionDuration) * Time.deltaTime;
        for (int i = 0; i < actorCount; i++) {
            images[i].rectTransform.localPosition = Vector3.Lerp(startPos[i], endPos[i], currentTransition);
            images[i].rectTransform.localScale = Vector3.Lerp(startScale[i], endScale[i], currentTransition);
            skeletonGraphics[i].rectTransform.localPosition = Vector3.Lerp(startPos[i], endPos[i], currentTransition);
            skeletonGraphics[i].rectTransform.localScale = Vector3.Lerp(startScale[i], endScale[i], currentTransition);
        }
    }

    private Image SetActorSprite(Image image, string name, string expression, int actorIndex, RR_DialogueTools_VisualData visualData, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        if (transitionMode == TransitionMode.MovePosition) {
            return RR_DialogueTools_FunctionsVisual.SetActorSprite(image, rR_DialogueTools_AssetManager.GetActorSprite(name, expression), visualData.startPos[actorIndex], visualData.startScale[actorIndex]);
        } else {
            return RR_DialogueTools_FunctionsVisual.SetActorSprite(image, rR_DialogueTools_AssetManager.GetActorSprite(name, expression), visualData.endPos[actorIndex], visualData.endScale[actorIndex]);
        }
    }

    private SkeletonGraphic SetActorSpine(SkeletonGraphic skeletonGraphic, string name, string expression, int actorIndex, RR_DialogueTools_VisualData visualData, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        if (rR_DialogueTools_AssetManager.GetActorSpine(name) == null) {
            skeletonGraphic.skeletonDataAsset = null;
        } else if (transitionMode == TransitionMode.MovePosition) {
            skeletonGraphic = RR_DialogueTools_FunctionsVisual.SetActorSkeletonGraphics(skeletonGraphic, rR_DialogueTools_AssetManager.GetActorSpine(name).skeletonDataAsset, expression, visualData.startPos[actorIndex], visualData.startScale[actorIndex]);
        } else {
            skeletonGraphic = RR_DialogueTools_FunctionsVisual.SetActorSkeletonGraphics(skeletonGraphic, rR_DialogueTools_AssetManager.GetActorSpine(name).skeletonDataAsset, expression, visualData.endPos[actorIndex], visualData.endScale[actorIndex]);
        }
        return skeletonGraphic;
    }

    private void SetResetAnim(SkeletonGraphic skeletonGraphic) {
        skeletonGraphic.AnimationState.Complete += delegate {
            ResetAnim(skeletonGraphic);
        };
    }

    private void ResetAnim(SkeletonGraphic skeletonGraphic) {
        skeletonGraphic.AnimationState.SetAnimation(0, defaultAnimation, true);
    }
}
