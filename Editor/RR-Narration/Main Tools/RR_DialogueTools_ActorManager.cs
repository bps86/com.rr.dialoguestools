using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class RR_DialogueTools_ActorManager : EditorWindow
{
    public Action<RR_Narration, RR_DialogueTools_Visualization> SetRRVarEvent;

    private RR_Narration rR_Narration;
    private RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    private RR_DialogueTools_RectData tempActorRectData;
    private RR_ActorManagerMode mode;
    private List<string> tempExpression;
    private Vector2 scrollPos = Vector2.zero;
    private Vector2 scrollPos2 = Vector2.zero;
    private Rect cursorChangeRect;
    private string tempName;
    private string tempRectDataString;
    private string[] tempRectData;
    private float currentScrollViewWidth;
    private int index;
    private int dynamicIndex;
    private int optionalIndex;
    private bool resize = false;

    private const int MAX_ROW = 4;

    [MenuItem("Window/RR/Actor Manager")]
    static void initPreview() {
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_ActorManager thisWindow = (RR_DialogueTools_ActorManager)EditorWindow.GetWindow(typeof(RR_DialogueTools_ActorManager));
        thisWindow.init(RR_ActorManagerMode.Preview, 0);
        thisWindow.init_Window();
    }

    public void init(RR_ActorManagerMode selectedMode, int currentIndex, int secondIndex = 0) {
        rR_Narration = new RR_Narration();
        rR_DialogueTools_Visualization = new RR_DialogueTools_Visualization();
        index = 0;
        dynamicIndex = currentIndex;
        optionalIndex = secondIndex;
        mode = selectedMode;
        RR_EditorTools.Refresh_RR_DialogueTools();
        tempName = RR_EditorTools.names[index];
        tempExpression = GetExpression(RR_EditorTools.expression[RR_EditorTools.names[index]], new List<string>());
        tempRectData = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/RR-Narration/Resources/RR-Actors-RectData/" + tempName + ".txt").text.Split(';');
        tempActorRectData = new RR_DialogueTools_RectData(tempRectData);
    }

    public void init_Window() {
        position = new Rect(Screen.width / 2, Screen.height / 2, 800, 480);
        Show();
    }

    public void SetRRVar(RR_Narration selected_RR_Naration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        rR_Narration = selected_RR_Naration;
        rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
    }

    void OnEnable() {
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }
    void OnGUI() {
        GUILayout.BeginHorizontal();
        GetActors();
        RR_EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawActorData();
        GUILayout.EndHorizontal();
        Repaint();
    }
    void GetActors() {
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Actors");
        for (int i = 0; i < RR_EditorTools.names.Length; i++) {
            if (GUILayout.Button(RR_EditorTools.names[i], GUILayout.Height(25))) {
                index = i;
                tempName = RR_EditorTools.names[i];
                tempExpression = GetExpression(RR_EditorTools.expression[RR_EditorTools.names[i]], new List<string>());
                tempRectData = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/RR-Narration/Resources/RR-Actors-RectData/" + tempName + ".txt").text.Split(';');
                tempActorRectData = new RR_DialogueTools_RectData(tempRectData);
            }
        }
        GUILayout.EndScrollView();
    }
    void DrawActorData() {
        GUILayout.BeginVertical();
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        GUILayout.Space(10);
        GUILayout.Label(tempName);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < tempExpression.Count; i++) {
            if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + tempName + "," + tempExpression[i] + ".png"), GUILayout.Width(150), GUILayout.Height(150))) {
                if (mode != RR_ActorManagerMode.Preview) {
                    if (mode == RR_ActorManagerMode.Dialogue) {
                        rR_Narration.dialogues[dynamicIndex].actorName = tempName;
                        rR_Narration.dialogues[dynamicIndex].expression = tempExpression[i];
                    }
                    if (mode == RR_ActorManagerMode.Visual) {
                        rR_DialogueTools_Visualization.visual.visualDatas[dynamicIndex].actorName[optionalIndex] = tempName;
                        rR_DialogueTools_Visualization.visual.visualDatas[dynamicIndex].expression[optionalIndex] = tempExpression[i];
                    }
                    SetRRVarEvent(rR_Narration, rR_DialogueTools_Visualization);
                    Close();
                }
            }
            if ((i + 1f) / MAX_ROW == Math.Round((i + 1f) / MAX_ROW)) {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        if (tempActorRectData != null) {
            tempActorRectData.ActorPivot = EditorGUILayout.Vector3Field("Actor Pivot: ", tempActorRectData.ActorPivot);
            tempActorRectData.ActorScale = EditorGUILayout.Vector3Field("Actor Scale: ", tempActorRectData.ActorScale);
            if (GUILayout.Button("Save Rect Data")) {
                tempRectDataString = tempActorRectData.ActorPivot.x.ToString();
                tempRectDataString += ";" + tempActorRectData.ActorPivot.y.ToString();
                tempRectDataString += ";" + tempActorRectData.ActorPivot.z.ToString();
                tempRectDataString += ";" + tempActorRectData.ActorScale.x.ToString();
                tempRectDataString += ";" + tempActorRectData.ActorScale.y.ToString();
                tempRectDataString += ";" + tempActorRectData.ActorScale.z.ToString();
                RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/RR-Actors-RectData/" + tempName + ".txt", tempRectDataString);
                RR_EditorTools.Refresh_RR_DialogueTools();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    static List<string> GetExpression(string[] expressions, List<string> expressions2) {
        for (int i = 0; i < expressions.Length; i++) {
            expressions2.Add(expressions[i]);
        }
        return expressions2;
    }
}