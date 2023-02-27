using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

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
                    RR_Narration.dialogues[dynamicIndex].name = tempName;
                    RR_Narration.dialogues[dynamicIndex].expression = tempExpression[i];
                }
                if (mode == RR_DialogueTools_ActorManager.Mode.Visual)
                {
                    RR_NarrationVisualization.visual.visualDatas[dynamicIndex].actor[optionalIndex] = tempName + ";;" + tempExpression[i];
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