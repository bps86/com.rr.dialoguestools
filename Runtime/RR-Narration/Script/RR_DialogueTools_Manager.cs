using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RR.DialogueTools.Engine;
using RR.DialogueTools.Audio;
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
    public Image image;
    public bool useBeep;
    public float minPitch = -0.2f, maxPitch = 0.2f, Range = 0;
    bool stop = false;
    public enum IsInverted { True = -1, False = 1 }
    public IsInverted isInverted = IsInverted.False;
    public List<string> Dialogues = new List<string>();
    public string currentDialogue;
    Color color = new Color(255, 255, 255, 0);
    AudioSource audioSource;
    Vector3 spriteV3, spineV3, spriteV3scale, spineV3scale;
    void Awake()
    {
        image.color = color;
        audioSource = gameObject.GetComponent<AudioSource>();
        spriteV3 = image.gameObject.transform.position;
        spineV3 = skeletonAnimation.gameObject.transform.position;
        spriteV3scale = image.gameObject.transform.localScale;
        spineV3scale = skeletonAnimation.gameObject.transform.localScale;
        for ( int i = 0; i < Dialogues.Count;i++)
        Loaders.dialoguesData.Add(Dialogues[i], Resources.Load<TextAsset>("RR-Dialogues/"+Dialogues[i]));
        StartCoroutine(Load());
        button.onClick.AddListener(delegate { NextDialogue(); });
    }

    IEnumerator Load()
    {
        Debug.Log(Loaders.isLoaded);
        StartCoroutine(Loaders.LoadActorData());
        yield return new WaitUntil(() => Loaders.isLoaded);
        // Loaders.LoadDialogueFile(Loaders.dialoguesData["Dialogue1"].text);
        Loaders.LoadDialogueTable(currentDialogue);
        _name.text = "failed loading statics";
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
            Loaders.LoadDialogueTable(currentDialogue);
            stop = true;
            Loaders.LoadDialogueData(tags, index);
            stop = false;
            Refresh(useBeep);
        }
    }
    public void NextDialogue()
    {
        if (index < Loaders.dialogues.Count - 1) index += 1;
        if (index >= Loaders.dialogues.Count - 1) return;
        stop = true;
        Loaders.LoadDialogueData(tags, index);
        stop = false;
        Refresh(useBeep);
    }
    void Refresh(bool isBeep = false)
    {
        Debug.Log(Loaders.dialogue.nameShown);
        _name.text = Loaders.dialogue.nameShown;
        audioSource.clip = null;
        if (Loaders.dictActorBeep.ContainsKey(Loaders.dialogue.name))
        {
            audioSource.clip = Loaders.dictActorBeep[Loaders.dialogue.name];
        }
        if (isBeep)
        {
            Debug.Log("Beep Used");
            Beep();
        }
        if (!isBeep) _dialogue.text = Loaders.dialogue.dialogue;
        image.sprite = Loaders.dialogue.sprite;
        if (Loaders.dictActorSprite.ContainsKey(Loaders.dialogue.name + ";;" + Loaders.dialogue.expression))
        {
            image.sprite = Loaders.dictActorSprite[Loaders.dialogue.name + ";;" + Loaders.dialogue.expression];
            color.a = 255;
            image.transform.position = spriteV3 + new Vector3((float)Loaders.dialogue.charPos * Range, 1, 1);
            image.gameObject.transform.localScale = Vector3.Scale(spriteV3scale, new Vector3((float)isInverted, 1, 1));
        }
        else
        {
            image.sprite = null;
            color.a = 0;
        }
        image.color = color;
        if (Loaders.dialogue.skeletonDataAsset == null)
        {
            skeletonAnimation.gameObject.SetActive(false);
            return;
        }
        if (!skeletonAnimation.gameObject.activeSelf) skeletonAnimation.gameObject.SetActive(true);
        // skeletonAnimation.skeletonDataAsset.Clear();
        skeletonAnimation.skeletonDataAsset = Loaders.dialogue.skeletonDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.AnimationName = Loaders.dialogue.expression;
        skeletonAnimation.gameObject.transform.position = spineV3 + new Vector3((float)Loaders.dialogue.charPos * Range, 1, 1);
        skeletonAnimation.transform.localScale = Vector3.Scale(spineV3scale, new Vector3((float)isInverted, 1, 1));
    }
    void Beep()
    {
        if (audioSource.clip == null) return;
        StartCoroutine(PlayBeep(false, Loaders.dialogue, audioSource, 1 / beepPerSeconds));
    }

    public IEnumerator PlayBeep(bool pause, Dialogue dialogue, AudioSource audioSource, float seconds = 0.1f)
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
            Player.PlayBeep(audioSource, minPitch, maxPitch);
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
