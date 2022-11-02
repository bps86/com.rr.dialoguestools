using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using RR.DialogueTools.Engine;
using RR.DialogueTools.Editor;
using RR.DialogueTools.Extra_Visual;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_MainEditor : EditorWindow
{
    float currentScrollViewWidth;
    bool resize = false;
    public static bool ready = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    public static EditorWindow getWindow, thisWindow;


    [MenuItem("Window/RR/Narration")]
    static void init()
    {
        EditorTools.Initialize_RR_Dir();
        thisWindow = (RR_DialogueTools_MainEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_MainEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        thisWindow.Show();
    }

    void OnEnable()
    {
        EditorTools.locales = EditorTools.GetLocales(new string[] { });
        if (Narration.dialogues == null) EditorTools.currentLocaleIndex = EditorTools.locales.Length - 1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI()
    {
        if (Narration.dialogues != null)
        {
            EditorTools.fileData = DialoguesToString();
            Visualization.visual = GetVisualAsset();
        }
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
        if (GUILayout.Button("New"))
        {
            ready = false;
            RR_DialogueTools_NewFileWindow.init();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Open"))
        {
            ready = false;
            RR_DialogueTools_OpenFileWindow.init();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Refresh"))
        {
            EditorTools.Refresh_RR_DialogueTools();
        }
        if (Narration.dialogues != null)
        {
            if (GUILayout.Button("Save"))
            {
                ready = false;
                RR_DialogueTools_SaveFileWindow.init();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("New Dialogue")) Narration.dialogues.Add(new Dialogue());
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDialogue()
    {
        if (!ready) return;
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (Narration.dialogues != null)
            for (int i = 0; i < Narration.dialogues.Count; i++)
            {
                string nameThumb = Narration.dialogues[i].name;
                string expressionThumb = Narration.dialogues[i].expression;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(100));
                GUILayout.Label(nameThumb);
                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + nameThumb + "," + expressionThumb + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
                {
                    int index = i;
                    RR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Dialogue, index);
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndVertical();
                Narration.dialogues[i].dialogue = GUILayout.TextArea(Narration.dialogues[i].dialogue, GUILayout.Width(300), GUILayout.Height(117));
                GUILayout.BeginVertical(GUILayout.Width(60));
                Narration.dialogues[i].tags = GUILayout.TextField(Narration.dialogues[i].tags, GUILayout.Width(60));
                Narration.dialogues[i].nameMode = (Dialogue.NameMode)EditorGUILayout.EnumPopup(Narration.dialogues[i].nameMode);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                Narration.dialogues[i].index = EditorGUILayout.IntField(Narration.dialogues[i].index, GUILayout.Width(60));
                Narration.dialogues[i].charPos = (Dialogue.CharPos)EditorGUILayout.EnumPopup(Narration.dialogues[i].charPos, GUILayout.Width(60));
                if (GUILayout.Button("Remove", GUILayout.Width(60))) Narration.dialogues.RemoveAt(i);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        GUILayout.EndScrollView();
    }

    static string DialoguesToString(string fileData = "")
    {
        for (int i = 0; i < Narration.dialogues.Count; i++)
        {
            if (i > 0) fileData += "||";
            fileData += DialogueToString(Narration.dialogues[i], ";");
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
    static Visual GetVisualAsset()
    {
        for (int i = 0; i < Narration.dialogues.Count; i++)
        {
            if (Visualization.visual.visualDatas.Count < Narration.dialogues.Count) Visualization.visual.visualDatas.Add(new VisualData());
            Visualization.visual.visualDatas[i].tags = Narration.dialogues[i].tags;
            Visualization.visual.visualDatas[i].index = Narration.dialogues[i].index;
        }
        for (int i = Visualization.visual.visualDatas.Count - 1; i >= 0; i--)
        {
            if (Visualization.visual.visualDatas.Count > Narration.dialogues.Count) Visualization.visual.visualDatas.RemoveAt(i);
        }
        return Visualization.visual;
    }
    // static Visual CreateVisualAsset()
    // {

    // }
    void OnDestroy()
    {
        EditorTools.Refresh_RR_DialogueTools();
    }
}

public class RR_DialogueTools_NewFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_NewFileWindow thisWindow = (RR_DialogueTools_NewFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_NewFileWindow));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        thisWindow.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        EditorTools.fileName = EditorGUILayout.TextField(EditorTools.fileName);
        EditorTools.currentLocaleIndex = EditorGUILayout.Popup(EditorTools.currentLocaleIndex, EditorTools.locales);
        EditorTools.currentLocale = EditorTools.locales[EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK"))
            NewFile();
        GUILayout.EndVertical();
    }

    void NewFile()
    {
        // if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale) && RR_DialogueTools_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale);
        // if (RR_DialogueTools_EditorTools.currentLocaleIndex < RR_DialogueTools_EditorTools.locales.Length - 1)
        //     Manager.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale + "/" + RR_DialogueTools_EditorTools.fileName + ".txt");
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            Manager.NewFile("RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt");
        EditorTools.CreateNewVisualAsset();
        EditorTools.ToStringTable(EditorTools.fileName, "", EditorTools.rrDialoguesTable);
        RR_DialogueTools_OpenFileWindow.OpenFile();
        Close();
    }
}

public class RR_DialogueTools_OpenFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_OpenFileWindow thisWindow = (RR_DialogueTools_OpenFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_OpenFileWindow));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        thisWindow.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        EditorTools.fileName = EditorGUILayout.TextField(EditorTools.fileName);
        EditorTools.currentLocaleIndex = EditorGUILayout.Popup(EditorTools.currentLocaleIndex, EditorTools.locales);
        EditorTools.currentLocale = EditorTools.locales[EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK"))
        {
            OpenFile();
            Close();
        }
        GUILayout.EndVertical();
    }
    public static void OpenFile()
    {
        if (EditorTools.currentLocaleIndex < EditorTools.locales.Length - 1)
        {
            StringTable stringTable = AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + EditorTools.currentLocale + ".asset");
            StringTableEntry stringTableEntry = EditorTools.GetStringTableEntry(stringTable, EditorTools.fileName);
            string[] dialogueList = stringTableEntry.Value.Split(new string[] { "||" }, System.StringSplitOptions.None);
            Narration.dialogues = Manager.GetDialogues(dialogueList);
        }
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            Narration.dialogues = Manager.OpenDialogueFile("Assets/RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt");
        if (!File.Exists("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json"))
            EditorTools.CreateNewVisualAsset();
        JsonUtility.FromJsonOverwrite(Manager.OpenFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json"), Visualization.visual);
        EditorTools.GetDialogueIndex();
        EditorTools.Refresh_RR_DialogueTools();
        RR_DialogueTools_MainEditor.ready = true;

    }
}

public class RR_DialogueTools_SaveFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_SaveFileWindow thisWindow = (RR_DialogueTools_SaveFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_SaveFileWindow));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        thisWindow.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        EditorTools.fileName = EditorGUILayout.TextField(EditorTools.fileName);
        EditorTools.currentLocaleIndex = EditorGUILayout.Popup(EditorTools.currentLocaleIndex, EditorTools.locales);
        EditorTools.currentLocale = EditorTools.locales[EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK"))
            SaveFile();
        GUILayout.EndVertical();
    }
    void SaveFile()
    {
        Debug.Log("Saving");
        // if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale) && RR_DialogueTools_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale);
        // if (RR_DialogueTools_EditorTools.currentLocaleIndex < RR_DialogueTools_EditorTools.locales.Length - 1)
        //     Manager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale + "/" + RR_DialogueTools_EditorTools.fileName + ".txt", RR_DialogueTools_EditorTools.fileData);
        // Debug.Log("Checked Save on Locale");
        EditorTools.ToStringTable(EditorTools.fileName, EditorTools.fileData, EditorTools.rrDialoguesTable);
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            Manager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt", EditorTools.fileData);
        string visualJson = JsonUtility.ToJson(Visualization.visual);
        Manager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json", visualJson);
        Debug.Log("Checked Save on Non Locale");
        RR_DialogueTools_OpenFileWindow.OpenFile();
        Debug.Log("FileRefreshed");
        Close();
    }
}

public class RR_DialogueTools_ActorManager : EditorWindow
{
    float currentScrollViewWidth;
    bool resize = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    static int index = 0,
    dynamicIndex, optionalIndex;
    static string tempName;
    static List<string> tempExpression;
    public enum Mode { Dialogue, Visual }
    public static Mode mode;


    // [MenuItem("Window/RR/ActorManager")]
    public static void init(RR_DialogueTools_ActorManager.Mode selectedMode, int currentIndex, int secondIndex = 0)
    {
        dynamicIndex = currentIndex;
        optionalIndex = secondIndex;
        mode = selectedMode;
        EditorTools.Refresh_RR_DialogueTools();
        tempName = EditorTools.names[index];
        tempExpression = GetExpression(EditorTools.expression[EditorTools.names[index]], new List<string>());
        RR_DialogueTools_ActorManager window = (RR_DialogueTools_ActorManager)EditorWindow.GetWindow(typeof(RR_DialogueTools_ActorManager));
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
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Actors");
        for (int i = 0; i < EditorTools.names.Length; i++)
        {
            if (GUILayout.Button(EditorTools.names[i]))
            {
                index = i;
                tempName = EditorTools.names[i];
                tempExpression = GetExpression(EditorTools.expression[EditorTools.names[i]], new List<string>());
            }
        }
        GUILayout.EndScrollView();
    }
    void DrawActorData()
    {
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        GUILayout.Space(10);
        GUILayout.Label(tempName);
        for (int i = 0; i < tempExpression.Count; i++)
            if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + tempName + "," + tempExpression[i] + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
            {
                if (mode == RR_DialogueTools_ActorManager.Mode.Dialogue)
                {
                    Narration.dialogues[dynamicIndex].name = tempName;
                    Narration.dialogues[dynamicIndex].expression = tempExpression[i];
                }
                if (mode == RR_DialogueTools_ActorManager.Mode.Visual)
                {
                    Visualization.visual.visualDatas[dynamicIndex].actor[optionalIndex] = tempName + ";;" + tempExpression[i];
                }
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

public class RR_DialogueTools_VisualEditor : EditorWindow
{
    float currentScrollViewWidth;
    bool resize = false;
    public static bool ready = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    public static EditorWindow getWindow, thisWindow;
    public static Object textAsset;
    public TextAsset config;
    public List<string> clipboardActor;
    public List<Vector3> clipboardPos = new List<Vector3>(),
    clipboardScale = new List<Vector3>();

    [MenuItem("Window/RR/Visual Editor")]
    static void init()
    {
        EditorTools.Initialize_RR_Dir();
        if(textAsset == null) Visualization.visual = new Visual();
        thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        thisWindow.Show();
    }
    void OnEnable()
    {
        EditorTools.locales = EditorTools.GetLocales(new string[] { });
        if (Narration.dialogues == null) EditorTools.currentLocaleIndex = EditorTools.locales.Length - 1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI()
    {
        // if (Loaders.dialogues != null) EditorTools.fileData = DialoguesToString();
        GUILayout.BeginHorizontal();
        DrawOptions();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        // this place is to draw
        DrawVisualPos();
        GUILayout.EndHorizontal();
        Repaint();
    }

    private void DrawOptions()
    {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Load your config here:");
        textAsset = EditorGUILayout.ObjectField(textAsset, typeof(TextAsset), true);
        if (textAsset != null && GUILayout.Button("Load"))
        {
            Debug.Log(textAsset.name);
            config = (TextAsset)textAsset;
            Debug.Log(config.text);
            if (!System.String.IsNullOrEmpty(config.text)) JsonUtility.FromJsonOverwrite(config.text, Visualization.visual);
            else Visualization.visual = new Visual();
        }
        if (textAsset != null && GUILayout.Button("Save"))
        {
            string visualJson = JsonUtility.ToJson(Visualization.visual);
            Debug.Log(visualJson);
            Manager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + textAsset.name + ".json", visualJson);
            EditorTools.Refresh_RR_DialogueTools();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawVisualPos()
    {
        if (Visualization.visual != null && textAsset !=null)
        {
            CheckActor();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Actors: ", GUILayout.Width(60));
            Visualization.visual.actorCount = EditorGUILayout.IntField(Visualization.visual.actorCount, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
            for (int i = 0; i < Visualization.visual.visualDatas.Count; i++)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Tags:" + Visualization.visual.visualDatas[i].tags);
                GUILayout.Label("Index:" + Visualization.visual.visualDatas[i].index);
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < Visualization.visual.visualDatas[i].actor.Count; ii++)
                {
                    string name = Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
                    string expression = "";
                    if (Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                        expression = Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];
                    GUILayout.BeginVertical();
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + name + "," + expression + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        int index = i;
                        int index2 = ii;
                        RR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Visual, index, index2);
                        GUIUtility.ExitGUI();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(100)))
                        Visualization.visual.visualDatas[i].actor[ii] = "";
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                for (int ii = 0; ii < Visualization.visual.visualDatas[i].pos.Count; ii++)
                {
                    GUILayout.BeginHorizontal();
                    Visualization.visual.visualDatas[i].pos[ii] = EditorGUILayout.Vector3Field("pos" + ii + ": ", Visualization.visual.visualDatas[i].pos[ii], GUILayout.Width(400));
                    Visualization.visual.visualDatas[i].scale[ii] = EditorGUILayout.Vector3Field("scale" + ii + ": ", Visualization.visual.visualDatas[i].scale[ii], GUILayout.Width(400));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80)))
                {
                    clipboardActor = Visualization.visual.visualDatas[i].actor;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80)))
                {
                    Visualization.visual.visualDatas[i].actor = clipboardActor;
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80)))
                {
                    clipboardPos = Visualization.visual.visualDatas[i].pos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80)))
                {
                    Visualization.visual.visualDatas[i].pos = clipboardPos;
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80)))
                {
                    clipboardScale = Visualization.visual.visualDatas[i].scale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80)))
                {
                    Visualization.visual.visualDatas[i].scale = clipboardScale;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }

    private void CheckActor()
    {
        for (int i = 0; i < Visualization.visual.visualDatas.Count; i++)
        {
            for (int ii = 0; ii < Visualization.visual.actorCount; ii++)
            {
                if (Visualization.visual.visualDatas[i].actor.Count < Visualization.visual.actorCount) Visualization.visual.visualDatas[i].actor.Add("");
            }
            for (int ii = Visualization.visual.visualDatas[i].actor.Count - 1; ii >= 0; ii--)
            {
                if (Visualization.visual.visualDatas[i].actor.Count > Visualization.visual.actorCount) Visualization.visual.visualDatas[i].actor.RemoveAt(ii);
            }
            for (int ii = 0; ii < Visualization.visual.actorCount; ii++)
            {
                if (Visualization.visual.visualDatas[i].pos.Count < Visualization.visual.actorCount) Visualization.visual.visualDatas[i].pos.Add(new Vector3());
            }
            for (int ii = Visualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--)
            {
                if (Visualization.visual.visualDatas[i].pos.Count > Visualization.visual.actorCount) Visualization.visual.visualDatas[i].pos.RemoveAt(ii);
            }
            for (int ii = 0; ii < Visualization.visual.actorCount; ii++)
            {
                if (Visualization.visual.visualDatas[i].scale.Count < Visualization.visual.actorCount) Visualization.visual.visualDatas[i].scale.Add(new Vector3(1, 1, 1));
            }
            for (int ii = Visualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--)
            {
                if (Visualization.visual.visualDatas[i].scale.Count > Visualization.visual.actorCount) Visualization.visual.visualDatas[i].scale.RemoveAt(ii);
            }
        }
    }
    private void Saving()
    {
        string data = JsonUtility.ToJson(Visualization.visual);
        // EditorUtility.SetDirty(textAsset);
        // RR_DialogueTools_SaveFileWindow.init();
        // GUIUtility.ExitGUI();
        ready = true;
    }
}

