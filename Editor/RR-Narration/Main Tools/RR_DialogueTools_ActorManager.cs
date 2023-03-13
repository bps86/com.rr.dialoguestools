using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_ActorManager : EditorWindow
{
    public Action<RR_Narration, RR_Narration_Visualization> SetRRVarEvent;
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
    RR_Narration rR_Narration;
    RR_Narration_Visualization rR_Narration_Visualization;


    // [MenuItem("Window/RR/ActorManager")]

    public void init(RR_DialogueTools_ActorManager.Mode selectedMode, int currentIndex, int secondIndex = 0) {
        rR_Narration = new RR_Narration();
        rR_Narration_Visualization = new RR_Narration_Visualization();
        dynamicIndex = currentIndex;
        optionalIndex = secondIndex;
        mode = selectedMode;
        RR_EditorTools.Refresh_RR_DialogueTools();
        tempName = RR_EditorTools.names[index];
        tempExpression = GetExpression(RR_EditorTools.expression[RR_EditorTools.names[index]], new List<string>());
    }

    public void init_Window() {
        position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
        Show();
    }

    public void SetRRVar(RR_Narration selected_RR_Naration, RR_Narration_Visualization selected_RR_Narration_Visualization) {
        rR_Narration = selected_RR_Naration;
        rR_Narration_Visualization = selected_RR_Narration_Visualization;
    }

    void OnEnable() {
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }
    void OnGUI() {
        GUILayout.BeginHorizontal();
        GetActors();
        EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawActorData();
        GUILayout.EndHorizontal();
        Repaint();
    }
    void GetActors() {
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        GUILayout.Label("Actors");
        for (int i = 0; i < RR_EditorTools.names.Length; i++) {
            if (GUILayout.Button(RR_EditorTools.names[i])) {
                index = i;
                tempName = RR_EditorTools.names[i];
                tempExpression = GetExpression(RR_EditorTools.expression[RR_EditorTools.names[i]], new List<string>());
            }
        }
        GUILayout.EndScrollView();
    }
    void DrawActorData() {
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        GUILayout.Space(10);
        GUILayout.Label(tempName);
        for (int i = 0; i < tempExpression.Count; i++)
            if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + tempName + "," + tempExpression[i] + ".png"), GUILayout.Width(100), GUILayout.Height(100))) {
                if (mode == RR_DialogueTools_ActorManager.Mode.Dialogue) {
                    Debug.Log(rR_Narration.dialogues[dynamicIndex].name);
                    rR_Narration.dialogues[dynamicIndex].name = tempName;
                    rR_Narration.dialogues[dynamicIndex].expression = tempExpression[i];
                }
                if (mode == RR_DialogueTools_ActorManager.Mode.Visual) {
                    rR_Narration_Visualization.visual.visualDatas[dynamicIndex].actor[optionalIndex] = tempName + ";;" + tempExpression[i];
                }
                SetRRVarEvent(rR_Narration, rR_Narration_Visualization);
                Close();
            }
        GUILayout.EndScrollView();
    }
    static List<string> GetExpression(string[] expressions, List<string> expressions2) {
        for (int i = 0; i < expressions.Length; i++) {
            expressions2.Add(expressions[i]);
        }
        return expressions2;
    }
}