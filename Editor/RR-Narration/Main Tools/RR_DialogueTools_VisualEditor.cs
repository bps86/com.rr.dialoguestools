using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RR_DialogueTools_VisualEditor : EditorWindow
{
    private RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    private RR_Narration rR_Narration;
    private TextAsset config;
    private Object textAsset;
    private List<string> clipboardActor;
    private List<string> clipboardExpression;
    private List<Vector3> clipboardStartPos;
    private List<Vector3> clipboardStartScale;
    private List<Vector3> clipboardEndPos;
    private List<Vector3> clipboardEndScale;
    private Rect cursorChangeRect;
    private Vector2 scrollPos;
    private Vector2 scrollPos2;
    private bool sameTransitionDuration;
    private bool loopAll;
    private bool resize = false;
    private float allDurationTransition;
    private float currentScrollViewWidth;

    [MenuItem("Window/RR/Visual Editor")]
    public static void init() {
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_VisualEditor thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
        thisWindow.clipboardStartPos = new List<Vector3>();
        thisWindow.clipboardStartScale = new List<Vector3>();
        thisWindow.clipboardEndPos = new List<Vector3>();
        thisWindow.clipboardEndScale = new List<Vector3>();
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1080, 600);
        thisWindow.sameTransitionDuration = false;
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
        GUILayout.BeginHorizontal();
        DrawOptions();
        RR_EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
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
            loopAll = false;
            sameTransitionDuration = false;
            config = (TextAsset)textAsset;
            if (!System.String.IsNullOrEmpty(config.text)) {
                JsonUtility.FromJsonOverwrite(config.text, rR_DialogueTools_Visualization.visual);
            } else {
                rR_DialogueTools_Visualization.visual = new RR_DialogueTools_Visual();
            }
        }
        if (rR_DialogueTools_Visualization.visual != null && config != null && GUILayout.Button("Save")) {
            string visualJson = JsonUtility.ToJson(rR_DialogueTools_Visualization.visual);
            RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + textAsset.name + ".json", visualJson);
            RR_EditorTools.Refresh_RR_DialogueTools();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OpenActorManagerWindow(int index, int index2) {
        RR_DialogueTools_ActorManager rR_DialogueTools_ActorManager = (RR_DialogueTools_ActorManager)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_ActorManager));
        rR_DialogueTools_ActorManager.init(RR_ActorManagerMode.Visual, index, index2);
        rR_DialogueTools_ActorManager.SetRRVar(rR_Narration, rR_DialogueTools_Visualization);
        rR_DialogueTools_ActorManager.SetRRVarEvent += OnOpen;
        rR_DialogueTools_ActorManager.init_Window();
        GUIUtility.ExitGUI();
    }

    private void DrawVisualPos() {
        if (rR_DialogueTools_Visualization.visual != null && textAsset != null) {
            CheckActor();
            GUILayout.BeginVertical();
            if (rR_DialogueTools_Visualization.visual != null && config != null) {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Actors: ", GUILayout.Width(60));
                rR_DialogueTools_Visualization.visual.actorCount = EditorGUILayout.IntSlider(rR_DialogueTools_Visualization.visual.actorCount, 1, 4);
                rR_DialogueTools_Visualization.visual.animMode = (RR_DialogueTools_TransitionMode)EditorGUILayout.EnumPopup("Transition Mode", rR_DialogueTools_Visualization.visual.animMode);
                GUILayout.EndHorizontal();
                if (rR_DialogueTools_Visualization.visual.animMode == RR_DialogueTools_TransitionMode.MovePosition) {
                    GUILayout.BeginHorizontal(GUILayout.Width(150));
                    GUILayout.Label("Single Transition Duration: ");
                    sameTransitionDuration = EditorGUILayout.Toggle(sameTransitionDuration);
                    GUILayout.EndHorizontal();
                } else {
                    sameTransitionDuration = false;
                }
                if (sameTransitionDuration) {
                    GUILayout.BeginHorizontal(GUILayout.Width(150));
                    GUILayout.Label("Transition Duration: ");
                    allDurationTransition = EditorGUILayout.FloatField(allDurationTransition);
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal(GUILayout.Width(150));
                GUILayout.Label("Loop All Animation: ");
                loopAll = EditorGUILayout.Toggle(loopAll);
                GUILayout.EndHorizontal();
                if (!loopAll) {
                    if (GUILayout.Button("Clear Loop", GUILayout.Width(100))) {
                        for (int i = 0; i < rR_DialogueTools_Visualization.visual.visualDatas.Count; i++) {
                            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.visualDatas[i].actorName.Count; ii++) {
                                rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping[ii] = false;
                            }
                        }
                    }
                }
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
                    GUILayout.Label("Expr: " + expression, GUILayout.Width(100));
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + name + "," + expression + ".png"), GUILayout.Width(150), GUILayout.Height(150))) {
                        int index = i;
                        int index2 = ii;
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
                if (rR_DialogueTools_Visualization.visual.animMode == RR_DialogueTools_TransitionMode.MovePosition) {
                    if (!sameTransitionDuration) {
                        GUILayout.BeginHorizontal(GUILayout.Width(150));
                        GUILayout.Label("Transition Duration: ");
                        rR_DialogueTools_Visualization.visual.visualDatas[i].transitionDuration = EditorGUILayout.FloatField(rR_DialogueTools_Visualization.visual.visualDatas[i].transitionDuration);
                        GUILayout.EndHorizontal();
                    } else {
                        rR_DialogueTools_Visualization.visual.visualDatas[i].transitionDuration = allDurationTransition;
                    }
                }
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.visualDatas[i].endPos.Count; ii++) {
                    GUILayout.BeginVertical();
                    if (rR_DialogueTools_Visualization.visual.animMode == RR_DialogueTools_TransitionMode.Static) {
                        GUILayout.BeginVertical();
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii]);
                        GUILayout.EndVertical();
                        GUILayout.Space(10);
                    } else if (rR_DialogueTools_Visualization.visual.animMode == RR_DialogueTools_TransitionMode.MovePosition) {
                        GUILayout.BeginVertical();
                        rR_DialogueTools_Visualization.visual.visualDatas[i].startPos[ii] = EditorGUILayout.Vector3Field("Start pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].startPos[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii] = EditorGUILayout.Vector3Field("End pos " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endPos[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].startScale[ii] = EditorGUILayout.Vector3Field("Start scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].startScale[ii]);
                        rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii] = EditorGUILayout.Vector3Field("End scale " + ii + ": ", rR_DialogueTools_Visualization.visual.visualDatas[i].endScale[ii]);
                        GUILayout.EndVertical();
                        GUILayout.Space(20);
                    }
                    if (loopAll) {
                        rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping[ii] = true;
                    } else {
                        rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping[ii] = EditorGUILayout.Toggle("Loop Animation: ", rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping[ii]);
                    }
                    rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette[ii] = EditorGUILayout.Toggle("Use Silhouette: ", rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette[ii]);
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80))) {
                    clipboardActor = rR_DialogueTools_Visualization.visual.visualDatas[i].actorName;
                    clipboardExpression = rR_DialogueTools_Visualization.visual.visualDatas[i].expression;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].actorName = new List<string>(clipboardActor);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression = new List<string>(clipboardExpression);
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80))) {
                    clipboardStartPos = rR_DialogueTools_Visualization.visual.visualDatas[i].startPos;
                    clipboardEndPos = rR_DialogueTools_Visualization.visual.visualDatas[i].endPos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startPos = new List<Vector3>(clipboardStartPos);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endPos = new List<Vector3>(clipboardEndPos);
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80))) {
                    clipboardStartScale = rR_DialogueTools_Visualization.visual.visualDatas[i].startScale;
                    clipboardEndScale = rR_DialogueTools_Visualization.visual.visualDatas[i].endScale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startScale = new List<Vector3>(clipboardStartScale);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endScale = new List<Vector3>(clipboardEndScale);
                }
                if (GUILayout.Button("Copy All(Actor,Pos,Scale)", GUILayout.Width(160))) {
                    clipboardActor = rR_DialogueTools_Visualization.visual.visualDatas[i].actorName;
                    clipboardExpression = rR_DialogueTools_Visualization.visual.visualDatas[i].expression;
                    clipboardStartPos = rR_DialogueTools_Visualization.visual.visualDatas[i].startPos;
                    clipboardEndPos = rR_DialogueTools_Visualization.visual.visualDatas[i].endPos;
                    clipboardEndScale = rR_DialogueTools_Visualization.visual.visualDatas[i].endScale;
                    clipboardStartScale = rR_DialogueTools_Visualization.visual.visualDatas[i].startScale;
                }
                if (GUILayout.Button("Paste All(Actor,Pos,Scale)", GUILayout.Width(160))) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].actorName = new List<string>(clipboardActor);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression = new List<string>(clipboardExpression);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startPos = new List<Vector3>(clipboardStartPos);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endPos = new List<Vector3>(clipboardEndPos);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].startScale = new List<Vector3>(clipboardStartScale);
                    rR_DialogueTools_Visualization.visual.visualDatas[i].endScale = new List<Vector3>(clipboardEndScale);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
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
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].expression.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].expression.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.Add(false);
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].isLooping.Count - 1);
                }
            }
            for (int ii = 0; ii < rR_DialogueTools_Visualization.visual.actorCount; ii++) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.Count < rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.Add(false);
                }
            }
            for (int ii = rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.Count - 1; ii >= 0; ii--) {
                if (rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.Count > rR_DialogueTools_Visualization.visual.actorCount) {
                    rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.RemoveAt(rR_DialogueTools_Visualization.visual.visualDatas[i].useSilhouette.Count - 1);
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

    private void OnOpen(RR_Narration selected_RR_Narration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        this.rR_Narration = selected_RR_Narration;
        this.rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
    }
}