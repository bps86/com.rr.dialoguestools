using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_FileManagerWindow : EditorWindow
{
    public Action<RR_Narration, RR_DialogueTools_Visualization> OpenRREvent;
    public Action<string, string, RR_Narration, RR_DialogueTools_Visualization> OpenEvent;
    public Action CloseEvent;

    private RR_DialogueTools_FileMode fileMode;
    private RR_Narration rR_Narration;
    private RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    private string fileName;
    private string fileData;

    public void SetData(string fileName, string fileData) {
        this.fileName = fileName;
        this.fileData = fileData;
    }

    public void init(RR_Narration selected_RR_Narration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        rR_Narration = selected_RR_Narration;
        rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
    }

    public void init_Window(RR_DialogueTools_FileMode selectedFileMode) {
        fileMode = selectedFileMode;
        position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        Show();

    }

    void OnGUI() {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        fileName = EditorGUILayout.TextField(fileName);
        RR_EditorTools.currentLocaleIndex = EditorGUILayout.Popup(RR_EditorTools.currentLocaleIndex, RR_EditorTools.locales);
        RR_EditorTools.currentLocale = RR_EditorTools.locales[RR_EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK")) {
            switch (fileMode) {
                case RR_DialogueTools_FileMode.New:
                    NewFile();
                    break;
                case RR_DialogueTools_FileMode.Save:
                    SaveFile();
                    break;
                case RR_DialogueTools_FileMode.Open:
                    OpenFile();
                    Close();
                    break;
            }
        }
        GUILayout.EndVertical();
    }

    public void NewFile() {
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale) && RR_EditorTools.currentLocale != "<None>") {
            Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale);
        }
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1) {
            RR_DialogueTools_Functions.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale + "/" + fileName + ".txt");
        }
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1) {
            RR_DialogueTools_Functions.NewFile("RR-Narration/Resources/RR-Dialogues/" + fileName + ".txt");
        }
        RR_EditorTools.CreateNewVisualAsset(fileName);
        RR_EditorTools.ToStringTable(fileName, "", RR_EditorTools.rrDialoguesTable);
        OpenFile();
        Close();
    }

    public void SaveFile() {
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale) && RR_EditorTools.currentLocale != "<None>") {
            Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale);
        }
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1) {
            RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_EditorTools.currentLocale + "/" + fileName + ".txt", fileData);
        }
        RR_EditorTools.ToStringTable(fileName, fileData, RR_EditorTools.rrDialoguesTable);
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1) {
            RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + fileName + ".txt", fileData);
        }
        string visualJson = JsonUtility.ToJson(rR_DialogueTools_Visualization.visual);
        RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + fileName + ".json", visualJson);
        OpenFile();
        Close();
    }

    public void OpenFile() {
        if (RR_EditorTools.currentLocaleIndex < RR_EditorTools.locales.Length - 1) {
            StringTable stringTable = AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + RR_EditorTools.currentLocale + ".asset");
            StringTableEntry stringTableEntry = RR_EditorTools.GetStringTableEntry(stringTable, fileName);
            string[] dialogueList = stringTableEntry.Value.Split(new string[] { "||" }, System.StringSplitOptions.None);
            rR_Narration.dialogues = RR_DialogueTools_Functions.GetDialogues(dialogueList);
        }
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1) {
            rR_Narration.dialogues = RR_DialogueTools_Functions.OpenDialogueFile("Assets/RR-Narration/Resources/RR-Dialogues/" + fileName + ".txt");
        }
        if (!File.Exists("Assets/RR-Narration/Resources/RR-Visual/" + fileName + ".json")) {
            RR_EditorTools.CreateNewVisualAsset(fileName);
        }
        JsonUtility.FromJsonOverwrite(RR_DialogueTools_Functions.OpenFile("Assets/RR-Narration/Resources/RR-Visual/" + fileName + ".json"), rR_DialogueTools_Visualization.visual);
        OpenEvent(fileName, fileData, rR_Narration, rR_DialogueTools_Visualization);
        RR_EditorTools.GetDialogueIndex(rR_Narration);
        RR_EditorTools.Refresh_RR_DialogueTools();
    }

    private void OnDestroy() {
        CloseEvent();
        RR_EditorTools.Refresh_RR_DialogueTools();
    }
}