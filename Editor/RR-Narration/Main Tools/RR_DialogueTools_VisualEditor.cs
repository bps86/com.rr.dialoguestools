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
    public List<string> clipboardExpression;
    public List<Vector3> clipboardPos = new List<Vector3>(),
    clipboardScale = new List<Vector3>();
    RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    RR_Narration rR_Narration;

    [MenuItem("Window/RR/Visual Editor")]
    public static void init() {
        Debug.Log("a");
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_VisualEditor thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1080, 600);
        thisWindow.Show();
    }
    void OnEnable() {
        textAsset = new Object();
        rR_DialogueTools_Visualization = new RR_DialogueTools_Visualization();
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
        RR_EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
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
            config = (TextAsset)textAsset;
            if (!System.String.IsNullOrEmpty(config.text)) {
                JsonUtility.FromJsonOverwrite(config.text, rR_DialogueTools_Visualization.visual);
            } else {
                rR_DialogueTools_Visualization.visual = new RR_DialogueTools_Visual();
            }
        }
        if (rR_DialogueTools_Visualization.visual != null && GUILayout.Button("Save")) {
            string visualJson = JsonUtility.ToJson(rR_DialogueTools_Visualization.visual);
            Debug.Log(visualJson);
            RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + textAsset.name + ".json", visualJson);
            RR_EditorTools.Refresh_RR_DialogueTools();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OpenActorManagerWindow(int index, int index2) {
        RR_DialogueTools_ActorManager rR_DialogueTools_ActorManager = (RR_DialogueTools_ActorManager)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_ActorManager));
        rR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Visual, index, index2);
        rR_DialogueTools_ActorManager.SetRRVar(rR_Narration, rR_DialogueTools_Visualization);
        rR_DialogueTools_ActorManager.SetRRVarEvent += OnOpen;
        rR_DialogueTools_ActorManager.init_Window();
        GUIUtility.ExitGUI();
    }

    private void DrawVisualPos() {
        if (rR_DialogueTools_Visualization.visual != null && textAsset != null) {
            CheckActor();
            GUILayout.BeginVertical();
            if (rR_DialogueTools_Visualization.visual != null) {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Actors: ", GUILayout.Width(60));
                rR_DialogueTools_Visualization.visual.actorCount = EditorGUILayout.IntSlider(rR_DialogueTools_Visualization.visual.actorCount, 1, 4);
                rR_DialogueTools_Visualization.visual.animMode = (TransitionMode)EditorGUILayout.EnumPopup("Transition Mode", rR_DialogueTools_Visualization.visual.animMode);
                GUILayout.EndHorizontal();
            }
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
            for (int i = 0; i < rR_DialogueTools_Visualization.visual.visualDatas.Count; i++) {
                GUILayout.BeginVertical();
                GUILayout.Label("Tags:" + rR_DialogueTools_Visualization.visual.visualDatas[i].tags);
                GUILayout.Label("Index:" + rR_DialogueTools_Visualization.visual.visualDatas[i].index);
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count; ii++) {
                    string name = rR_DialogueTools_Visualization.visual.visualDatas[i].actorName[ii];
                    string expression = rR_DialogueTools_Visualization.visual.visualDatas[i].expression[ii];
                    GUILayout.BeginVertical();
                    GUILayout.Label("Name: " + name, GUILayout.Width(100));
                    // GUILayout.Label("Expr: " + rR_DialogueTools_Visualization.visual.visualDatas[i].expression[ii], GUILayout.Width(100));
                    GUILayout.Label("Expr: " + expression, GUILayout.Width(100));
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + name + "," + expression + ".png"), GUILayout.Width(100), GUILayout.Height(100))) {
                        int index = i;
                        int index2 = ii;
                        Debug.Log(rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count);
                        OpenActorManagerWindow(index, index2);
                        GUIUtility.ExitGUI();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(100))) {
                        rR_DialogueTools_Visualization.visual.visualDatas[i].actorName[ii] = "";
                        rR_DialogueTools_Visualization.visual.visualDatas[i].expression[ii] = "";
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                if (rR_DialogueTools_Visualization.visual.animMode == TransitionMode.MovePosition) {
                    GUILayout.BeginHorizontal(GUILayout.Width(150));
                    GUILayout.Label("Transition Duration: ");
                    rR_DialogueTools_Visualization.visual.visualDatas[i].transitionDuration = EditorGUILayout.FloatField(rR_DialogueTools_Visualization.visual.visualDatas[i].transitionDuration);
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count; ii++) {
                    if (rR_DialogueTools_Visualization.visual.animMode == TransitionMode.Static) {
                        GUILayout.BeginVertical();
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii]);
                        GUILayout.EndVertical();
                        GUILayout.Space(10);
                    } else if (rR_DialogueTools_Visualization.visual.animMode == TransitionMode.MovePosition) {
                        GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                        rR_DialogueTools_Visualization.visual.visualDatas[i].startPos[ii] = EditorGUILayout.Vector3Field("Start pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].startPos[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("End pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii]);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        rR_DialogueTools_Visualization.visual.visualDatas[i].startScale[ii] = EditorGUILayout.Vector3Field("Start scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].startScale[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("End scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii]);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.Space(20);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80))) {
                    clipboardActor = rR_DialogueTools_Visualization.visual.visualDatas[i].actorName;
                    clipboardExpression = rR_DialogueTools_Visualization.visual.visualDatas[i].expression;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].actorName = clipboardActor;
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression = clipboardExpression;
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80))) {
                    Debug.Log("isCopyPressed");
                    clipboardPos = rR_DialogueTools_Visualization.visual.visualDatas[i].endPos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80))) {
                    Debug.Log("isPastePressed");
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endPos = clipboardPos;
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80))) {
                    clipboardScale = rR_DialogueTools_Visualization.visual.visualDatas[i].endScale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endScale = clipboardScale;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }

    private void CheckActor() {
        for (int i = 0; i < rR_DialogueTools_Visualization.visual.visualDatas.Count; i++) {
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Add("");
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Add("");
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Add(new Vector3());
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.Add(new Vector3(1, 1, 1));
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].endScale.Count - 1);
                }
            }

            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.Add(new Vector3());
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].startPos.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.Add(new Vector3(1, 1, 1));
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].startScale.Count - 1);
                }
            }
        }
    }
    // private void Saving() {
    //     string data = JsonUtility.ToJson(rR_DialogueTools_Visualization.visual);
    //     EditorUtility.SetDirty(textAsset);
    //     rR_DialogueTools_FileManagerWindow.init_Window(FileMode.Save);
    //     GUIUtility.ExitGUI();
    //     ready = true;
    // }

    private void OnOpen(RR_Narration selected_RR_Narration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        this.rR_Narration = selected_RR_Narration;
        this.rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
        Debug.Log(selected_RR_DialogueTools_Visualization.visual.visualDatas[0].expression[0]);
        Debug.Log(rR_DialogueTools_Visualization.visual.visualDatas[0].expression[0]);
    }
}