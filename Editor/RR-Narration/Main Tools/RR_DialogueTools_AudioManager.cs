using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class RR_DialogueTools_AudioManager : EditorWindow
{
    public Action<int, string, string, string> ApplyEvent;

    private Rect cursorChangeRect;
    private Vector2 scrollPos;
    private Vector2 scrollPos2;
    private List<string> sfxList;
    private List<string> bgmList;
    private List<string> voiceActList;
    private float currentScrollViewWidth;
    private int index;
    private int mode;
    private bool resize;
    private string sfxID;
    private string bgmID;
    private string voiceActID;
    private const int SFX = 0;
    private const int BGM = 1;
    private const int VOICE_ACT = 2;

    public void init(int currentIndex, string sfxID, string bgmID, string voiceActID) {
        index = currentIndex;
        this.sfxID = sfxID;
        this.bgmID = bgmID;
        this.voiceActID = voiceActID;
        sfxList = new List<string>();
        bgmList = new List<string>();
        voiceActList = new List<string>();
        LoadSoundData();
    }

    public void LoadSoundData() {
        TextAsset sfxDataPath = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/RR-Narration/Resources/sfxPaths.txt");
        TextAsset bgmDataPath = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/RR-Narration/Resources/bgmPaths.txt");
        TextAsset voiceActDataPath = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/RR-Narration/Resources/voiceActPaths.txt");
        string name = "";
        string[] sfxPath = sfxDataPath.text.Split(';');
        string[] bgmPath = bgmDataPath.text.Split(';');
        string[] voiceActPath = voiceActDataPath.text.Split(';');
        sfxList.Add("");
        bgmList.Add("");
        voiceActList.Add("");
        for (int i = 0; i < sfxPath.Length; i++) {
            if (System.String.IsNullOrEmpty(sfxPath[i])) {
                continue;
            }
            name = sfxPath[i].Substring(sfxPath[i].LastIndexOf('/') + 1);
            sfxList.Add(name);
        }
        for (int i = 0; i < bgmPath.Length; i++) {
            if (System.String.IsNullOrEmpty(bgmPath[i])) {
                continue;
            }
            name = bgmPath[i].Substring(bgmPath[i].LastIndexOf('/') + 1);
            bgmList.Add(name);
        }
        for (int i = 0; i < voiceActPath.Length; i++) {
            if (System.String.IsNullOrEmpty(voiceActPath[i])) {
                continue;
            }
            name = voiceActPath[i].Substring(voiceActPath[i].LastIndexOf('/') + 1);
            voiceActList.Add(name);
        }
    }

    void OnEnable() {
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    public void init_Window() {
        position = new Rect(Screen.width / 2, Screen.height / 2, 400, 300);
        Show();
    }

    void OnGUI() {
        GUILayout.BeginHorizontal();
        DrawOptions();
        RR_EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        SelectAudio();
        GUILayout.EndHorizontal();
        Repaint();
    }

    void DrawOptions() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        if (GUILayout.Button("Sfx")) {
            mode = SFX;
        }
        if (GUILayout.Button("Bgm")) {
            mode = BGM;
        }
        if (GUILayout.Button("Voice Act")) {
            mode = VOICE_ACT;
        }
        if (GUILayout.Button("Apply")) {
            ApplyEvent(index, sfxID, bgmID, voiceActID);
            Close();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void SelectAudio() {
        switch (mode) {
            case SFX:
                SelectSFX();
                break;
            case BGM:
                SelectBGM();
                break;
            case VOICE_ACT:
                SelectVoiceAct();
                break;
        }
    }

    void SelectSFX() {
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (GUILayout.Button("None")) {
            sfxID = sfxList[0];
        }
        for (int i = 1; i < sfxList.Count; i++) {
            if (GUILayout.Button(sfxList[i])) {
                sfxID = sfxList[i];
            }
        }
        GUILayout.EndScrollView();
    }

    void SelectBGM() {
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (GUILayout.Button("None")) {
            bgmID = bgmList[0];
        }
        for (int i = 1; i < bgmList.Count; i++) {
            if (GUILayout.Button(bgmList[i])) {
                bgmID = bgmList[i];
            }
        }
        GUILayout.EndScrollView();
    }

    void SelectVoiceAct() {
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (GUILayout.Button("None")) {
            voiceActID = voiceActList[0];
        }
        for (int i = 1; i < voiceActList.Count; i++) {
            if (GUILayout.Button(voiceActList[i])) {
                voiceActID = voiceActList[i];
            }
        }
        GUILayout.EndScrollView();
    }

}