using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_SaveFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_SaveFileWindow thisWindow = (RR_DialogueTools_SaveFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_SaveFileWindow));
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
            SaveFile();
        GUILayout.EndVertical();
    }
    void SaveFile()
    {
        Debug.Log("Saving");
        // if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale) && RR_DialogueTools_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale);
        // if (RR_DialogueTools_EditorTools.currentLocaleIndex < RR_DialogueTools_EditorTools.locales.Length - 1)
        //     Manager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale + "/" + RR_DialogueTools_EditorTools.fileName + ".txt", RR_DialogueTools_EditorTools.fileData);
        // Debug.Log("Checked Save on Locale");
        EditorTools.ToStringTable(EditorTools.fileName, EditorTools.fileData, EditorTools.rrDialoguesTable);
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            RR_NarrationManager.SaveFile("Assets/RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt", EditorTools.fileData);
        string visualJson = JsonUtility.ToJson(RR_NarrationVisualization.visual);
        RR_NarrationManager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json", visualJson);
        Debug.Log("Checked Save on Non Locale");
        RR_DialogueTools_OpenFileWindow.OpenFile();
        Debug.Log("FileRefreshed");
        Close();
    }
}