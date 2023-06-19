using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RR_DialogueTools_VisualConverter : EditorWindow
{
    private RR_DialogueTools_Visual dialogueTools_Visual;
    private RR_DialogueTools_VisualData tempVisualData;
    private RR_VisualConverterMode visualConverterMode;
    private TextAsset dialogueToolsVisual;
    private TextAsset dialogueToolsVisualStat;
    private TextAsset dialogueToolsVisualData;
    private string[] visualDataTemps;
    private string[] visualDataTemp;
    private string[] visualStatTemp;
    private string[] actorDataTemps;
    private string[] actorDataTemp;
    private string outputStatFile;
    private string outputTSVFile;
    private string outputPath;
    private string outputName;
    private const string PATH = "Assets/";
    private const string TAB = "\t";

    [MenuItem("Window/RR/Visual Converter")]
    public static void init() {
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_VisualConverter thisWindow = (RR_DialogueTools_VisualConverter)EditorWindow.GetWindow(typeof(RR_DialogueTools_VisualConverter));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 400);
        thisWindow.Show();
    }
    private void OnEnable() {
        outputPath = PATH;
        dialogueTools_Visual = new RR_DialogueTools_Visual();
    }

    void OnGUI() {
        visualConverterMode = (RR_VisualConverterMode)EditorGUILayout.EnumPopup(visualConverterMode);
        outputPath = EditorGUILayout.TextField("Path", outputPath);
        outputName = EditorGUILayout.TextField("FileName", outputName);
        switch (visualConverterMode) {
            case RR_VisualConverterMode.VisualToTSVAndText:
                VisualToTSVAndText();
                break;
            case RR_VisualConverterMode.TSVAndTextToVisual:
                TSVAndTextToVisual();
                break;
        }
    }

    private void VisualToTSVAndText() {
        dialogueToolsVisual = (TextAsset)EditorGUILayout.ObjectField("Visual Json", dialogueToolsVisual, typeof(TextAsset), false);
        if (dialogueToolsVisual != null) {
            GUILayout.Label("Value: " + dialogueToolsVisual.text);
        }
        if (GUILayout.Button("Convert")) {
            if (!outputPath.EndsWith("/") && outputPath.Length > 0) {
                outputPath += "/";
            }
            if (!System.String.IsNullOrEmpty(dialogueToolsVisual.text)) {
                JsonUtility.FromJsonOverwrite(dialogueToolsVisual.text, dialogueTools_Visual);
                outputStatFile = $"{dialogueTools_Visual.actorCount};{(int)dialogueTools_Visual.animMode}";
                outputTSVFile = "";
                for (int i = 0; i < dialogueTools_Visual.visualDatas.Count; i++) {
                    if (i > 0) {
                        outputTSVFile += System.Environment.NewLine;
                    }
                    outputTSVFile += dialogueTools_Visual.visualDatas[i].tags + TAB;
                    outputTSVFile += dialogueTools_Visual.visualDatas[i].index + TAB;
                    outputTSVFile += dialogueTools_Visual.visualDatas[i].transitionDuration + TAB;
                    outputTSVFile += "|||" + TAB;
                    for (int ii = 0; ii < dialogueTools_Visual.actorCount; ii++) {
                        if (ii > 0) {
                            outputTSVFile += TAB + "||" + TAB;
                        }
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].actorName[ii] + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].expression[ii] + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].isLooping[ii] + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].useSilhouette[ii] + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startPos[ii].x + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startPos[ii].y + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startPos[ii].z + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endPos[ii].x + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endPos[ii].y + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endPos[ii].z + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startScale[ii].x + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startScale[ii].y + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].startScale[ii].z + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endScale[ii].x + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endScale[ii].y + TAB;
                        outputTSVFile += dialogueTools_Visual.visualDatas[i].endScale[ii].z;
                    }
                }
                RR_DialogueTools_Functions.SaveFile(outputPath + outputName + "_stat.txt", outputStatFile);
                RR_DialogueTools_Functions.SaveFile(outputPath + outputName + "_visualDatas.tsv", outputTSVFile);
                RR_DialogueTools_Functions.SaveFile(outputPath + outputName + "_visualDatas.txt", outputTSVFile);
            }
        }
    }

    private void TSVAndTextToVisual() {
        dialogueToolsVisualStat = (TextAsset)EditorGUILayout.ObjectField("Visual Stat TextAsset", dialogueToolsVisualStat, typeof(TextAsset), false);
        dialogueToolsVisualData = (TextAsset)EditorGUILayout.ObjectField("VisualDatas TextAsset", dialogueToolsVisualData, typeof(TextAsset), false);
        if (GUILayout.Button("Convert")) {
            Debug.Log("in 1");
            dialogueTools_Visual = new RR_DialogueTools_Visual();
            dialogueTools_Visual.actorCount = int.Parse(dialogueToolsVisualStat.text.Split(';')[0]);
            dialogueTools_Visual.animMode = (RR_DialogueTools_TransitionMode)int.Parse(dialogueToolsVisualStat.text.Split(';')[1]);
            visualDataTemps = dialogueToolsVisualData.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);
            dialogueTools_Visual.visualDatas = new List<RR_DialogueTools_VisualData>(new RR_DialogueTools_VisualData[visualDataTemps.Length]);
            Debug.Log(visualDataTemps.Length);
            Debug.Log(dialogueTools_Visual.visualDatas.Count);
            for (int i = 0; i < visualDataTemps.Length; i++) {
                Debug.Log("in 3");
                visualDataTemp = visualDataTemps[i].Split(new string[] { "|||" }, System.StringSplitOptions.None);
                visualStatTemp = visualDataTemp[0].Split(new string[] { "\t" }, System.StringSplitOptions.None);
                actorDataTemps = visualDataTemp[1].Split(new string[] { "||" }, System.StringSplitOptions.None);
                dialogueTools_Visual.visualDatas[i] = new RR_DialogueTools_VisualData();
                dialogueTools_Visual.visualDatas[i].tags = visualStatTemp[0];
                dialogueTools_Visual.visualDatas[i].index = int.Parse(visualStatTemp[1]);
                dialogueTools_Visual.visualDatas[i].transitionDuration = float.Parse(visualStatTemp[2]);
                dialogueTools_Visual.visualDatas[i].actorName = new List<string>(new string[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].expression = new List<string>(new string[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].isLooping = new List<bool>(new bool[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].useSilhouette = new List<bool>(new bool[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].startPos = new List<Vector3>(new Vector3[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].endPos = new List<Vector3>(new Vector3[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].startScale = new List<Vector3>(new Vector3[actorDataTemps.Length]);
                dialogueTools_Visual.visualDatas[i].endScale = new List<Vector3>(new Vector3[actorDataTemps.Length]);
                Debug.Log("in 4");
                for (int ii = 0; ii < actorDataTemps.Length; ii++) {
                    Debug.Log("in 5");
                    actorDataTemp = actorDataTemps[ii].Split(new string[] { "\t" }, System.StringSplitOptions.None);
                    dialogueTools_Visual.visualDatas[i].actorName[ii] = actorDataTemp[1];
                    dialogueTools_Visual.visualDatas[i].expression[ii] = actorDataTemp[2];
                    dialogueTools_Visual.visualDatas[i].isLooping[ii] = bool.Parse(actorDataTemp[3]);
                    dialogueTools_Visual.visualDatas[i].useSilhouette[ii] = bool.Parse(actorDataTemp[4]);
                    dialogueTools_Visual.visualDatas[i].startPos[ii] = new Vector3(
                        x: float.Parse(actorDataTemp[5]),
                        y: float.Parse(actorDataTemp[6]),
                        z: float.Parse(actorDataTemp[7])
                    );
                    dialogueTools_Visual.visualDatas[i].endPos[ii] = new Vector3(
                        x: float.Parse(actorDataTemp[8]),
                        y: float.Parse(actorDataTemp[9]),
                        z: float.Parse(actorDataTemp[10])
                    );
                    dialogueTools_Visual.visualDatas[i].startScale[ii] = new Vector3(
                        x: float.Parse(actorDataTemp[11]),
                        y: float.Parse(actorDataTemp[12]),
                        z: float.Parse(actorDataTemp[13])
                    );
                    dialogueTools_Visual.visualDatas[i].endScale[ii] = new Vector3(
                        x: float.Parse(actorDataTemp[14]),
                        y: float.Parse(actorDataTemp[15]),
                        z: float.Parse(actorDataTemp[16])
                    );
                }
            }
            string visualJson = JsonUtility.ToJson(dialogueTools_Visual);
            RR_DialogueTools_Functions.SaveFile(outputPath + outputName + ".json", visualJson);
        }
    }
}
