using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RR_DialogueTools_Extra : MonoBehaviour
{
    public bool isDim;
    public SpriteRenderer dim;
    public List<SpriteRenderer> spritePos;
    public List<SkeletonAnimation> skeletonAnimations;
    List<MeshRenderer> skeletonMesh = new List<MeshRenderer>();
    Color color = new Color(255, 255, 255, 0);
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < skeletonAnimations.Count; i++)
        {
            if (skeletonMesh.Count < skeletonAnimations.Count) skeletonMesh.Add(null);
            skeletonMesh[i] = skeletonAnimations[i].GetComponent<MeshRenderer>();
        }
        if (!isDim) dim.gameObject.SetActive(false);
        else dim.gameObject.SetActive(true);
        dim.sortingLayerName = "RR-Layers2";
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeAnimPos()
    {
        string name = "";
        string expression = "";
        for (int i = 0; i < RR_NarrationVisualization.visual.actorCount; i++)
        {
            name = RR_NarrationVisualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
            expression = "";
            if (RR_NarrationVisualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                expression = RR_NarrationVisualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];

            spritePos[i].color = color;
            if (RR_Narration.dictActorSprite.ContainsKey(RR_NarrationVisualization.visualData.actor[i]))
            {
                spritePos[i].sprite = RR_Narration.dictActorSprite[RR_NarrationVisualization.visualData.actor[i]];
                color.a = 255;
                spritePos[i].color = color;
                spritePos[i].transform.localPosition = RR_NarrationVisualization.visualData.pos[i];
                spritePos[i].gameObject.transform.localScale = RR_NarrationVisualization.visualData.scale[i];
                spritePos[i].sortingLayerName = "RR-Layers1";
                if (RR_NarrationVisualization.visualData.actor[i] == RR_Narration.dialogue.name + ";;" + RR_Narration.dialogue.expression)
                    spritePos[i].sortingLayerName = "RR-Layers3";

            }
            else
            {
                spritePos[i].sprite = null;
                color.a = 0;
            }
            if (!RR_Narration.dictActorSpine.ContainsKey(name))
            {
                skeletonAnimations[i].gameObject.SetActive(false);
                continue;
            }
            if (!skeletonAnimations[i].gameObject.activeSelf) skeletonAnimations[i].gameObject.SetActive(true);
            // skeletonAnimation.skeletonDataAsset.Clear();
            skeletonAnimations[i].skeletonDataAsset = RR_Narration.dictActorSpine[name].skeletonDataAsset;
            skeletonAnimations[i].Initialize(true);
            skeletonAnimations[i].AnimationName = expression;
            skeletonAnimations[i].gameObject.transform.localPosition = RR_NarrationVisualization.visualData.pos[i];
            skeletonAnimations[i].gameObject.transform.localScale = RR_NarrationVisualization.visualData.scale[i];
            skeletonMesh[i].sortingLayerName = "RR-Layers1";
            if (RR_NarrationVisualization.visualData.actor[i] == RR_Narration.dialogue.name + ";;" + RR_Narration.dialogue.expression)
                skeletonMesh[i].sortingLayerName = "RR-Layers3";
        }
    }
}
