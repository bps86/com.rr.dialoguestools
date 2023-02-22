using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

[RequireComponent(typeof(AudioSource))]
public class RR_DialogueTools_Manager : MonoBehaviour
{
    public string tags;
    public int index, beepPerSeconds;
    public TMP_Text _name, _dialogue;
    // bool ready = false;
    public Button button;
    public SkeletonAnimation skeletonAnimation;
    public SpriteRenderer sprite;
    public bool useBeep;
    public float minPitch = -0.2f, maxPitch = 0.2f, Range = 0;
    bool stop = false;
    public enum IsInverted { True = -1, False = 1 }
    public IsInverted isInverted = IsInverted.False;
    public bool useExtra_Visual;
    public RR_DialogueTools_Extra rR_DialogueTools_Extra;
    public List<string> Dialogues = new List<string>();
    public string currentDialogue;
    public bool useLocalization;
    Color color = new Color(255, 255, 255, 0);
    AudioSource audioSource;
    Vector3 spriteV3, spineV3, spriteV3scale, spineV3scale;
    void Awake()
    {
        sprite.color = color;
        audioSource = gameObject.GetComponent<AudioSource>();
        spriteV3 = sprite.gameObject.transform.position;
        spineV3 = skeletonAnimation.gameObject.transform.position;
        spriteV3scale = sprite.gameObject.transform.localScale;
        spineV3scale = skeletonAnimation.gameObject.transform.localScale;
        for (int i = 0; i < Dialogues.Count; i++)
        {
            RR_Narration.dialoguesData.Add(Dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/" + Dialogues[i]));
            if (useExtra_Visual) RR_NarrationVisualization.visualAssets.Add(Dialogues[i], Resources.Load<TextAsset>("RR-Visual/" + Dialogues[i]));
        }
        StartCoroutine(Load());
        button.onClick.AddListener(delegate { NextDialogue(); });
    }

    IEnumerator Load()
    {
        Debug.Log(RR_Narration.isLoaded);
        StartCoroutine(RR_Narration.LoadActorData());
        yield return new WaitUntil(() => RR_Narration.isLoaded);
        if (!useLocalization) RR_Narration.LoadDialogueFile(RR_Narration.dialoguesData[currentDialogue].text);
        else RR_Narration.LoadDialogueTable(currentDialogue);
        if (useExtra_Visual) RR_NarrationVisualization.LoadVisualAsset(RR_NarrationVisualization.visualAssets[currentDialogue].text);
        Refresh(useBeep);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextDialogue();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!useLocalization) RR_Narration.LoadDialogueFile(RR_Narration.dialoguesData[currentDialogue].text);
            else RR_Narration.LoadDialogueTable(currentDialogue);
            stop = true;
            RR_Narration.LoadDialogueData(tags, index);
            stop = false;
            Refresh(useBeep);
        }
    }
    public void NextDialogue()
    {
        if (index < RR_Narration.dialogues.Count - 1) index += 1;
        if (index >= RR_Narration.dialogues.Count) return;
        stop = true;
        RR_Narration.LoadDialogueData(tags, index);
        if (useExtra_Visual) RR_NarrationVisualization.LoadVisualData(tags, index);
        stop = false;
        Refresh(useBeep);
    }
    void Refresh(bool isBeep = false)
    {
        Debug.Log(RR_Narration.dialogue.nameShown);
        _name.text = RR_Narration.dialogue.nameShown;
        audioSource.clip = null;
        if (RR_Narration.dictActorBeep.ContainsKey(RR_Narration.dialogue.name))
        {
            audioSource.clip = RR_Narration.dictActorBeep[RR_Narration.dialogue.name];
        }
        if (isBeep)
        {
            Debug.Log("Beep Used");
            Beep();
        }
        if (!isBeep) _dialogue.text = RR_Narration.dialogue.dialogue;
        if (!useExtra_Visual)
        {
            sprite.color = color;
            if (RR_Narration.dictActorSprite.ContainsKey(RR_Narration.dialogue.name + ";;" + RR_Narration.dialogue.expression))
            {
                sprite.sprite = RR_Narration.dictActorSprite[RR_Narration.dialogue.name + ";;" + RR_Narration.dialogue.expression];
                color.a = 255;
                sprite.color = color;
                sprite.transform.position = spriteV3 + new Vector3((float)RR_Narration.dialogue.charPos * Range, 1, 1);
                sprite.gameObject.transform.localScale = Vector3.Scale(spriteV3scale, new Vector3((float)isInverted, 1, 1));
            }
            else
            {
                sprite.sprite = null;
                color.a = 0;
            }
            if (RR_Narration.dialogue.skeletonDataAsset == null)
            {
                skeletonAnimation.gameObject.SetActive(false);
                return;
            }
            if (!skeletonAnimation.gameObject.activeSelf) skeletonAnimation.gameObject.SetActive(true);
            // skeletonAnimation.skeletonDataAsset.Clear();
            skeletonAnimation.skeletonDataAsset = RR_Narration.dialogue.skeletonDataAsset;
            skeletonAnimation.Initialize(true);
            skeletonAnimation.AnimationName = RR_Narration.dialogue.expression;
            skeletonAnimation.gameObject.transform.position = spineV3 + new Vector3((float)RR_Narration.dialogue.charPos * Range, 1, 1);
            skeletonAnimation.transform.localScale = Vector3.Scale(spineV3scale, new Vector3((float)isInverted, 1, 1));
        }
        if (useExtra_Visual)
        {
            rR_DialogueTools_Extra.ChangeAnimPos();
        }
    }
    void Beep()
    {
        if (audioSource.clip == null) return;
        StartCoroutine(PlayBeep(false, RR_Narration.dialogue, audioSource, 1 / beepPerSeconds));
    }

    public IEnumerator PlayBeep(bool pause, RRDialogue dialogue, AudioSource audioSource, float seconds = 0.1f)
    {
        int index = 0;
        int skipIndex = -1;
        bool scanDone = false;
        Debug.Log("dialogue length: " + dialogue.dialogue.Length);
        for (int i = 0; i < dialogue.dialogue.Length; i++)
        {
            if (stop) yield break;
            if (index < skipIndex)
            {
                index += 1;
                continue;
            }
            if (dialogue.dialogue[index].ToString() == "<")
            {

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
    int getIndex(int index, string dialogue, ref bool scanDone)
    {
        for (int i = index; i < dialogue.Length; i++)
        {
            if (dialogue[i].ToString() == ">")
            {
                Debug.Log(dialogue.Substring(index, i - index + 1));
                scanDone = true;
                return i;
            }
        }
        scanDone = true;
        return -1;
    }
}
