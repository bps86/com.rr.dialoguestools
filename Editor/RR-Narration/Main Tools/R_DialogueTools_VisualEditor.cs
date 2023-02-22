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
        if (textAsset == null) RR_NarrationVisualization.visual = new RR_NarrationVisual();
        thisWindow = (RR_DialogueTools_VisualEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualEditor));
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
            if (!System.String.IsNullOrEmpty(config.text)) JsonUtility.FromJsonOverwrite(config.text, RR_NarrationVisualization.visual);
            else RR_NarrationVisualization.visual = new RR_NarrationVisual();
        }
        if (textAsset != null && GUILayout.Button("Save"))
        {
            string visualJson = JsonUtility.ToJson(RR_NarrationVisualization.visual);
            Debug.Log(visualJson);
            RR_NarrationManager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + textAsset.name + ".json", visualJson);
            EditorTools.Refresh_RR_DialogueTools();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawVisualPos()
    {
        if (RR_NarrationVisualization.visual != null && textAsset != null)
        {
            CheckActor();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Actors: ", GUILayout.Width(60));
            RR_NarrationVisualization.visual.actorCount = EditorGUILayout.IntField(RR_NarrationVisualization.visual.actorCount, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
            for (int i = 0; i < RR_NarrationVisualization.visual.visualDatas.Count; i++)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Tags:" + RR_NarrationVisualization.visual.visualDatas[i].tags);
                GUILayout.Label("Index:" + RR_NarrationVisualization.visual.visualDatas[i].index);
                GUILayout.BeginHorizontal();
                for (int ii = 0; ii < RR_NarrationVisualization.visual.visualDatas[i].actor.Count; ii++)
                {
                    string name = RR_NarrationVisualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[0];
                    string expression = "";
                    if (RR_NarrationVisualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None).Length > 1)
                        expression = RR_NarrationVisualization.visual.visualDatas[i].actor[ii].Split(new string[] { ";;" }, System.StringSplitOptions.None)[1];
                    GUILayout.BeginVertical();
                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + name + "," + expression + ".png"), GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        int index = i;
                        int index2 = ii;
                        RR_DialogueTools_ActorManager.init(RR_DialogueTools_ActorManager.Mode.Visual, index, index2);
                        GUIUtility.ExitGUI();
                    }
                    if (GUILayout.Button("Reset", GUILayout.Width(100)))
                        RR_NarrationVisualization.visual.visualDatas[i].actor[ii] = "";
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                for (int ii = 0; ii < RR_NarrationVisualization.visual.visualDatas[i].pos.Count; ii++)
                {
                    GUILayout.BeginHorizontal();
                    RR_NarrationVisualization.visual.visualDatas[i].pos[ii] = EditorGUILayout.Vector3Field("pos" + ii + ": ", RR_NarrationVisualization.visual.visualDatas[i].pos[ii], GUILayout.Width(400));
                    RR_NarrationVisualization.visual.visualDatas[i].scale[ii] = EditorGUILayout.Vector3Field("scale" + ii + ": ", RR_NarrationVisualization.visual.visualDatas[i].scale[ii], GUILayout.Width(400));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Actor", GUILayout.Width(80)))
                {
                    clipboardActor = RR_NarrationVisualization.visual.visualDatas[i].actor;
                }
                if (GUILayout.Button("Paste Actor", GUILayout.Width(80)))
                {
                    RR_NarrationVisualization.visual.visualDatas[i].actor = clipboardActor;
                }
                if (GUILayout.Button("Copy Pos", GUILayout.Width(80)))
                {
                    clipboardPos = RR_NarrationVisualization.visual.visualDatas[i].pos;
                }
                if (GUILayout.Button("Paste Pos", GUILayout.Width(80)))
                {
                    RR_NarrationVisualization.visual.visualDatas[i].pos = clipboardPos;
                }
                if (GUILayout.Button("Copy Scale", GUILayout.Width(80)))
                {
                    clipboardScale = RR_NarrationVisualization.visual.visualDatas[i].scale;
                }
                if (GUILayout.Button("Paste Scale", GUILayout.Width(80)))
                {
                    RR_NarrationVisualization.visual.visualDatas[i].scale = clipboardScale;
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
        for (int i = 0; i < RR_NarrationVisualization.visual.visualDatas.Count; i++)
        {
            for (int ii = 0; ii < RR_NarrationVisualization.visual.actorCount; ii++)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].actor.Count < RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].actor.Add("");
            }
            for (int ii = RR_NarrationVisualization.visual.visualDatas[i].actor.Count - 1; ii >= 0; ii--)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].actor.Count > RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].actor.RemoveAt(ii);
            }
            for (int ii = 0; ii < RR_NarrationVisualization.visual.actorCount; ii++)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].pos.Count < RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].pos.Add(new Vector3());
            }
            for (int ii = RR_NarrationVisualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].pos.Count > RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].pos.RemoveAt(ii);
            }
            for (int ii = 0; ii < RR_NarrationVisualization.visual.actorCount; ii++)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].scale.Count < RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].scale.Add(new Vector3(1, 1, 1));
            }
            for (int ii = RR_NarrationVisualization.visual.visualDatas[i].pos.Count - 1; ii >= 0; ii--)
            {
                if (RR_NarrationVisualization.visual.visualDatas[i].scale.Count > RR_NarrationVisualization.visual.actorCount) RR_NarrationVisualization.visual.visualDatas[i].scale.RemoveAt(ii);
            }
        }
    }
    private void Saving()
    {
        string data = JsonUtility.ToJson(RR_NarrationVisualization.visual);
        // EditorUtility.SetDirty(textAsset);
        // RR_DialogueTools_SaveFileWindow.init();
        // GUIUtility.ExitGUI();
        ready = true;
    }
}