using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_VisualEditor : EditorWindow
{
    float currentScrollViewWidth;
    bool resize = false;
    public static bool ready = false;
    private Vector2 scrollPos = Vector2.zero, scrollPos2 = Vector2.zero;
    Rect cursorChangeRect;
    // public static EditorWindow getWindow, thisWindow;
    public Object textAsset;
    public TextAsset config;
    public List<string> clipboardActor;
    public List<Vector3> clipboardPos = new List<Vector3>(),
    clipboardScale = new List<Vector3>();
    RR_Narration_Visualization rR_Narration_Visualization;
    RR_Narration rR_Narration;
    RR_DialogueTools_FileManagerWindow rR_DialogueTools_FileManagerWindow;

    [MenuItem("Window/RR/Visual Editor")]
    public static void init() {
        Debug.Log("a");
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_VisualEditor thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        thisWindow.textAsset = new Object();
        thisWindow.rR_Narration_Visualization = new RR_Narration_Visualization();
        thisWindow.rR_Narration_Visualization.visual = new RR_NarrationVisual();
        thisWindow.Show();
    }
    void OnEnable() {
        RR_EditorTools.locales = RR_EditorTools.GetLocales(new string[] { });
        if (rR_Narration.dialogues == null) RR_EditorTools.currentLocaleIndex = RR_EditorTools.locales.Length - 1;
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI() {
        // if (Loaders.dialogues != null) RR_EditorTools.fileData = DialoguesToString();
        GUILayout.BeginHorizontal();
        DrawOptions();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        // this place is to draw
        DrawVisualPos();
        GUILayout.EndHorizontal();
        Repaint();
    }

    private void DrawOptions() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Load your config here:");
        textAsset = EditorGUILayout.ObjectField(textAsset, typeof(TextAsset), true);
        if (textAsset != null && GUILayout.Button("Load")) {
            Debug.Log(textAsset.name);
            config = (TextAsset)textAsset;
            Debug.Log(config.text);
            if (!System.String.IsNullOrEmpty(config.text)) JsonUtility.FromJsonOverwrite(config.text, rR_Narration_Visualization.visual);
            else rR_Narration_Visualization.visual = new RR_NarrationVisual();
        }
        if (textAsset != null && GUILayout.Button("Save")) {
            string visualJson = JsonUtility.ToJson(rR_Narration_Visualization.visual);
            Debug.Log(visualJson);
            RR_NarrationFunctions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + textAsset.name + ".json", visualJson);
            RR_EditorTools.Refresh_RR_DialogueTools();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawVisualPos() {
        if (rR_Narration_Visualization.visual != null && textAsset != null) {
            CheckActor();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Actors: ", GUILayout.Width(60));
            rR_Narration_Visualization.visual.actorCount = EditorGUILayout.IntField(rR_Narration_Visualization.visual.actorCount, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
            for (int i = 0; i < rR_Narration_Visualization.visual.visualDatas.Count; i++) {
                GUILayout.BeginVertical();
                GUILayout.Label("Tags:" + rR_Narration_Visualization.visual.visualDatas[i].tags);
                GUILayout.Label("Index:" + rR_Narration_Visualization.visual.visualDatas[i].index);
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < rR_Narration_Visualization.visual.visualDatas[i].actor.Count; ii++) {
                    string name = rR_Narration_Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
                    string expression = "";
                    if (rR_Narration_Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                        expression = rR_Narration_Visualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];
                    GUILayout.BeginVertical();
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + name + "," + expression + ".png"), GUILayout.Width(100), GUILayout.Height(100))) {
                        int index = i;
                        int index2 = ii;
                        RR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Visual, index, index2);
                        GUIUtility.ExitGUI();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(100)))
                        rR_Narration_Visualization.visual.visualDatas[i].actor[ii] = "";
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                for (int ii = 0; ii < rR_Narration_Visualization.visual.visualDatas[i].pos.Count; ii++) {
                    GUILayout.BeginHorizontal();
                    rR_Narration_Visualization.visual.visualDatas[i].pos[ii] = EditorGUILayout.Vector3Field("pos" + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].pos[ii], GUILayout.Width(400));
                    rR_Narration_Visualization.visual.visualDatas[i].scale[ii] = EditorGUILayout.Vector3Field("scale" + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].scale[ii], GUILayout.Width(400));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80))) {
                    clipboardActor = rR_Narration_Visualization.visual.visualDatas[i].actor;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].actor = clipboardActor;
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80))) {
                    clipboardPos = rR_Narration_Visualization.visual.visualDatas[i].pos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].pos = clipboardPos;
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80))) {
                    clipboardScale = rR_Narration_Visualization.visual.visualDatas[i].scale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].scale = clipboardScale;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }

    private void CheckActor() {
        for (int i = 0; i < rR_Narration_Visualization.visual.visualDatas.Count; i++) {
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].actor.Count < rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].actor.Add("");
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].actor.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].actor.Count > rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].actor.RemoveAt(ii);
            }
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].pos.Count < rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].pos.Add(new Vector3());
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].pos.Count > rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].pos.RemoveAt(ii);
            }
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].scale.Count < rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].scale.Add(new Vector3(1, 1, 1));
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].scale.Count > rR_Narration_Visualization.visual.actorCount) rR_Narration_Visualization.visual.visualDatas[i].scale.RemoveAt(ii);
            }
        }
    }
    private void Saving() {
        string data = JsonUtility.ToJson(rR_Narration_Visualization.visual);
        EditorUtility.SetDirty(textAsset);
        rR_DialogueTools_FileManagerWindow.init_Window(FileMode.Save);
        GUIUtility.ExitGUI();
        ready = true;
    }
}