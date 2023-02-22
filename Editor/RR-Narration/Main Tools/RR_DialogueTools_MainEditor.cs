using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
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
        if (RR_Narration.dialogues == null) EditorTools.currentLocaleIndex = EditorTools.locales.Length - 1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI()
    {
        if (RR_Narration.dialogues != null)
        {
            EditorTools.fileData = DialoguesToString();
            RR_NarrationVisualization.visual = GetVisualAsset();
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
        if (RR_Narration.dialogues != null)
        {
            if (GUILayout.Button("Save"))
            {
                ready = false;
                RR_DialogueTools_SaveFileWindow.init();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("New Dialogue")) RR_Narration.dialogues.Add(new RRDialogue());
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDialogue()
    {
        if (!ready) return;
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (RR_Narration.dialogues != null)
            for (int i = 0; i < RR_Narration.dialogues.Count; i++)
            {
                string nameThumb = RR_Narration.dialogues[i].name;
                string expressionThumb = RR_Narration.dialogues[i].expression;
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
                RR_Narration.dialogues[i].dialogue = GUILayout.TextArea(RR_Narration.dialogues[i].dialogue, GUILayout.Width(300), GUILayout.Height(117));
                GUILayout.BeginVertical(GUILayout.Width(60));
                RR_Narration.dialogues[i].tags = GUILayout.TextField(RR_Narration.dialogues[i].tags, GUILayout.Width(60));
                RR_Narration.dialogues[i].nameMode = (RRDialogue.NameMode)EditorGUILayout.EnumPopup(RR_Narration.dialogues[i].nameMode);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                RR_Narration.dialogues[i].index = EditorGUILayout.IntField(RR_Narration.dialogues[i].index, GUILayout.Width(60));
                RR_Narration.dialogues[i].charPos = (RRDialogue.CharPos)EditorGUILayout.EnumPopup(RR_Narration.dialogues[i].charPos, GUILayout.Width(60));
                if (GUILayout.Button("Remove", GUILayout.Width(60))) RR_Narration.dialogues.RemoveAt(i);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        GUILayout.EndScrollView();
    }

    static string DialoguesToString(string fileData = "")
    {
        for (int i = 0; i < RR_Narration.dialogues.Count; i++)
        {
            if (i > 0) fileData += "||";
            fileData += DialogueToString(RR_Narration.dialogues[i], ";");
        }
        return fileData;
    }
    static string DialogueToString(RRDialogue dialogue, string separator, string dialoguedata = "")
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
    static RR_NarrationVisual GetVisualAsset()
    {
        for (int i = 0; i < RR_Narration.dialogues.Count; i++)
        {
            if (RR_NarrationVisualization.visual.visualDatas.Count < RR_Narration.dialogues.Count) RR_NarrationVisualization.visual.visualDatas.Add(new RR_NarrationVisualData());
            RR_NarrationVisualization.visual.visualDatas[i].tags = RR_Narration.dialogues[i].tags;
            RR_NarrationVisualization.visual.visualDatas[i].index = RR_Narration.dialogues[i].index;
        }
        for (int i = RR_NarrationVisualization.visual.visualDatas.Count - 1; i >= 0; i--)
        {
            if (RR_NarrationVisualization.visual.visualDatas.Count > RR_Narration.dialogues.Count) RR_NarrationVisualization.visual.visualDatas.RemoveAt(i);
        }
        return RR_NarrationVisualization.visual;
    }
    // static Visual CreateVisualAsset()
    // {

    // }
    void OnDestroy()
    {
        EditorTools.Refresh_RR_DialogueTools();
    }
}