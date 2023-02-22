using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using Spine.Unity;

public static class RR_NarrationManager
{
    public static void NewFile(string fileName)
    {
        string newFile = null;
        using StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + fileName);
        writer.Write(newFile);
        writer.Close();
    }
    public static string OpenFile(string path)
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        return json;
    }
    public static List<RRDialogue> OpenDialogueFile(string path)
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        string[] dialoguesList = json.Split(new string[] { "||" }, System.StringSplitOptions.None);
        return GetDialogues(dialoguesList);
    }
    public static void SaveFile(string path, string fileData)
    {
        using StreamWriter write = new StreamWriter(path);
        write.Write(fileData);
        write.Close();
    }

    public static List<RRDialogue> GetDialogues(string[] dialoguesList)
    {
        List<RRDialogue> tmpDialogues = new List<RRDialogue>();
        if (dialoguesList.Length == 1 && System.String.IsNullOrEmpty(dialoguesList[0]))
        {
            tmpDialogues.Add(new RRDialogue());
            return tmpDialogues;
        }
        for (int i = 0; i < dialoguesList.Length; i++)
        {
            string[] dialoguedata = dialoguesList[i].Split(new string[] { ";" }, System.StringSplitOptions.None);
            tmpDialogues.Add(new RRDialogue());
            tmpDialogues[i].name = dialoguedata[0];
            tmpDialogues[i].expression = dialoguedata[1];
            tmpDialogues[i].dialogue = dialoguedata[2];
            tmpDialogues[i].tags = dialoguedata[3];
            tmpDialogues[i].index = int.Parse(dialoguedata[4]);
            tmpDialogues[i].nameMode = (RRDialogue.NameMode)int.Parse(dialoguedata[5]);
            tmpDialogues[i].nameShown = tmpDialogues[i].GetName(tmpDialogues[i].nameMode);
            tmpDialogues[i].charPos = (RRDialogue.CharPos)int.Parse(dialoguedata[6]);
        }
        return tmpDialogues;
    }
}