namespace RR.DialogueTools.Editor
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

    public static class EditorTools
    {
        public static string fileName, fileData;
        static string[] spritePath = new string[] { };
        static string[] spinePath = new string[] { };
        static string[] beepPath = new string[] { };
        public static string[] names = new string[] { }, locales = new string[] { };
        public static Dictionary<string, int> actorDataIndex = new Dictionary<string, int>();
        public static Dictionary<string, string[]> expression = new Dictionary<string, string[]>();
        public static List<bool> hidename;
        public static string currentLocale;
        public static int currentLocaleIndex;
        public static string getFileName;
        public static StringTableCollection rrDialoguesTable;
        public static void GetDialogueIndex()
        {
            for (int i = 0; i < Narration.dialogues.Count; i++)
            {
                EditorTools.actorDataIndex["Actor" + i] = 0;
                EditorTools.actorDataIndex["Expression" + i] = 0;
            }
        }
        [MenuItem("Tools/RR/Initialize RR Dir & Settings")]
        public static void Initialize_RR_Dir()
        {
            LocalizationSettings localizationSettings = null;
            if (!Directory.Exists("Assets/RR-Narration")) Directory.CreateDirectory("Assets/RR-Narration");
            if (!Directory.Exists("Assets/RR-Narration/Resources")) Directory.CreateDirectory("Assets/RR-Narration/Resources");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-DialoguesTable");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors-Spine")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors-Spine");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Beep")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Beep");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Voice-Act")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Voice-Act");
            if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Visual")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Visual");
            if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
            if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");
            if (LocalizationEditorSettings.ActiveLocalizationSettings != null)
            {
                localizationSettings = LocalizationEditorSettings.ActiveLocalizationSettings;
                if (LocalizationEditorSettings.GetLocales().Count > 0)
                    if (!File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset") || !File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue Shared Data.asset"))
                    {
                        LocalizationEditorSettings.CreateStringTableCollection("RR-Dialogue", "Assets/RR-Narration/Resources/RR-DialoguesTable", LocalizationEditorSettings.GetLocales());
                    }
            }
            if (File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset"))
            {
                rrDialoguesTable = AssetDatabase.LoadAssetAtPath<StringTableCollection>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset");
                List<Locale> missingLocale = new List<Locale>();
                for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
                    if (!rrDialoguesTable.ContainsTable(LocalizationEditorSettings.GetLocales()[i].Identifier))
                        missingLocale.Add(LocalizationEditorSettings.GetLocales()[i]);
                for (int i = 0; i < missingLocale.Count; i++)
                    rrDialoguesTable.AddNewTable(missingLocale[i].Identifier);
            }

            EditorTools.CheckSortingLayers();

            EditorTools.Refresh_RR_DialogueTools();
        }
        [MenuItem("Tools/RR/Refresh RR AssetPaths")]
        public static void Refresh_RR_DialogueTools()
        {
            spritePath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Actors/");
            spinePath = Directory.GetDirectories("Assets/RR-Narration/Resources/RR-Actors-Spine/");
            beepPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Beep/");
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
        // [MenuItem("Testing/AddSortLayer")]
        public static void CheckSortingLayers()
        {
            var tagMan = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            if (tagMan == null) return;
            var so = new SerializedObject(tagMan);
            var m_SortingLayers = so.FindProperty("m_SortingLayers");
            Debug.Log(m_SortingLayers.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue);
            int uid1 = CheckIdDuplicates(m_SortingLayers);
            int uid2 = CheckIdDuplicates(m_SortingLayers);
            int uid3 = CheckIdDuplicates(m_SortingLayers);
            CreateRRLayers("RR-Layers1", uid1, so, m_SortingLayers);
            CreateRRLayers("RR-Layers2", uid2, so, m_SortingLayers);
            CreateRRLayers("RR-Layers3", uid3, so, m_SortingLayers);
            EditorUtility.SetDirty(tagMan);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static int CheckIdDuplicates(SerializedProperty sp)
        {
            int uniqueID = GenerateUID();
            for (int i = 0; i < sp.arraySize; i++)
            {
                if (sp.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueId") == null)
                    continue;
                if (uniqueID == 0 || uniqueID == sp.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueId").intValue)
                {
                    uniqueID = GenerateUID();
                    i = -1;
                    continue;
                }
                continue;
            }
            return uniqueID;
        }

        public static void CreateRRLayers(string name, int uid, SerializedObject so, SerializedProperty m_SortingLayers)
        {
            var num_SortingLayers = m_SortingLayers.arraySize;
            for (int i = 0; i < num_SortingLayers; i++)
            {
                if (name == m_SortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue) return;
            }
            m_SortingLayers.InsertArrayElementAtIndex(num_SortingLayers);
            m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("name").stringValue = name;
            m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("uniqueID").intValue = uid;
            m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("locked").boolValue = false;
            so.ApplyModifiedProperties();
            so.Update();
        }

        public static void ToStringTable(string _filename, string _fileData, StringTableCollection stringTableCollection)
        {
            if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1) return;
            for (int i = 0; i < stringTableCollection.StringTables.Count; i++)
            {
                if (stringTableCollection.StringTables[i].name == "RR-Dialogue_" + EditorTools.currentLocale)
                {
                    SetStringTable(_filename, _fileData, AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + EditorTools.currentLocale + ".asset"));
                }
            }
        }
        public static void SetStringTable(string _filename, string _fileData, StringTable stringTable)
        {
            StringTableEntry entry = GetStringTableEntry(stringTable, _filename);
            entry.Value = _fileData;
            EditorUtility.SetDirty(stringTable);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static StringTableEntry GetStringTableEntry(StringTable stringTable, string entry)
        {
            return stringTable.GetEntry(entry) ?? stringTable.AddEntry(entry, string.Empty);
        }

        public static string[] GetLocales(string[] localesArray)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
            {
                names.Add(LocalizationEditorSettings.GetLocales()[i].Identifier.Code);
            }
            names.Add("<None>");
            return names.ToArray();
        }

        public static void CreateNewVisualAsset()
        {
            Visualization.visual = new Visual();
            string visualJson = JsonUtility.ToJson(Visualization.visual);
            Manager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json", visualJson);
        }

        static int GenerateUID()
        {
            int newGuid = System.Guid.NewGuid().GetHashCode();
            if (newGuid < 0) newGuid *= -1;
            return newGuid;
        }
    }
}