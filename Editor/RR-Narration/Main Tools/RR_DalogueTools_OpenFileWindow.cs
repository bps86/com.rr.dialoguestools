using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_OpenFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_OpenFileWindow thisWindow = (RR_DialogueTools_OpenFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_OpenFileWindow));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 100, 100);
        thisWindow.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("File Name");
        EditorTools.fileName = EditorGUILayout.TextField(EditorTools.fileName);
        EditorTools.currentLocaleIndex = EditorGUILayout.Popup(EditorTools.currentLocaleIndex, EditorTools.locales);
        EditorTools.currentLocale = EditorTools.locales[EditorTools.currentLocaleIndex];
        if (GUILayout.Button("OK"))
        {
            OpenFile();
            Close();
        }
        GUILayout.EndVertical();
    }
    public static void OpenFile()
    {
        if (EditorTools.currentLocaleIndex < EditorTools.locales.Length - 1)
        {
            StringTable stringTable = AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + EditorTools.currentLocale + ".asset");
            StringTableEntry stringTableEntry = EditorTools.GetStringTableEntry(stringTable, EditorTools.fileName);
            string[] dialogueList = stringTableEntry.Value.Split(new string[] { "||" }, System.StringSplitOptions.None);
            RR_Narration.dialogues = RR_NarrationManager.GetDialogues(dialogueList);
        }
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            RR_Narration.dialogues = RR_NarrationManager.OpenDialogueFile("Assets/RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt");
        if (!File.Exists("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json"))
            EditorTools.CreateNewVisualAsset();
        JsonUtility.FromJsonOverwrite(RR_NarrationManager.OpenFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json"), RR_NarrationVisualization.visual);
        EditorTools.GetDialogueIndex();
        EditorTools.Refresh_RR_DialogueTools();
        RR_DialogueTools_MainEditor.ready = true;

    }
}