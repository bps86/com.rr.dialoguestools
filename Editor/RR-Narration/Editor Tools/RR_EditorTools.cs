using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Spine.Unity;

public class RR_EditorTools
{
    static string[] spritePath = new string[] { };
    static string[] spinePath = new string[] { };
    static string[] rectDataPath = new string[] { };
    static string[] beepPath = new string[] { };
    static string[] sfxPath = new string[] { };
    static string[] bgmPath = new string[] { };
    static string[] voiceActPath = new string[] { };
    public static string[] names = new string[] { }, locales = new string[] { };
    public static Dictionary<string, int> actorDataIndex = new Dictionary<string, int>();
    public static Dictionary<string, string[]> expression = new Dictionary<string, string[]>();
    public static List<bool> hidename;
    public static string currentLocale;
    public static int currentLocaleIndex;
    public static StringTableCollection rrDialoguesTable;
    public static SharedTableData rrDialoguesSharedData;
    public static void GetDialogueIndex(RR_Narration rR_Narration) {
        for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
            actorDataIndex["Actor" + i] = 0;
            actorDataIndex["Expression" + i] = 0;
        }
    }
    [MenuItem("Tools/RR/Initialize RR Dir & Settings")]

    public static void Initialize_RR_Dir() {
        LocalizationSettings localizationSettings = null;
        if (!Directory.Exists("Assets/RR-Narration")) Directory.CreateDirectory("Assets/RR-Narration");
        if (!Directory.Exists("Assets/RR-Narration/Resources")) Directory.CreateDirectory("Assets/RR-Narration/Resources");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-DialoguesTable");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors-Spine")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors-Spine");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors-RectData")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors-RectData");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Beep")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Beep");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Voice-Act")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Voice-Act");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Sfx")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Sfx");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Bgm")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Bgm");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Visual")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Visual");
        if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
        if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");
        if (LocalizationEditorSettings.ActiveLocalizationSettings != null) {
            localizationSettings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (LocalizationEditorSettings.GetLocales().Count > 0)
                if (!File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset") || !File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue Shared Data.asset")) {
                    LocalizationEditorSettings.CreateStringTableCollection("RR-Dialogue", "Assets/RR-Narration/Resources/RR-DialoguesTable", LocalizationEditorSettings.GetLocales());
                    rrDialoguesSharedData = GetSharedTableData(rrDialoguesSharedData);
                }
        }
        if (File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset")) {
            rrDialoguesTable = AssetDatabase.LoadAssetAtPath<StringTableCollection>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset");
            List<Locale> missingLocale = new List<Locale>();
            for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
                if (!rrDialoguesTable.ContainsTable(LocalizationEditorSettings.GetLocales()[i].Identifier))
                    missingLocale.Add(LocalizationEditorSettings.GetLocales()[i]);
            for (int i = 0; i < missingLocale.Count; i++)
                rrDialoguesTable.AddNewTable(missingLocale[i].Identifier);
        }

        RR_EditorTools.Refresh_RR_DialogueTools();
    }
    [MenuItem("Tools/RR/Refresh RR AssetPaths")]
    public static void Refresh_RR_DialogueTools() {
        spritePath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Actors/");
        spinePath = Directory.GetDirectories("Assets/RR-Narration/Resources/RR-Actors-Spine/");
        rectDataPath = Directory.GetDirectories("Assets/RR-Narration/Resources/RR-Actors-RectData/");
        beepPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Beep/");
        sfxPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Sfx/");
        bgmPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Bgm/");
        voiceActPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Voice-Act/");
        int index = 0;
        int nameIndex = 0;
        List<string> temp = new List<string>();
        List<string> tempSprite = new List<string>();
        List<string> tempSpriteActor = new List<string>();
        Dictionary<string, List<string>> dictSprite = new Dictionary<string, List<string>>();
        List<string> tempSpine = new List<string>();
        TextureImporter textureImporter = null;
        string spritePaths = "";
        string spinePaths = "";
        string rectDataPaths = "";
        string beepPaths = "";
        string sfxPaths = "";
        string bgmPaths = "";
        string voiceActPaths = "";
        if (!Directory.Exists("Assets/Editor")) {
            Directory.CreateDirectory("Assets/Editor");
        }
        if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) {
            Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");
        }

        for (int i = 0; i < spritePath.Length; i++) {
            if (spritePath[i].Contains(".meta")) {
                continue;
            }
            tempSprite.Add(spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf('.') - spritePath[i].LastIndexOf('/') - 1));
            if (nameIndex > 0) {
                spritePaths += ";";
            }
            spritePaths += "RR-Actors/" + tempSprite[nameIndex];
            textureImporter = AssetImporter.GetAtPath("Assets/RR-Narration/Resources/RR-Actors/" + tempSprite[nameIndex] + ".png") as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            AssetDatabase.WriteImportSettingsIfDirty("Assets/RR-Narration/Resources/RR-Actors/" + tempSprite[nameIndex] + ".png");
            File.Copy(spritePath[i], "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSprite[nameIndex] + ".png", true);
            nameIndex += 1;
            string name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
            if (temp.Contains(name)) {
                continue;
            }
            temp.Add(name);
            tempSpriteActor.Add(name);
            dictSprite[name] = new List<string>();
            if (!File.Exists(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/RR-Actors-RectData/" + name + ".txt")) {
                RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/RR-Actors-RectData/" + name + ".txt", "0;0;0;1;1;1");
            }
            if (!System.String.IsNullOrEmpty(rectDataPaths)) {
                rectDataPaths += ";";
            }
            rectDataPaths += "RR-Actors-RectData/" + name;
        }
        for (int i = 0; i < spinePath.Length; i++) {
            string name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            if (temp.Contains(name)) {
                continue;
            }
            temp.Add(name);
            tempSpine.Add(name);
            if (!File.Exists(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/RR-Actors-RectData/" + name + ".txt")) {
                RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/RR-Actors-RectData/" + name + ".txt", "0;0;0;1;1;1");
            }
            if (!System.String.IsNullOrEmpty(rectDataPaths)) {
                rectDataPaths += ";";
            }
            rectDataPaths += "RR-Actors-RectData/" + name;
        }
        index = 0;
        for (int i = 0; i < beepPath.Length; i++) {
            if (beepPath[i].Contains(".meta")) {
                continue;
            }
            string beepName = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1, beepPath[i].LastIndexOf('.') - beepPath[i].LastIndexOf('/') - 1);
            if (index > 0) {
                beepPaths += ";";
            }
            beepPaths += "RR-Sound/Beep/" + beepName;
            index += 1;
        }
        index = 0;
        for (int i = 0; i < sfxPath.Length; i++) {
            if (sfxPath[i].Contains(".meta")) {
                continue;
            }
            string sfxName = sfxPath[i].Substring(sfxPath[i].LastIndexOf('/') + 1, sfxPath[i].LastIndexOf('.') - sfxPath[i].LastIndexOf('/') - 1);
            if (index > 0) {
                sfxPaths += ";";
            }
            sfxPaths += "RR-Sound/Sfx/" + sfxName;
            index += 1;
        }
        index = 0;
        for (int i = 0; i < bgmPath.Length; i++) {
            if (bgmPath[i].Contains(".meta")) {
                continue;
            }
            string bgmName = bgmPath[i].Substring(bgmPath[i].LastIndexOf('/') + 1, bgmPath[i].LastIndexOf('.') - bgmPath[i].LastIndexOf('/') - 1);
            if (index > 0) {
                bgmPaths += ";";
            }
            bgmPaths += "RR-Sound/Bgm/" + bgmName;
            index += 1;
        }
        index = 0;
        for (int i = 0; i < voiceActPath.Length; i++) {
            if (voiceActPath[i].Contains(".meta")) {
                continue;
            }
            string voiceActName = voiceActPath[i].Substring(voiceActPath[i].LastIndexOf('/') + 1, voiceActPath[i].LastIndexOf('.') - voiceActPath[i].LastIndexOf('/') - 1);
            if (index > 0) {
                voiceActPaths += ";";
            }
            voiceActPaths += "RR-Sound/Bgm/" + voiceActName;
            index += 1;
        }


        names = temp.ToArray();
        for (int i = 0; i < tempSprite.Count; i++) {
            string name = tempSprite[i].Substring(0, tempSprite[i].LastIndexOf(','));
            string expression = tempSprite[i].Substring(tempSprite[i].LastIndexOf(',') + 1, tempSprite[i].Length - tempSprite[i].LastIndexOf(',') - 1);
            dictSprite[name].Add(expression);
        }
        for (int i = 0; i < tempSpriteActor.Count; i++) {
            expression[tempSpriteActor[i]] = dictSprite[tempSpriteActor[i]].ToArray();
        }
        for (int i = 0; i < tempSpine.Count; i++) {
            List<string> name = new List<string>();
            if (i > 0) {
                spinePaths += ";";
            }
            spinePaths += "RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i];
            SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>("Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/skeleton_SkeletonData.asset");
            Spine.SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
            Spine.Animation[] animations = skeletonData.Animations.ToArray();
            for (int _index = 0; _index < animations.Length; _index++) {
                name.Add(animations[_index].Name);
                string thumbPath = "Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png";
                if (File.Exists(thumbPath)) {
                    File.Copy(thumbPath, "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png", true);
                    File.Delete(thumbPath);
                    File.Delete(thumbPath + ".meta");
                }
            }
            expression[tempSpine[i]] = name.ToArray();
        }
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spritePaths.txt", spritePaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spinePaths.txt", spinePaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/rectDataPaths.txt", rectDataPaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/beepPaths.txt", beepPaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/bgmPaths.txt", bgmPaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/sfxPaths.txt", sfxPaths);
        RR_DialogueTools_Functions.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/voiceActPaths.txt", voiceActPaths);
        AssetDatabase.Refresh();
    }

    public static void ToStringTable(string _filename, string _fileData, StringTableCollection stringTableCollection) {
        if (RR_EditorTools.currentLocaleIndex == RR_EditorTools.locales.Length - 1) return;
        for (int i = 0; i < stringTableCollection.StringTables.Count; i++) {
            if (stringTableCollection.StringTables[i].name == "RR-Dialogue_" + RR_EditorTools.currentLocale) {
                SetStringTable(_filename, _fileData, AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + RR_EditorTools.currentLocale + ".asset"));
            }
        }
    }
    public static void SetStringTable(string _filename, string _fileData, StringTable stringTable) {
        stringTable.AddEntry(_filename, _fileData);
        rrDialoguesSharedData = GetSharedTableData(rrDialoguesSharedData);
        EditorUtility.SetDirty(stringTable);
        EditorUtility.SetDirty(rrDialoguesSharedData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static SharedTableData GetSharedTableData(SharedTableData sharedTableData) {
        if (sharedTableData != null) {
            return sharedTableData;
        } else {
            return AssetDatabase.LoadAssetAtPath<SharedTableData>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue Shared Data.asset");
        }
    }

    public static StringTableEntry GetStringTableEntry(StringTable stringTable, string entry) {
        return stringTable.GetEntry(entry) ?? stringTable.AddEntry(entry, string.Empty);
    }

    public static string[] GetLocales(string[] localesArray) {
        List<string> names = new List<string>();
        for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++) {
            names.Add(LocalizationEditorSettings.GetLocales()[i].Identifier.Code);
        }
        names.Add("<None>");
        return names.ToArray();
    }

    public static void CreateNewVisualAsset(string fileName) {
        RR_DialogueTools_Visual visual = new RR_DialogueTools_Visual();
        string visualJson = JsonUtility.ToJson(visual);
        RR_DialogueTools_Functions.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + fileName + ".json", visualJson);
    }
}