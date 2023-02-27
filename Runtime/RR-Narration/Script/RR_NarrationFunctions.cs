using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using Spine.Unity;

public class RR_NarrationFunctions
{
    public static void NewFile(string fileName) {
        string newFile = null;
        using StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + fileName);
        writer.Write(newFile);
        writer.Close();
    }
    public static string OpenFile(string path) {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        return json;
    }
    public static List<RR_Dialogue> OpenDialogueFile(string path) {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        string[] dialoguesList = json.Split(new string[] { "||" }, System.StringSplitOptions.None);
        return GetDialogues(dialoguesList);
    }
    public static void SaveFile(string path, string fileData) {
        using StreamWriter write = new StreamWriter(path);
        write.Write(fileData);
        write.Close();
    }

    public static List<RR_Dialogue> GetDialogues(string[] dialoguesList) {
        string[] dialoguedata = new string[] { };
        List<RR_Dialogue> tmpDialogues = new List<RR_Dialogue>();
        if (dialoguesList.Length == 1 && System.String.IsNullOrEmpty(dialoguesList[0])) {
            tmpDialogues.Add(new RR_Dialogue());
            return tmpDialogues;
        }
        for (int i = 0; i < dialoguesList.Length; i++) {
            dialoguedata = dialoguesList[i].Split(new string[] { ";" }, System.StringSplitOptions.None);
            tmpDialogues.Add(new RR_Dialogue());
            tmpDialogues[i].name = dialoguedata[0];
            tmpDialogues[i].expression = dialoguedata[1];
            tmpDialogues[i].dialogue = dialoguedata[2];
            tmpDialogues[i].tags = dialoguedata[3];
            tmpDialogues[i].index = int.Parse(dialoguedata[4]);
            tmpDialogues[i].nameMode = (RR_Dialogue.NameMode)int.Parse(dialoguedata[5]);
            tmpDialogues[i].nameShown = tmpDialogues[i].GetName(tmpDialogues[i].nameMode);
            tmpDialogues[i].charPos = (RR_Dialogue.CharPos)int.Parse(dialoguedata[6]);
        }
        return tmpDialogues;
    }
}