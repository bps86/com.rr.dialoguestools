using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using Spine.Unity;

namespace RR.DialogueTools.Engine
{
    public class Dialogue
    {
        public string name;
        public string nameShown;
        public string expression;
        [TextArea]
        public string dialogue;
        public string tags;
        public int index;
        public enum NameMode { Normal = 0, Hidden = 1, None = 2 }
        public NameMode nameMode;
        public enum CharPos { Left = -1, Center = 0, Right = 1 }
        public CharPos charPos;
        public Sprite sprite;
        public SkeletonDataAsset skeletonDataAsset;
        public Dialogue(string _name = "name", string _expression = "expression", string _dialogue = "dialogue", string _tags = "tags", int _index = 0, NameMode _nameMode = NameMode.Normal, CharPos _charPos = CharPos.Center)
        {
            this.name = _name;
            this.expression = _expression;
            this.dialogue = _dialogue;
            this.tags = _tags;
            this.index = _index;
            this.nameMode = _nameMode;
            this.nameShown = GetName(_nameMode);
            this.charPos = _charPos;
        }
        public string GetName(NameMode _nameMode)
        {
            if (_nameMode == NameMode.Hidden) return "?";
            if (_nameMode == NameMode.None) return "";
            return this.name;
        }
    }
    public class ActorSpine
    {
        public string name;
        public SkeletonDataAsset skeletonDataAsset;
        public ActorSpine(string _name, string path)
        {
            this.name = _name;
            this.skeletonDataAsset = Resources.Load<SkeletonDataAsset>(path);
        }
    }
    public static class Manager
    {

        public static void NewFile(string fileName)
        {
            string newFile = null;
            using StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + fileName);
            writer.Write(newFile);
            writer.Close();
        }
        public static string OpenFile(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();
            return json;
        }
        public static List<Dialogue> OpenDialogueFile(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();
            string[] dialoguesList = json.Split(new string[]{"||"},System.StringSplitOptions.None);
            return GetDialogues(dialoguesList);
        }
        public static void SaveFile(string path, string fileData)
        {
            using StreamWriter write = new StreamWriter(path);
            write.Write(fileData);
            write.Close();
        }

