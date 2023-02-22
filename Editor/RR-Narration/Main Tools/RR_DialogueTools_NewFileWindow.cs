using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Tables;

public class RR_DialogueTools_NewFileWindow : EditorWindow
{
    public static void init()
    {
        RR_DialogueTools_NewFileWindow thisWindow = (RR_DialogueTools_NewFileWindow)EditorWindow.GetWindow(typeof(RR_DialogueTools_NewFileWindow));
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
            NewFile();
        GUILayout.EndVertical();
    }

    void NewFile()
    {
        // if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale) && RR_DialogueTools_EditorTools.currentLocale != "<None>") Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale);
        // if (RR_DialogueTools_EditorTools.currentLocaleIndex < RR_DialogueTools_EditorTools.locales.Length - 1)
        //     Manager.NewFile("RR-Narration/Resources/RR-Dialogues/" + RR_DialogueTools_EditorTools.currentLocale + "/" + RR_DialogueTools_EditorTools.fileName + ".txt");
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1)
            RR_NarrationManager.NewFile("RR-Narration/Resources/RR-Dialogues/" + EditorTools.fileName + ".txt");
        EditorTools.CreateNewVisualAsset();
        EditorTools.ToStringTable(EditorTools.fileName, "", EditorTools.rrDialoguesTable);
        RR_DialogueTools_OpenFileWindow.OpenFile();
        Close();
    }
}