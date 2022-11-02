using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RR.DialogueTools.Extra_Visual;
using RR.DialogueTools.Engine;
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
        for (int i = 0; i < skeletonAnimations.Count;i++)
        {
            if(skeletonMesh.Count < skeletonAnimations.Count) skeletonMesh.Add(null);
            skeletonMesh[i] = skeletonAnimations[i].GetComponent<MeshRenderer>();
        }
        if (!isDim) dim.gameObject.SetActive(false);
        else dim.gameObject.SetActive(true);
        dim.sortingLayerName =  "RR-Layers2";
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
        for (int i = 0; i < Visualization.visual.actorCount; i++)
        {
            name = Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
            expression = "";
            if (Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                expression = Visualization.visualData.actor[i].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];

            spritePos[i].color = color;
            if (Narration.dictActorSprite.ContainsKey(Visualization.visualData.actor[i]))
            {
                spritePos[i].sprite = Narration.dictActorSprite[Visualization.visualData.actor[i]];
                color.a = 255;
                spritePos[i].color = color;
                spritePos[i].transform.localPosition = Visualization.visualData.pos[i];
                spritePos[i].gameObject.transform.localScale = Visualization.visualData.scale[i];
                spritePos[i].sortingLayerName = "RR-Layers1";
                if (Visualization.visualData.actor[i] == Narration.dialogue.name + ";;" + Narration.dialogue.expression)
                    spritePos[i].sortingLayerName = "RR-Layers3";
                
            }
            else
            {
                spritePos[i].sprite = null;
                color.a = 0;
            }
            if (!Narration.dictActorSpine.ContainsKey(name))
            {
                skeletonAnimations[i].gameObject.SetActive(false);
                continue;
            }
            if (!skeletonAnimations[i].gameObject.activeSelf) skeletonAnimations[i].gameObject.SetActive(true);
            // skeletonAnimation.skeletonDataAsset.Clear();
            skeletonAnimations[i].skeletonDataAsset = Narration.dictActorSpine[name].skeletonDataAsset;
            skeletonAnimations[i].Initialize(true);
            skeletonAnimations[i].AnimationName = expression;
            skeletonAnimations[i].gameObject.transform.localPosition = Visualization.visualData.pos[i];
            skeletonAnimations[i].gameObject.transform.localScale = Visualization.visualData.scale[i];
            skeletonMesh[i].sortingLayerName = "RR-Layers1";
            if (Visualization.visualData.actor[i] == Narration.dialogue.name + ";;" + Narration.dialogue.expression)
                    skeletonMesh[i].sortingLayerName = "RR-Layers3";
        }
    }
}