        public static List<Dialogue> GetDialogues(string[] dialoguesList)
        {
            List<Dialogue> tmpDialogues = new List<Dialogue>();
            if (dialoguesList.Length == 1 && System.String.IsNullOrEmpty(dialoguesList[0]))
            {
                tmpDialogues.Add(new Dialogue());
                return tmpDialogues;
            }
            for (int i = 0; i < dialoguesList.Length; i++)
            {
                string[] dialoguedata = dialoguesList[i].Split(new string[]{";"},System.StringSplitOptions.None);
                tmpDialogues.Add(new Dialogue());
                tmpDialogues[i].name = dialoguedata[0];
                tmpDialogues[i].expression = dialoguedata[1];
                tmpDialogues[i].dialogue = dialoguedata[2];
                tmpDialogues[i].tags = dialoguedata[3];
                tmpDialogues[i].index = int.Parse(dialoguedata[4]);
                tmpDialogues[i].nameMode = (Dialogue.NameMode)int.Parse(dialoguedata[5]);
                tmpDialogues[i].nameShown = tmpDialogues[i].GetName(tmpDialogues[i].nameMode);
                tmpDialogues[i].charPos = (Dialogue.CharPos)int.Parse(dialoguedata[6]);
            }
            return tmpDialogues;
        }
    }
    public static class Narration
    {
        public static Dialogue dialogue;
        public static List<Dialogue> dialogues;
        public static Dictionary<string, TextAsset> dialoguesData = new Dictionary<string, TextAsset>();
        public static Dictionary<string, Dialogue> dictDialogues = new Dictionary<string, Dialogue>();
        public static Dictionary<string, Sprite> dictActorSprite = new Dictionary<string, Sprite>();
        public static Dictionary<string, ActorSpine> dictActorSpine = new Dictionary<string, ActorSpine>();
        public static Dictionary<string, AudioClip> dictActorBeep = new Dictionary<string, AudioClip>();
        public static LocalizedStringTable localizedDialogueTable = new LocalizedStringTable(tableReference: "RR-Dialogue");
        public static bool isLoaded = false;
        public static IEnumerator LoadActorData()
        {
            if (isLoaded) yield break;
            TextAsset spriteDataPath = Resources.Load<TextAsset>("spritePaths");
            TextAsset spineDataPath = Resources.Load<TextAsset>("spinePaths");
            TextAsset beepDataPath = Resources.Load<TextAsset>("beepPaths");
            string[] spritePath = spriteDataPath.text.Split(';');
            string[] spinePath = spineDataPath.text.Split(';');
            string[] beepPath = beepDataPath.text.Split(';');
            for (int i = 0; i < spritePath.Length; i++)
            {
                if (System.String.IsNullOrEmpty(spritePath[i])) continue;
                string name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
                string expression = spritePath[i].Substring(spritePath[i].LastIndexOf(',') + 1, spritePath[i].Length - spritePath[i].LastIndexOf(',') - 1);
                dictActorSprite.Add(name + ";;" + expression, Resources.Load<Sprite>(spritePath[i]));
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < spinePath.Length; i++)
            {
                if (System.String.IsNullOrEmpty(spinePath[i])) continue;
                string name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
                dictActorSpine.Add(name, new ActorSpine(name, "RR-Actors-Spine/" + name + "/skeleton_SkeletonData"));
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < beepPath.Length; i++)
            {
                if (System.String.IsNullOrEmpty(beepPath[i])) continue;
                string name = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1);
                dictActorBeep.Add(name, Resources.Load<AudioClip>(beepPath[i]));
                yield return new WaitForFixedUpdate();
            }
            isLoaded = true;
            yield break;
        }

        public static void LoadDialogueTable(string tableKey)
        {
            LoadDialogueFile(Narration.localizedDialogueTable.GetTable()[tableKey].LocalizedValue);

        }
        public static void LoadDialogueFile(string _dialoguedata)
        {
            dictDialogues.Clear();
            dialogues = Manager.GetDialogues(_dialoguedata.Split(new string[]{"||"},System.StringSplitOptions.None));
            if (dialogues.Count < 1) dialogues.Add(new Dialogue());
            for (int i = 0; i < dialogues.Count; i++)
                dictDialogues[dialogues[i].tags + ";" + dialogues[i].index] = dialogues[i];
            LoadDialogueData(dialogues[0].tags, dialogues[0].index);
        }
        public static void LoadDialogueData(string tags, int index)
        {
            dialogue = dictDialogues[tags + ";" + index];
            if (dictActorSprite.ContainsKey(dialogue.name + ";;" + dialogue.expression)) dialogue.sprite = dictActorSprite[dialogue.name + ";;" + dialogue.expression];
            else dialogue.sprite = null;
            if (dictActorSpine.ContainsKey(dialogue.name))
            {
                dialogue.skeletonDataAsset = dictActorSpine[dialogue.name].skeletonDataAsset;
            }
            else dialogue.skeletonDataAsset = null;
        }
    }
}

namespace RR.DialogueTools.Extra_Audio
{
    public static class Player
    {
        public static void PlayBeep(AudioSource audioSource, float minPitch = -0.2f, float maxPitch = 0.2f)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
    }
}

namespace RR.DialogueTools.Extra_Visual
{
    public class Visual
    {
        public int actorCount;
        public List<VisualData> visualDatas = new List<VisualData>();
    }
    [System.Serializable]
    public class VisualData
    {
        public string tags;
        public int index;
        public List<string> actor = new List<string>();
        public List<Vector3> pos = new List<Vector3>(),
        scale = new List<Vector3>();
    }
    public static class Visualization
    {
        public static Visual visual = new Visual();
        public static VisualData visualData = new VisualData();
        public static Dictionary<string,TextAsset> visualAssets = new Dictionary<string, TextAsset>();
        public static Dictionary<string,VisualData> dictVisualDatas = new Dictionary<string, VisualData>();
        public static void LoadVisualAsset(string visualAsset)
        {
            dictVisualDatas.Clear();
            JsonUtility.FromJsonOverwrite(visualAsset,visual);
            if (visual.visualDatas.Count < 1) visual.visualDatas.Add(new VisualData());
            for (int i = 0; i < visual.visualDatas.Count; i++)
                dictVisualDatas[visual.visualDatas[i].tags + ";" + visual.visualDatas[i].index] = visual.visualDatas[i];
            LoadVisualData(visual.visualDatas[0].tags, visual.visualDatas[0].index);
        }
        public static void LoadVisualData(string tags, int index)
        {
            visualData = dictVisualDatas[tags+";"+index];
        }
    }
    
}