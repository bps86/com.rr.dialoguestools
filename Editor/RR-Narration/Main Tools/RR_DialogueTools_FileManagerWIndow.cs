using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_FileManagerWindow : EditorWindow
{
    public Action Save;
    public Action<RR_Narration, RR_Narration_Visualization> Open;

    private FileMode fileMode;
    private RR_Narration rR_Narration;
    private RR_Narration_Visualization rR_Narration_Visualization;

    public void init() {
        rR_Narration = new RR_Narration();
        rR_Narration_Visualization = new RR_Narration_Visualization();
    }

    public void init_Window(FileMode selectedFileMode) {
        fileMode = selectedFileMode;
        position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        Show();
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        RR_EditorTools.fileName = EditorGUILayout.TextField(RR_EditorTools.fileName);
        RR_EditorTools.currentLocaleIndex = EditorGUILayout.Popup(RR_EditorTools.currentLocaleIndex, RR_EditorTools.locales);
        RR_EditorTools.currentLocale = RR_EditorTools.locales[RR_EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK")) {
            switch (fileMode) {
                case FileMode.New:
                    NewFile();
                    break;
                case FileMode.Save:
                    SaveFile();
                    break;
                case FileMode.Open:
                    OpenFile();
                    Close();
                    break;
            }
        }
        GUILayout.EndVertical();
    }

    public void NewFile() {
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale) && RR_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale);
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1)
            RR_NarrationFunctions.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale + "/" + RR_EditorTools.fileName + ".txt");
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1)
            RR_NarrationFunctions.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.fileName + ".txt");
        RR_EditorTools.CreateNewVisualAsset();
        RR_EditorTools.ToStringTable(RR_EditorTools.fileName, "", RR_EditorTools.rrDialoguesTable);
        OpenFile();
        Close();
    }

    public void SaveFile() {
        Debug.Log("Saving");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale) && RR_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale);
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1)
            RR_NarrationFunctions.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale + "/" + RR_EditorTools.fileName + ".txt", RR_EditorTools.fileData);
        Debug.Log("Checked Save on Locale");
        RR_EditorTools.ToStringTable(RR_EditorTools.fileName, RR_EditorTools.fileData, RR_EditorTools.rrDialoguesTable);
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1)
            RR_NarrationFunctions.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.fileName + ".txt", RR_EditorTools.fileData);
        string visualJson = JsonUtility.ToJson(rR_Narration_Visualization.visual);
        RR_NarrationFunctions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + RR_EditorTools.fileName + ".json", visualJson);
        Debug.Log("Checked Save on Non Locale");
        OpenFile();
        Debug.Log("FileRefreshed");
        Close();
    }

    public void OpenFile() {
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1) {
            StringTable stringTable = AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + RR_EditorTools.currentLocale + ".asset");
            StringTableEntry stringTableEntry = RR_EditorTools.GetStringTableEntry(stringTable, RR_EditorTools.fileName);
            string[] dialogueList = stringTableEntry.Value.Split(new string[] { "||" }, System.StringSplitOptions.None);
            rR_Narration.dialogues = RR_NarrationFunctions.GetDialogues(dialogueList);
        }
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1)
            rR_Narration.dialogues = RR_NarrationFunctions.OpenDialogueFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.fileName + ".txt");
        if (!File.Exists("Assets/RR-Narration/Resources/RR-Visual/" + RR_EditorTools.fileName + ".json"))
            RR_EditorTools.CreateNewVisualAsset();
        JsonUtility.FromJsonOverwrite(RR_NarrationFunctions.OpenFile("Assets/RR-Narration/Resources/RR-Visual/" + RR_EditorTools.fileName + ".json"), rR_Narration_Visualization.visual);
        Open(rR_Narration, rR_Narration_Visualization);
        RR_EditorTools.GetDialogueIndex(rR_Narration);
        RR_EditorTools.Refresh_RR_DialogueTools();
        RR_DialogueTools_MainEditor.ready = true;
    }
}