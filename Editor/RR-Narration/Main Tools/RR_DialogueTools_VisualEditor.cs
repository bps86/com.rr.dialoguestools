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

    [MenuItem("Window/RR/Visual Editor")]
    public static void init() {
        Debug.Log("a");
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_VisualEditor thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1000, 600);
        thisWindow.Show();
    }
    void OnEnable() {
        textAsset = new Object();
        rR_Narration_Visualization = new RR_Narration_Visualization();
        rR_Narration_Visualization.visual = new RR_NarrationVisual();
        rR_Narration = new RR_Narration();
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

    // private void OpenFileManagerWindow(FileMode fileMode) {
    //     // RR_DialogueTools_FileManagerWindow rR_DialogueTools_FileManagerWindow = new RR_DialogueTools_FileManagerWindow(fileName, fileData);
    //     RR_DialogueTools_FileManagerWindow rR_DialogueTools_FileManagerWindow = (RR_DialogueTools_FileManagerWindow)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_FileManagerWindow));
    //     rR_DialogueTools_FileManagerWindow.init(rR_Narration, rR_Narration_Visualization);
    //     rR_DialogueTools_FileManagerWindow.OpenRR += OnOpen;
    //     rR_DialogueTools_FileManagerWindow.init_Window(fileMode);
    //     GUIUtility.ExitGUI();
    // }

    private void OpenActorManagerWindow(int index, int index2) {
        RR_DialogueTools_ActorManager rR_DialogueTools_ActorManager = (RR_DialogueTools_ActorManager)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_ActorManager));
        rR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Visual, index, index2);
        rR_DialogueTools_ActorManager.SetRRVar(rR_Narration, rR_Narration_Visualization);
        rR_DialogueTools_ActorManager.SetRRVarEvent += OnOpen;
        rR_DialogueTools_ActorManager.init_Window();
        GUIUtility.ExitGUI();
    }

    private void DrawVisualPos() {
        if (rR_Narration_Visualization.visual != null && textAsset != null) {
            CheckActor();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Actors: ", GUILayout.Width(60));
            rR_Narration_Visualization.visual.actorCount = EditorGUILayout.IntField(rR_Narration_Visualization.visual.actorCount, GUILayout.Width(60));
            rR_Narration_Visualization.visual.animMode = (TransitionMode)EditorGUILayout.EnumPopup("Transition Mode", rR_Narration_Visualization.visual.animMode);
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
                        OpenActorManagerWindow(index, index2);
                        GUIUtility.ExitGUI();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(100)))
                        rR_Narration_Visualization.visual.visualDatas[i].actor[ii] = "";
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                for (int ii = 0; ii < rR_Narration_Visualization.visual.visualDatas[i].endPos.Count; ii++) {
                    if (rR_Narration_Visualization.visual.animMode == TransitionMode.Static) {
                        GUILayout.BeginHorizontal();
                        rR_Narration_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("pos " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].endPos[ii], GUILayout.Width(400));
                        rR_Narration_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("scale " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].endScale[ii], GUILayout.Width(400));
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    } else if (rR_Narration_Visualization.visual.animMode == TransitionMode.MovePosition) {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        rR_Narration_Visualization.visual.visualDatas[i].startPos[ii] = EditorGUILayout.Vector3Field("Start pos " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].startPos[ii], GUILayout.Width(400));
                        rR_Narration_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("End pos " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].endPos[ii], GUILayout.Width(400));
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                        rR_Narration_Visualization.visual.visualDatas[i].startScale[ii] = EditorGUILayout.Vector3Field("Start scale " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].startScale[ii], GUILayout.Width(400));
                        rR_Narration_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("End scale " + ii + ": ", rR_Narration_Visualization.visual.visualDatas[i].endScale[ii], GUILayout.Width(400));
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(20);
                    }
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80))) {
                    clipboardActor = rR_Narration_Visualization.visual.visualDatas[i].actor;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].actor = clipboardActor;
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80))) {
                    clipboardPos = rR_Narration_Visualization.visual.visualDatas[i].endPos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].endPos = clipboardPos;
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80))) {
                    clipboardScale = rR_Narration_Visualization.visual.visualDatas[i].endScale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80))) {
                    rR_Narration_Visualization.visual.visualDatas[i].endScale = clipboardScale;
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
                if (rR_Narration_Visualization.visual.visualDatas[i].actor.Count < rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].actor.Add("");
                }
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].actor.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].actor.Count > rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].actor.RemoveAt(ii);
                }
            }
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].endPos.Count < rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].endPos.Add(new Vector3());
                }
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].endPos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].endPos.Count > rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].endPos.RemoveAt(ii);
                }
            }
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].endScale.Count < rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].endScale.Add(new Vector3(1, 1, 1));
                }
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].endPos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].endScale.Count > rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].endScale.RemoveAt(ii);
                }
            }


            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].startPos.Count < rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].startPos.Add(new Vector3());
                }
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].startPos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].startPos.Count > rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].startPos.RemoveAt(ii);
                }
            }
            for (int ii = 0; ii < rR_Narration_Visualization.visual.actorCount; ii++) {
                if (rR_Narration_Visualization.visual.visualDatas[i].startScale.Count < rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].startScale.Add(new Vector3(1, 1, 1));
                }
            }
            for (int ii = rR_Narration_Visualization.visual.visualDatas[i].startPos.Count - 1; ii >= 0; ii--) {
                if (rR_Narration_Visualization.visual.visualDatas[i].startScale.Count > rR_Narration_Visualization.visual.actorCount) {
                    rR_Narration_Visualization.visual.visualDatas[i].startScale.RemoveAt(ii);
                }
            }
        }
    }
    // private void Saving() {
    //     string data = JsonUtility.ToJson(rR_Narration_Visualization.visual);
    //     EditorUtility.SetDirty(textAsset);
    //     rR_DialogueTools_FileManagerWindow.init_Window(FileMode.Save);
    //     GUIUtility.ExitGUI();
    //     ready = true;
    // }

    private void OnOpen(RR_Narration selected_RR_Narration, RR_Narration_Visualization selected_RR_Narration_Visualization) {
        this.rR_Narration = selected_RR_Narration;
        this.rR_Narration_Visualization = selected_RR_Narration_Visualization;
    }
}