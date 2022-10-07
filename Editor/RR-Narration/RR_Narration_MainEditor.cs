using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using RR.Narration;
using RR.Narration.Editor;
using Spine.Collections;
using UnityEditor.Localization;

public class RR_Narration_MainEditor : EditorWindow
{
    public static string fileName, fileData;
    float currentScrollViewWidth;
    bool resize = false;
    public static bool ready = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    public static EditorWindow getWindow, thisWindow;

    [MenuItem("Window/RR/Narration")]
    static void init()
    {
        if (!Directory.Exists("Assets/RR-Narration")) Directory.CreateDirectory("Assets/RR-Narration");
        if (!Directory.Exists("Assets/RR-Narration/Resources")) Directory.CreateDirectory("Assets/RR-Narration/Resources");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors-Spine")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors-Spine");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Beep")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Beep");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Voice-Act")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Voice-Act");
        if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
        if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");
        RR_Narration_EditorTools.RefreshActorManager();
        thisWindow = (RR_Narration_MainEditor)EditorWindow.GetWindow(typeof(RR_Narration_MainEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        thisWindow.Show();
    }

    void OnEnable()
    {
        RR_Narration_EditorTools.locales = GetLocales(new string[] { });
        RR_Narration_EditorTools.currentLocaleIndex = RR_Narration_EditorTools.locales.Length-1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI()
    {
        if (Loaders.dialogues != null) fileData = DialoguesToString();
        GUILayout.BeginHorizontal();
        DrawOptions();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawDialogue();
        GUILayout.EndHorizontal();
        Repaint();
    }

    private void DrawOptions()
    {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("File Name");
        fileName = EditorGUILayout.TextField(fileName);
        RR_Narration_EditorTools.currentLocaleIndex = EditorGUILayout.Popup(RR_Narration_EditorTools.currentLocaleIndex, RR_Narration_EditorTools.locales);
        RR_Narration_EditorTools.currentLocale = RR_Narration_EditorTools.locales[RR_Narration_EditorTools.currentLocaleIndex];
        if (GUILayout.Button("New"))
        {
            NewFile();
        }
        if (GUILayout.Button("Open"))
        {
            ready = false;
            OpenFile();
        }
        if (GUILayout.Button("Refresh"))
        {
            RR_Narration_EditorTools.RefreshActorManager();
        }
        if (Loaders.dialogues != null)
        {
            if (GUILayout.Button("Save")) SaveFile();
            if (GUILayout.Button("New Dialogue")) Loaders.dialogues.Add(new Dialogue());
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDialogue()
    {
        if (!ready) return;
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (Loaders.dialogues != null)
            if (!System.String.IsNullOrEmpty(Loaders.dialogues[0].dialogue))
                for (int i = 0; i < Loaders.dialogues.Count; i++)
                {
                    string nameThumb = Loaders.dialogues[i].name;
                    string expressionThumb = Loaders.dialogues[i].expression;
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(100));
                    GUILayout.Label(nameThumb);
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + nameThumb + "," + expressionThumb + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        int index = i;
                        RR_Narration_ActorManager.init(index);
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.EndVertical();
                    Loaders.dialogues[i].dialogue = GUILayout.TextArea(Loaders.dialogues[i].dialogue, GUILayout.Width(300), GUILayout.Height(117));
                    GUILayout.BeginVertical(GUILayout.Width(60));
                    Loaders.dialogues[i].tags = GUILayout.TextField(Loaders.dialogues[i].tags, GUILayout.Width(60));
                    Loaders.dialogues[i].nameMode = (Dialogue.NameMode)EditorGUILayout.Popup((int)Loaders.dialogues[i].nameMode, new string[] { "Normal", "Hidden", "None" }, GUILayout.Width(60));
                    GUILayout.EndVertical();
                    Loaders.dialogues[i].index = EditorGUILayout.IntField(Loaders.dialogues[i].index, GUILayout.Width(60));
                    if (GUILayout.Button("Remove", GUILayout.Width(60))) Loaders.dialogues.RemoveAt(i);
                    GUILayout.EndHorizontal();
                }
        GUILayout.EndScrollView();
    }
    string[] GetLocales(string[] localesArray)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
        {
            names.Add(LocalizationEditorSettings.GetLocales()[i].Identifier.Code);
        }
        names.Add("<None>");
        return names.ToArray();
    }

    static string DialoguesToString(string fileData = "")
    {
        for (int i = 0; i < Loaders.dialogues.Count; i++)
        {
            if (i > 0) fileData += ";";
            fileData += DialogueToString(Loaders.dialogues[i], "_");
        }
        return fileData;
    }
    static string DialogueToString(Dialogue dialogue, string separator, string dialoguedata = "")
    {
        dialoguedata += dialogue.name + separator;
        dialoguedata += dialogue.expression + separator;
        dialoguedata += dialogue.dialogue + separator;
        dialoguedata += dialogue.tags + separator;
        dialoguedata += dialogue.index.ToString() + separator;
        dialoguedata += (int)dialogue.nameMode + separator;
        dialoguedata += (int)dialogue.charPos;
        return dialoguedata;
    }

    void NewFile()
    {
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale)) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale);
        if (RR_Narration_EditorTools.currentLocaleIndex < RR_Narration_EditorTools.locales.Length - 1)
            Manager.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale + "/" + RR_Narration_MainEditor.fileName + ".txt");
        if (RR_Narration_EditorTools.currentLocaleIndex == RR_Narration_EditorTools.locales.Length - 1)
            Manager.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_Narration_MainEditor.fileName + ".txt");
        AssetDatabase.Refresh();
        if (RR_Narration_EditorTools.currentLocaleIndex < RR_Narration_EditorTools.locales.Length - 1)
            Loaders.dialogues = Manager.OpenFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale + "/" + RR_Narration_MainEditor.fileName + ".txt");
        if (RR_Narration_EditorTools.currentLocaleIndex == RR_Narration_EditorTools.locales.Length - 1)
            Loaders.dialogues = Manager.OpenFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_MainEditor.fileName + ".txt");
    }

    void OpenFile()
    {
        if (RR_Narration_EditorTools.currentLocaleIndex < RR_Narration_EditorTools.locales.Length - 1)
            Loaders.dialogues = Manager.OpenFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale + "/" + RR_Narration_MainEditor.fileName + ".txt");
        if (RR_Narration_EditorTools.currentLocaleIndex == RR_Narration_EditorTools.locales.Length - 1)
            Loaders.dialogues = Manager.OpenFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_MainEditor.fileName + ".txt");
        RR_Narration_EditorTools.GetDialogueIndex();
        RR_Narration_EditorTools.RefreshActorManager();
        RR_Narration_MainEditor.ready = true;
    }

    void SaveFile()
    {
        Debug.Log("Saving");
        if (RR_Narration_EditorTools.currentLocaleIndex < RR_Narration_EditorTools.locales.Length - 1)
            Manager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_EditorTools.currentLocale + "/" + RR_Narration_MainEditor.fileName + ".txt", RR_Narration_MainEditor.fileData);
        Debug.Log("Checked Save on Locale");
        if (RR_Narration_EditorTools.currentLocaleIndex == RR_Narration_EditorTools.locales.Length - 1)
            Manager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_Narration_MainEditor.fileName + ".txt", RR_Narration_MainEditor.fileData);
        Debug.Log("Checked Save on Non Locale");
        OpenFile();
        Debug.Log("FileRefreshed");
    }
    void OnDestroy()
    {
        RR_Narration_EditorTools.RefreshActorManager();
    }
}

