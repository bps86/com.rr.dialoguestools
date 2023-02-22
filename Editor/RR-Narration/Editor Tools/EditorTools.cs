using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class EditorTools
{
    public static string fileName, fileData;
    static string[] spritePath = new string[] { };
    static string[] spinePath = new string[] { };
    static string[] beepPath = new string[] { };
    public static string[] names = new string[] { }, locales = new string[] { };
    public static Dictionary<string, int> actorDataIndex = new Dictionary<string, int>();
    public static Dictionary<string, string[]> expression = new Dictionary<string, string[]>();
    public static List<bool> hidename;
    public static string currentLocale;
    public static int currentLocaleIndex;
    public static string getFileName;
    public static StringTableCollection rrDialoguesTable;
    public static void GetDialogueIndex()
    {
        for (int i = 0; i < RR_Narration.dialogues.Count; i++)
        {
            EditorTools.actorDataIndex["Actor" + i] = 0;
            EditorTools.actorDataIndex["Expression" + i] = 0;
        }
    }
    [MenuItem("Tools/RR/Initialize RR Dir & Settings")]
    public static void Initialize_RR_Dir()
    {
        LocalizationSettings localizationSettings = null;
        if (!Directory.Exists("Assets/RR-Narration")) Directory.CreateDirectory("Assets/RR-Narration");
        if (!Directory.Exists("Assets/RR-Narration/Resources")) Directory.CreateDirectory("Assets/RR-Narration/Resources");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Dialogues")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Dialogues");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-DialoguesTable");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Actors-Spine")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Actors-Spine");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Beep")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Beep");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Sound/Voice-Act")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Sound/Voice-Act");
        if (!Directory.Exists("Assets/RR-Narration/Resources/RR-Visual")) Directory.CreateDirectory("Assets/RR-Narration/Resources/RR-Visual");
        if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
        if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");
        if (LocalizationEditorSettings.ActiveLocalizationSettings != null)
        {
            localizationSettings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (LocalizationEditorSettings.GetLocales().Count > 0)
                if (!File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset") || !File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue Shared Data.asset"))
                {
                    LocalizationEditorSettings.CreateStringTableCollection("RR-Dialogue", "Assets/RR-Narration/Resources/RR-DialoguesTable", LocalizationEditorSettings.GetLocales());
                }
        }
        if (File.Exists("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset"))
        {
            rrDialoguesTable = AssetDatabase.LoadAssetAtPath<StringTableCollection>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue.asset");
            List<Locale> missingLocale = new List<Locale>();
            for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
                if (!rrDialoguesTable.ContainsTable(LocalizationEditorSettings.GetLocales()[i].Identifier))
                    missingLocale.Add(LocalizationEditorSettings.GetLocales()[i]);
            for (int i = 0; i < missingLocale.Count; i++)
                rrDialoguesTable.AddNewTable(missingLocale[i].Identifier);
        }

        EditorTools.CheckSortingLayers();

        EditorTools.Refresh_RR_DialogueTools();
    }
    [MenuItem("Tools/RR/Refresh RR AssetPaths")]
    public static void Refresh_RR_DialogueTools()
    {
        spritePath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Actors/");
        spinePath = Directory.GetDirectories("Assets/RR-Narration/Resources/RR-Actors-Spine/");
        beepPath = Directory.GetFiles("Assets/RR-Narration/Resources/RR-Sound/Beep/");
        int index = 0;
        int nameIndex = 0;
        int beepIndex = 0;
        List<string> temp = new List<string>();
        List<string> tempSprite = new List<string>();
        List<string> tempSpriteActor = new List<string>();
        Dictionary<string, List<string>> dictSprite = new Dictionary<string, List<string>>();
        List<string> tempSpine = new List<string>();
        string spritePaths = "";
        string spinePaths = "";
        string beepPaths = "";
        if (!Directory.Exists("Assets/Editor")) Directory.CreateDirectory("Assets/Editor");
        if (!Directory.Exists("Assets/Editor/RR-Thumbnail")) Directory.CreateDirectory("Assets/Editor/RR-Thumbnail");

        for (int i = 0; i < spritePath.Length; i++)
        {
            if (spritePath[i].Contains(".meta"))
            {
                continue;
            }
            tempSprite.Add(spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf('.') - spritePath[i].LastIndexOf('/') - 1));
            if (nameIndex > 0) spritePaths += ";";
            spritePaths += "RR-Actors/" + tempSprite[nameIndex];
            File.Copy(spritePath[i], "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSprite[nameIndex] + ".png", true);
            nameIndex += 1;
            string name = spritePath[i].Substring(spritePath[i].LastIndexOf('/') + 1, spritePath[i].LastIndexOf(',') - spritePath[i].LastIndexOf('/') - 1);
            if (temp.Contains(name))
            {
                continue;
            }
            temp.Add(name);
            tempSpriteActor.Add(name);
            dictSprite[name] = new List<string>();
            index += 1;
        }
        for (int i = 0; i < spinePath.Length; i++)
        {
            string name = spinePath[i].Substring(spinePath[i].LastIndexOf('/') + 1);
            if (temp.Contains(name))
            {
                continue;
            }
            temp.Add(name);
            tempSpine.Add(name);
        }
        for (int i = 0; i < beepPath.Length; i++)
        {
            if (beepPath[i].Contains(".meta"))
            {
                continue;
            }
            string beepName = beepPath[i].Substring(beepPath[i].LastIndexOf('/') + 1, beepPath[i].LastIndexOf('.') - beepPath[i].LastIndexOf('/') - 1);
            if (beepIndex > 0) beepPaths += ";";
            beepPaths += "RR-Sound/Beep/" + beepName;
            beepIndex += 1;
        }
        names = temp.ToArray();
        for (int i = 0; i < tempSprite.Count; i++)
        {
            string name = tempSprite[i].Substring(0, tempSprite[i].LastIndexOf(','));
            string expression = tempSprite[i].Substring(tempSprite[i].LastIndexOf(',') + 1, tempSprite[i].Length - tempSprite[i].LastIndexOf(',') - 1);
            dictSprite[name].Add(expression);
        }
        for (int i = 0; i < tempSpriteActor.Count; i++)
        {
            expression[tempSpriteActor[i]] = dictSprite[tempSpriteActor[i]].ToArray();
        }
        for (int i = 0; i < tempSpine.Count; i++)
        {
            List<string> name = new List<string>();
            if (i > 0) spinePaths += ";";
            spinePaths += "RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i];
            Spine.Unity.SkeletonDataAsset skeletonDataAsset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>("Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/skeleton_SkeletonData.asset");
            Spine.SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
            Spine.Animation[] animations = skeletonData.Animations.ToArray();
            for (int _index = 0; _index < animations.Length; _index++)
            {
                name.Add(animations[_index].Name);
                string thumbPath = "Assets/RR-Narration/Resources/RR-Actors-Spine/" + tempSpine[i] + "/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png";
                if (File.Exists(thumbPath))
                {
                    File.Copy(thumbPath, "Assets/Editor/RR-Thumbnail/Thumbnail-" + tempSpine[i] + "," + animations[_index].Name + ".png", true);
                    File.Delete(thumbPath);
                    File.Delete(thumbPath + ".meta");
                }
            }
            expression[tempSpine[i]] = name.ToArray();
        }
        RR_NarrationManager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spritePaths.txt", spritePaths);
        RR_NarrationManager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/spinePaths.txt", spinePaths);
        RR_NarrationManager.SaveFile(Application.dataPath + Path.AltDirectorySeparatorChar + "RR-Narration/Resources/beepPaths.txt", beepPaths);
        AssetDatabase.Refresh();
    }
    // [MenuItem("Testing/AddSortLayer")]
    public static void CheckSortingLayers()
    {
        var tagMan = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (tagMan == null) return;
        var so = new SerializedObject(tagMan);
        var m_SortingLayers = so.FindProperty("m_SortingLayers");
        Debug.Log(m_SortingLayers.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue);
        int uid1 = CheckIdDuplicates(m_SortingLayers);
        int uid2 = CheckIdDuplicates(m_SortingLayers);
        int uid3 = CheckIdDuplicates(m_SortingLayers);
        CreateRRLayers("RR-Layers1", uid1, so, m_SortingLayers);
        CreateRRLayers("RR-Layers2", uid2, so, m_SortingLayers);
        CreateRRLayers("RR-Layers3", uid3, so, m_SortingLayers);
        EditorUtility.SetDirty(tagMan);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static int CheckIdDuplicates(SerializedProperty sp)
    {
        int uniqueID = GenerateUID();
        for (int i = 0; i < sp.arraySize; i++)
        {
            if (sp.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueId") == null)
                continue;
            if (uniqueID == 0 || uniqueID == sp.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueId").intValue)
            {
                uniqueID = GenerateUID();
                i = -1;
                continue;
            }
            continue;
        }
        return uniqueID;
    }

    public static void CreateRRLayers(string name, int uid, SerializedObject so, SerializedProperty m_SortingLayers)
    {
        var num_SortingLayers = m_SortingLayers.arraySize;
        for (int i = 0; i < num_SortingLayers; i++)
        {
            if (name == m_SortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue) return;
        }
        m_SortingLayers.InsertArrayElementAtIndex(num_SortingLayers);
        m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("name").stringValue = name;
        m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("uniqueID").intValue = uid;
        m_SortingLayers.GetArrayElementAtIndex(num_SortingLayers).FindPropertyRelative("locked").boolValue = false;
        so.ApplyModifiedProperties();
        so.Update();
    }

    public static void ToStringTable(string _filename, string _fileData, StringTableCollection stringTableCollection)
    {
        if (EditorTools.currentLocaleIndex == EditorTools.locales.Length - 1) return;
        for (int i = 0; i < stringTableCollection.StringTables.Count; i++)
        {
            if (stringTableCollection.StringTables[i].name == "RR-Dialogue_" + EditorTools.currentLocale)
            {
                SetStringTable(_filename, _fileData, AssetDatabase.LoadAssetAtPath<StringTable>("Assets/RR-Narration/Resources/RR-DialoguesTable/RR-Dialogue_" + EditorTools.currentLocale + ".asset"));
            }
        }
    }
    public static void SetStringTable(string _filename, string _fileData, StringTable stringTable)
    {
        StringTableEntry entry = GetStringTableEntry(stringTable, _filename);
        entry.Value = _fileData;
        EditorUtility.SetDirty(stringTable);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static StringTableEntry GetStringTableEntry(StringTable stringTable, string entry)
    {
        return stringTable.GetEntry(entry) ?? stringTable.AddEntry(entry, string.Empty);
    }

    public static string[] GetLocales(string[] localesArray)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < LocalizationEditorSettings.GetLocales().Count; i++)
        {
            names.Add(LocalizationEditorSettings.GetLocales()[i].Identifier.Code);
        }
        names.Add("<None>");
        return names.ToArray();
    }

    public static void CreateNewVisualAsset()
    {
        RR_NarrationVisualization.visual = new RR_NarrationVisual();
        string visualJson = JsonUtility.ToJson(RR_NarrationVisualization.visual);
        RR_NarrationManager.SaveFile("Assets/RR-Narration/Resources/RR-Visual/" + EditorTools.fileName + ".json", visualJson);
    }

    static int GenerateUID()
    {
        int newGuid = System.Guid.NewGuid().GetHashCode();
        if (newGuid < 0) newGuid *= -1;
        return newGuid;
    }
}