using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class RR_DialogueTools_ExtraVisual : MonoBehaviour
{
    public Action<string, string, Image, Vector3, Vector3, bool> SetActorSpriteEvent;
    public Action<string, string, SkeletonGraphic, Vector3, Vector3, bool, bool> SetActorSpineEvent;

    [SerializeField] private List<Image> images;
    [SerializeField] private List<SkeletonGraphic> skeletonGraphics;
    [SerializeField] private bool disableAutoSetAnchor;
    [SerializeField] private bool useColorTransition;
    private TransitionMode transitionMode;
    private List<Color> startColor;
    private List<Color> endColor;
    private List<Vector3> startPos;
    private List<Vector3> startScale;
    private List<Vector3> endPos;
    private List<Vector3> endScale;
    private float currentTransition;
    private float transitionDuration;
    private int actorCount;
    private bool isTransitioning;

    public void Init() {
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
        startColor = new List<Color>();
        endColor = new List<Color>();
        for (int i = 0; i < 4; i++) {
            if (startColor.Count < 4) {
                startColor.Add(new Color());
            }
            if (endColor.Count < 4) {
                endColor.Add(Color.white);
            }
        }
    }

    private void Update() {
        MoveActorTransition();
    }

    public void ChangeAnimPos(RR_Narration rR_Narration, RR_DialogueTools_Visualization rR_DialogueTools_Visualization) {
        transitionMode = rR_DialogueTools_Visualization.visual.animMode;
        ChangeAnimPos_MovePosition(rR_Narration.dialogue, rR_DialogueTools_Visualization.visual, rR_DialogueTools_Visualization.visualData);
    }

    private void ChangeAnimPos_MovePosition(RR_Dialogue dialogue, RR_DialogueTools_Visual visual, RR_DialogueTools_VisualData visualData) {
        if (visualData.actorName.Count <= 0) return;

        if (transitionMode == TransitionMode.MovePosition) {
            isTransitioning = false;
            currentTransition = 0;
            actorCount = visual.actorCount;
            SetActorVisualPosition(dialogue, visual, visualData);
            startPos = visualData.startPos;
            startScale = visualData.startScale;
            endPos = visualData.endPos;
            endScale = visualData.endScale;
            transitionDuration = visualData.transitionDuration;
            isTransitioning = true;
        } else {
            SetActorVisualPosition(dialogue, visual, visualData);
        }
    }

    private void SetActorVisualPosition(RR_Dialogue dialogue, RR_DialogueTools_Visual visual, RR_DialogueTools_VisualData visualData) {
        string name = "";
        string expression = "";
        for (int i = 0; i < images.Count; i++) {
            if (images[i] != null) {
                if (i >= visual.actorCount) {
                    images[i].gameObject.SetActive(false);
                }
            }
        }
        for (int i = 0; i < skeletonGraphics.Count; i++) {
            if (skeletonGraphics[i] != null) {
                if (i >= visual.actorCount) {
                    skeletonGraphics[i].gameObject.SetActive(false);
                }
            }
        }
        for (int i = 0; i < visual.actorCount; i++) {
            name = visualData.actorName[i];
            expression = visualData.expression[i];
            startColor[i] = endColor[i];
            if (transitionMode == TransitionMode.MovePosition) {
                SetActorSpriteEvent(name, expression, images[i], visualData.startPos[i], visualData.startScale[i], visualData.useSilhouette[i]);
                SetActorSpineEvent(name, expression, skeletonGraphics[i], visualData.startPos[i], visualData.startScale[i], visualData.useSilhouette[i], visualData.isLooping[i]);
            } else {
                SetActorSpriteEvent(name, expression, images[i], visualData.endPos[i], visualData.endScale[i], visualData.useSilhouette[i]);
                SetActorSpineEvent(name, expression, skeletonGraphics[i], visualData.endPos[i], visualData.endScale[i], visualData.useSilhouette[i], visualData.isLooping[i]);
            }
            endColor[i] = skeletonGraphics[i].color;
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
            if (useColorTransition) {
                skeletonGraphics[i].color = Color.Lerp(startColor[i], endColor[i], currentTransition);
            }
        }
    }
}
