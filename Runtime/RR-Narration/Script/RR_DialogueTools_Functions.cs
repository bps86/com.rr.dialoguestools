using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using Spine.Unity;

public class RR_DialogueTools_Functions
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
            tmpDialogues.Add(new RR_Dialogue(
                _name: dialoguedata[0],
                _expression: dialoguedata[1],
                _dialogue: dialoguedata[2],
                _tags: dialoguedata[3],
                _index: int.Parse(dialoguedata[4]),
                _nameMode: (NameMode)int.Parse(dialoguedata[5]),
                _positionX: float.Parse(dialoguedata[6]),
                _positionY: float.Parse(dialoguedata[7]),
                _scale: float.Parse(dialoguedata[8]),
                _isInverted: bool.Parse(dialoguedata[9]),
                _animationLoop: bool.Parse(dialoguedata[10]),
                _useShake: bool.Parse(dialoguedata[11]),
                _useSilhouette: bool.Parse(dialoguedata[12]),
                _sfxID: dialoguedata[13],
                _bgmID: dialoguedata[14],
                _voiceActID: dialoguedata[15]
            ));
        }
        return tmpDialogues;
    }
}