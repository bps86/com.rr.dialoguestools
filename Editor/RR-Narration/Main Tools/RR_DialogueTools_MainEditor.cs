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
    // public static EditorWindow getWindow, thisWindow;
    RR_Narration rR_Narration;
    RR_Narration_Visualization rR_Narration_Visualization;
    RR_DialogueTools_FileManagerWindow rR_DialogueTools_FileManagerWindow;


    [MenuItem("Window/RR/Narration")]
    static void init() {
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_MainEditor thisWindow = (RR_DialogueTools_MainEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_MainEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        // thisWindow.rR_Narration = new RR_Narration();
        // thisWindow.rR_Narration_Visualization = new RR_Narration_Visualization();
        // thisWindow.rR_DialogueTools_FileManagerWindow = new RR_DialogueTools_FileManagerWindow();
        thisWindow.Show();
    }

    void OnEnable() {
        Debug.Log("Enabled");
        rR_Narration = new RR_Narration();
        rR_Narration_Visualization = new RR_Narration_Visualization();
        rR_DialogueTools_FileManagerWindow = (RR_DialogueTools_FileManagerWindow)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_FileManagerWindow));
        rR_DialogueTools_FileManagerWindow.init();
        rR_DialogueTools_FileManagerWindow.Open += OnOpen;
        RR_EditorTools.locales = RR_EditorTools.GetLocales(new string[] { });
        if (rR_Narration.dialogues == null) Debug.Log("is Null");
        if (rR_Narration.dialogues == null) RR_EditorTools.currentLocaleIndex = RR_EditorTools.locales.Length - 1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI() {
        if (rR_Narration.dialogues != null) {
            RR_EditorTools.fileData = DialoguesToString();
            rR_Narration_Visualization.visual = GetVisualAsset();
        }
        GUILayout.BeginHorizontal();
        DrawOptions();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawDialogue();
        GUILayout.EndHorizontal();
        Repaint();
    }

    private void DrawOptions() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        if (GUILayout.Button("New")) {
            ready = false;
            rR_DialogueTools_FileManagerWindow.init_Window(FileMode.New);
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Open")) {
            ready = false;
            rR_DialogueTools_FileManagerWindow.init_Window(FileMode.Open);
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Refresh")) {
            RR_EditorTools.Refresh_RR_DialogueTools();
        }
        if (rR_Narration.dialogues != null) {
            if (GUILayout.Button("Save")) {
                ready = false;
                rR_DialogueTools_FileManagerWindow.init_Window(FileMode.Save);
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("New Dialogue")) rR_Narration.dialogues.Add(new RR_Dialogue());
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDialogue() {
        if (!ready) return;
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (rR_Narration.dialogues != null)
            for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
                string nameThumb = rR_Narration.dialogues[i].name;
                string expressionThumb = rR_Narration.dialogues[i].expression;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(100));
                GUILayout.Label(nameThumb);
                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + nameThumb + "," + expressionThumb + ".png"), GUILayout.Width(100), GUILayout.Height(100))) {
                    int index = i;
                    RR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Dialogue, index);
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndVertical();
                rR_Narration.dialogues[i].dialogue = GUILayout.TextArea(rR_Narration.dialogues[i].dialogue, GUILayout.Width(300), GUILayout.Height(117));
                GUILayout.BeginVertical(GUILayout.Width(60));
                rR_Narration.dialogues[i].tags = GUILayout.TextField(rR_Narration.dialogues[i].tags, GUILayout.Width(60));
                rR_Narration.dialogues[i].nameMode = (NameMode)EditorGUILayout.EnumPopup(rR_Narration.dialogues[i].nameMode);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                rR_Narration.dialogues[i].index = EditorGUILayout.IntField(rR_Narration.dialogues[i].index, GUILayout.Width(60));
                rR_Narration.dialogues[i].charPos = (CharPos)EditorGUILayout.EnumPopup(rR_Narration.dialogues[i].charPos, GUILayout.Width(60));
                if (GUILayout.Button("Remove", GUILayout.Width(60))) rR_Narration.dialogues.RemoveAt(i);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        GUILayout.EndScrollView();
    }

    public string DialoguesToString(string fileData = "") {
        for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
            if (i > 0) fileData += "||";
            fileData += DialogueToString(rR_Narration.dialogues[i], ";");
        }
        return fileData;
    }
    static string DialogueToString(RR_Dialogue dialogue, string separator, string dialoguedata = "") {
        dialoguedata += dialogue.name + separator;
        dialoguedata += dialogue.expression + separator;
        dialoguedata += dialogue.dialogue + separator;
        dialoguedata += dialogue.tags + separator;
        dialoguedata += dialogue.index.ToString() + separator;
        dialoguedata += (int)dialogue.nameMode + separator;
        dialoguedata += (int)dialogue.charPos;
        return dialoguedata;
    }
    public RR_NarrationVisual GetVisualAsset() {
        for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
            if (rR_Narration_Visualization.visual.visualDatas.Count < rR_Narration.dialogues.Count) rR_Narration_Visualization.visual.visualDatas.Add(new RR_NarrationVisualData());
            rR_Narration_Visualization.visual.visualDatas[i].tags = rR_Narration.dialogues[i].tags;
            rR_Narration_Visualization.visual.visualDatas[i].index = rR_Narration.dialogues[i].index;
        }
        for (int i = rR_Narration_Visualization.visual.visualDatas.Count - 1; i >= 0; i--) {
            if (rR_Narration_Visualization.visual.visualDatas.Count > rR_Narration.dialogues.Count) rR_Narration_Visualization.visual.visualDatas.RemoveAt(i);
        }
        return rR_Narration_Visualization.visual;
    }
    // static Visual CreateVisualAsset()
    // {

    // }
    private void OnOpen(RR_Narration selected_RR_Narration, RR_Narration_Visualization selected_RR_Narration_Visualization) {
        this.rR_Narration = selected_RR_Narration;
        this.rR_Narration_Visualization = selected_RR_Narration_Visualization;
    }
    void OnDestroy() {
        RR_EditorTools.Refresh_RR_DialogueTools();
    }
}