public class RR_Narration_ActorManager : EditorWindow
{
    float currentScrollViewWidth;
    bool resize = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    static int index = 0,
    indexDialogue;
    static string tempName;
    static List<string> tempExpression;

    // [MenuItem("Window/RR/ActorManager")]
    public static void init(int indexD)
    {
        // RR_Narration_MainEditor.ready = false;
        indexDialogue = indexD;
        RR_Narration_EditorTools.RefreshActorManager();
        tempName = RR_Narration_EditorTools.names[index];
        tempExpression = GetExpression(RR_Narration_EditorTools.expression[RR_Narration_EditorTools.names[index]], new List<string>());
        RR_Narration_ActorManager window = (RR_Narration_ActorManager)EditorWindow.GetWindow(typeof(RR_Narration_ActorManager));
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
        window.Show();
    }
    void OnEnable()
    {
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GetActors();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawActorData();
        GUILayout.EndHorizontal();
        Repaint();
    }
    void GetActors()
    {
        GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Actors");
        for (int i = 0; i < RR_Narration_EditorTools.names.Length; i++)
        {
            if (GUILayout.Button(RR_Narration_EditorTools.names[i]))
            {
                index = i;
                tempName = RR_Narration_EditorTools.names[i];
                tempExpression = GetExpression(RR_Narration_EditorTools.expression[RR_Narration_EditorTools.names[i]], new List<string>());
            }
        }
        GUILayout.EndScrollView();
    }
    void DrawActorData()
    {
        GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        GUILayout.Space(10);
        GUILayout.Label(tempName);
        for (int i = 0; i < tempExpression.Count; i++)
            if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + tempName + "," + tempExpression[i] + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
            {
                Loaders.dialogues[indexDialogue].name = tempName;
                Loaders.dialogues[indexDialogue].expression = tempExpression[i];
                Close();
            }
        GUILayout.EndScrollView();
    }
    static List<string> GetExpression(string[] expressions, List<string> expressions2)
    {
        for (int i = 0; i < expressions.Length; i++)
        {
            expressions2.Add(expressions[i]);
        }
        return expressions2;
    }
}
namespace RR.Narration.Editor
{
    public static class EditorDrawTools
    {
        public static void ResizeScrollView(ref Rect cursorChangeRect, Rect defaultRect, ref bool resize, ref float currentScrollViewWidth)
        {
            GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);
            cursorChangeRect.height = defaultRect.height;

            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
                resize = true;
            if (Event.current.type == EventType.MouseUp) resize = false;
            if (resize)
            {
                currentScrollViewWidth = Event.current.mousePosition.x;
                cursorChangeRect.Set(currentScrollViewWidth, cursorChangeRect.y, cursorChangeRect.width, cursorChangeRect.height);
            }
        }
    }

    public static class RR_Narration_EditorTools
    {
        static string[] spritePath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Actors/");
        static string[] spinePath = Directory.GetDirectories("Assets/RR-Narration/Resources/RR-Actors-Spine/");
        static string[] beepPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Beep/");
        public static string[] names = new string[] { }, locales = new string[] { };
        public static Dictionary<string, int> actorDataIndex = new Dictionary<string, int>();
        public static Dictionary<string, string[]> expression = new Dictionary<string, string[]>();
        public static List<bool> hidename;
        public static string currentLocale;
        public static int currentLocaleIndex;
        public static string getFileName;
        public static void GetDialogueIndex()
        {
            for (int i = 0; i < Loaders.dialogues.Count; i++)
            {
                RR_Narration_EditorTools.actorDataIndex["Actor" + i] = 0;
                RR_Narration_EditorTools.actorDataIndex["Expression" + i] = 0;
            }
        }
        [MenuItem("Tools/RR/Refresh Actor AssetPaths")]
        public static void RefreshActorManager()
        {
            int index = 0;
            int nameIndex = 0;
            int beepIndex = 0;
            List<string> temp = new List<string>();
            List<string> tempSprite = new List<string>();
            List<string> tempSpriteActor = new List<string>();
            Dictionary<string, List<string>> dictSprite = new Dictionary<string, List<string>>();
            List<string> tempSpine = new List<string>();
            string spritePaths = "";
            string spinePaths = "";
            string beepPaths = "";
            if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
            if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");

            for (int i = 0; i < spritePath.Length; i++)
            {
                if (spritePath[i].Contains(".meta"))
                {
                    continue;
                }
                tempSprite.Add(spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf('.') - spritePath[i].LastIndexOf('/') - 1));
                if (nameIndex > 0) spritePaths += ";";
                spritePaths += "RR-Actors/" + tempSprite[nameIndex];
                File.Copy(spritePath[i], "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSprite[nameIndex] + ".png", true);
                nameIndex += 1;
                string name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
                if (temp.Contains(name))
                {
                    continue;
                }
                temp.Add(name);
                tempSpriteActor.Add(name);
                dictSprite[name] = new List<string>();
                index += 1;
            }
            for (int i = 0; i < spinePath.Length; i++)
            {
                string name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
                if (temp.Contains(name))
                {
                    continue;
                }
                temp.Add(name);
                tempSpine.Add(name);
            }
            for (int i = 0; i < beepPath.Length; i++)
            {
                if (beepPath[i].Contains(".meta"))
                {
                    continue;
                }
                string beepName = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1, beepPath[i].LastIndexOf('.') - beepPath[i].LastIndexOf('/') - 1);
                if (beepIndex > 0) beepPaths += ";";
                beepPaths += "RR-Sound/Beep/" + beepName;
                beepIndex += 1;
            }
            names = temp.ToArray();
            for (int i = 0; i < tempSprite.Count; i++)
            {
                string name = tempSprite[i].Substring(0, tempSprite[i].LastIndexOf(','));
                string expression = tempSprite[i].Substring(tempSprite[i].LastIndexOf(',') + 1, tempSprite[i].Length - tempSprite[i].LastIndexOf(',') - 1);
                dictSprite[name].Add(expression);
            }
            for (int i = 0; i < tempSpriteActor.Count; i++)
            {
                expression[tempSpriteActor[i]] = dictSprite[tempSpriteActor[i]].ToArray();
            }
            for (int i = 0; i < tempSpine.Count; i++)
            {
                List<string> name = new List<string>();
                if (i > 0) spinePaths += ";";
                spinePaths += "RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i];
                Spine.Unity.SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>("Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/skeleton_SkeletonData.asset");
                Spine.SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
                Spine.Animation[] animations = skeletonData.Animations.ToArray();
                for (int _index = 0; _index < animations.Length; _index++)
                {
                    name.Add(animations[_index].Name);
                    string thumbPath = "Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png";
                    if (File.Exists(thumbPath))
                    {
                        File.Copy(thumbPath, "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png", true);
                        File.Delete(thumbPath);
                        File.Delete(thumbPath + ".meta");
                    }
                }
                expression[tempSpine[i]] = name.ToArray();
            }
            Manager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spritePaths.txt", spritePaths);
            Manager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spinePaths.txt", spinePaths);
            Manager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/beepPaths.txt", beepPaths);
            AssetDatabase.Refresh();
        }
    }

}