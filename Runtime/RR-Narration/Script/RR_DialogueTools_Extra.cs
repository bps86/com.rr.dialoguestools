using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class RR_DialogueTools_Extra : MonoBehaviour
{
    // [SerializeField] private TransitionMode animMode;
    [SerializeField] private Color dimColor;
    public bool isDim;
    // public SpriteRenderer dim;
    public List<Image> imagePos;
    public List<SkeletonGraphic> skeletonGraphics;
    // Start is called before the first frame update
    void Awake() {
        for (int i = 0; i < imagePos.Count; i++) {
            imagePos[i].rectTransform.anchorMin = new Vector2(0.5f, 0);
            imagePos[i].rectTransform.anchorMax = new Vector2(0.5f, 0);
        }
        for (int i = 0; i < imagePos.Count; i++) {
            skeletonGraphics[i].rectTransform.anchorMin = new Vector2(0.5f, 0);
            skeletonGraphics[i].rectTransform.anchorMax = new Vector2(0.5f, 0);
        }
    }

    public void ChangeAnimPos(RR_Narration rR_Narration, RR_Narration_AssetManagement rR_Narration_AssetManagement, RR_Narration_Visualization rR_Narration_Visualization) {
        switch (rR_Narration_Visualization.visual.animMode) {
            case TransitionMode.Static:
                ChangeAnimPos_Static(rR_Narration, rR_Narration_AssetManagement, rR_Narration_Visualization);
                break;
            case TransitionMode.MovePosition:
                ChangeAnimPos_Static(rR_Narration, rR_Narration_AssetManagement, rR_Narration_Visualization);
                break;
        }
    }

    private void ChangeAnimPos_Static(RR_Narration rR_Narration, RR_Narration_AssetManagement rR_Narration_AssetManagement, RR_Narration_Visualization rR_Narration_Visualization) {
        if (rR_Narration_Visualization.visualData.actor.Count <= 0) return;

        string name = "";
        string expression = "";
        for (int i = 0; i < rR_Narration_Visualization.visual.actorCount; i++) {
            Debug.Log(rR_Narration_Visualization.visualData.actor[i]);
            name = rR_Narration_Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
            expression = "";
            if (rR_Narration_Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                expression = rR_Narration_Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];
            imagePos[i].color = Color.clear;
            if (rR_Narration_AssetManagement.dictActorSprite.ContainsKey(rR_Narration_Visualization.visualData.actor[i])) {
                imagePos[i].sprite = rR_Narration_AssetManagement.dictActorSprite[rR_Narration_Visualization.visualData.actor[i]];
                imagePos[i].rectTransform.localPosition = rR_Narration_Visualization.visualData.endPos[i];
                imagePos[i].rectTransform.localScale = rR_Narration_Visualization.visualData.endScale[i];
                imagePos[i].color = dimColor;
                if (rR_Narration_Visualization.visualData.actor[i] == rR_Narration.dialogue.name + ";;" + rR_Narration.dialogue.expression)
                    imagePos[i].color = Color.white;
            } else {
                imagePos[i].sprite = null;
                imagePos[i].color = Color.clear;
            }

            if (!rR_Narration_AssetManagement.dictActorSpine.ContainsKey(name)) {
                skeletonGraphics[i].gameObject.SetActive(false);
                continue;
            }
            if (!skeletonGraphics[i].gameObject.activeSelf) {
                skeletonGraphics[i].gameObject.SetActive(true);
            }
            skeletonGraphics[i].skeletonDataAsset = rR_Narration_AssetManagement.dictActorSpine[name].skeletonDataAsset;
            skeletonGraphics[i].startingAnimation = expression;
            skeletonGraphics[i].Initialize(true);
            skeletonGraphics[i].rectTransform.localPosition = rR_Narration_Visualization.visualData.endPos[i];
            skeletonGraphics[i].rectTransform.localScale = rR_Narration_Visualization.visualData.endScale[i];
            skeletonGraphics[i].color = dimColor;
            if (rR_Narration_Visualization.visualData.actor[i] == rR_Narration.dialogue.name + ";;" + rR_Narration.dialogue.expression)
                skeletonGraphics[i].color = Color.white;
        }
    }

}
