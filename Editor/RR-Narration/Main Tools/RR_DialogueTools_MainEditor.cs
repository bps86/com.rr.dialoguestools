using UnityEngine;
using UnityEditor;

public class RR_DialogueTools_MainEditor : EditorWindow
{
    private Rect cursorChangeRect;
    private RR_Narration rR_Narration;
    private RR_DialogueTools_Visualization rR_DialogueTools_Visualization;
    private Vector2 scrollPos;
    private Vector2 scrollPos2;
    private string fileName;
    private string fileData;
    private float currentScrollViewWidth;
    private bool resize;
    private bool ready;


    [MenuItem("Window/RR/Narration")]
    static void init() {
        RR_EditorTools.Initialize_RR_Dir();
        RR_DialogueTools_MainEditor thisWindow = (RR_DialogueTools_MainEditor)EditorWindow.GetWindow(typeof(RR_DialogueTools_MainEditor));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 1080, 600);
        thisWindow.Show();
    }

    void OnEnable() {
        rR_Narration = new RR_Narration();
        rR_DialogueTools_Visualization = new RR_DialogueTools_Visualization();
        RR_EditorTools.locales = RR_EditorTools.GetLocales(new string[] { });
        if (rR_Narration.dialogues == null) {
            RR_EditorTools.currentLocaleIndex = RR_EditorTools.locales.Length - 1;
        }
        currentScrollViewWidth = this.position.width / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 2f, this.position.height);
    }

    void OnGUI() {
        if (rR_Narration.dialogues != null) {
            fileData = DialoguesToString();
            rR_DialogueTools_Visualization.visual = GetVisualAsset();
        }
        GUILayout.BeginHorizontal();
        DrawOptions();
        RR_EditorDrawTools.ResizeScrollView(ref cursorChangeRect, this.position, ref resize, ref currentScrollViewWidth);
        DrawDialogue();
        GUILayout.EndHorizontal();
        Repaint();
    }

    private void OpenFileManagerWindow(RR_DialogueTools_FileMode fileMode) {
        RR_DialogueTools_FileManagerWindow rR_DialogueTools_FileManagerWindow = (RR_DialogueTools_FileManagerWindow)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_FileManagerWindow));
        rR_DialogueTools_FileManagerWindow.init(rR_Narration, rR_DialogueTools_Visualization);
        rR_DialogueTools_FileManagerWindow.SetData(fileName, fileData);
        rR_DialogueTools_FileManagerWindow.OpenEvent += OnOpen;
        rR_DialogueTools_FileManagerWindow.CloseEvent += OnClose;
        rR_DialogueTools_FileManagerWindow.init_Window(fileMode);
        GUIUtility.ExitGUI();
    }

    private void OpenActorManagerWindow(int index) {
        RR_DialogueTools_ActorManager rR_DialogueTools_ActorManager = (RR_DialogueTools_ActorManager)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_ActorManager));
        rR_DialogueTools_ActorManager.init(RR_ActorManagerMode.Dialogue, index);
        rR_DialogueTools_ActorManager.SetRRVar(rR_Narration, rR_DialogueTools_Visualization);
        rR_DialogueTools_ActorManager.SetRRVarEvent += OnActorApply;
        rR_DialogueTools_ActorManager.init_Window();
        GUIUtility.ExitGUI();
    }

    private void OpenAudioManagerWindow(int index, string sfxID, string bgmID, string voiceActID) {
        RR_DialogueTools_AudioManager rR_DialogueTools_AudioManager = (RR_DialogueTools_AudioManager)ScriptableObject.CreateInstance(typeof(RR_DialogueTools_AudioManager));
        rR_DialogueTools_AudioManager.init(index, sfxID, bgmID, voiceActID);
        rR_DialogueTools_AudioManager.ApplyEvent += OnAudioApply;
        rR_DialogueTools_AudioManager.init_Window();
        GUIUtility.ExitGUI();
    }

    private void DrawOptions() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));
        if (GUILayout.Button("New")) {
            ready = false;
            OpenFileManagerWindow(RR_DialogueTools_FileMode.New);
        }
        if (GUILayout.Button("Open")) {
            ready = false;
            OpenFileManagerWindow(RR_DialogueTools_FileMode.Open);
        }
        if (GUILayout.Button("Refresh")) {
            RR_EditorTools.Refresh_RR_DialogueTools();
        }
        if (rR_Narration.dialogues != null) {
            if (GUILayout.Button("Save")) {
                ready = false;
                OpenFileManagerWindow(RR_DialogueTools_FileMode.Save);
            }
            if (GUILayout.Button("New Dialogue")) rR_Narration.dialogues.Add(new RR_Dialogue(
                _tags: rR_Narration.dialogues[rR_Narration.dialogues.Count - 1].tags,
                _index: rR_Narration.dialogues[rR_Narration.dialogues.Count - 1].index + 1)
                );
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDialogue() {
        if (!ready) return;

        scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Width(this.position.width - currentScrollViewWidth));
        if (rR_Narration.dialogues != null)
            for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
                string nameThumb = rR_Narration.dialogues[i].actorName;
                string expressionThumb = rR_Narration.dialogues[i].expression;
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(100));
                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/RR-Thumbnail/Thumbnail-" + nameThumb + "," + expressionThumb + ".png"), GUILayout.Width(150), GUILayout.Height(150))) {
                    int index = i;
                    OpenActorManagerWindow(i);
                }
                GUILayout.EndVertical();
                Vector2 scrollPos3 = Vector2.zero;
                scrollPos3 = GUILayout.BeginScrollView(scrollPos3, GUILayout.Width(320), GUILayout.Height(150));
                rR_Narration.dialogues[i].dialogue = GUILayout.TextArea(rR_Narration.dialogues[i].dialogue, GUILayout.Width(300), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                GUILayout.BeginVertical(GUILayout.Width(60));
                GUILayout.Label("Tags: ");
                GUILayout.Label("Index: ");
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.Width(60));
                rR_Narration.dialogues[i].tags = GUILayout.TextField(rR_Narration.dialogues[i].tags, GUILayout.Width(60));
                rR_Narration.dialogues[i].index = EditorGUILayout.IntField(rR_Narration.dialogues[i].index, GUILayout.Width(60));
                rR_Narration.dialogues[i].nameMode = (RR_DialogueTools_NameMode)EditorGUILayout.EnumPopup(rR_Narration.dialogues[i].nameMode);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                GUILayout.Label($"Selected SFX: {rR_Narration.dialogues[i].sfxID}", GUILayout.Width(180));
                GUILayout.Label($"Selected BGM: {rR_Narration.dialogues[i].bgmID}", GUILayout.Width(180));
                GUILayout.Label($"Selected Voice Act: {rR_Narration.dialogues[i].voiceActID}", GUILayout.Width(180));
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (GUILayout.Button("Edit Audio", GUILayout.Width(80))) {
                    int index = i;
                    OpenAudioManagerWindow(i, rR_Narration.dialogues[i].sfxID, rR_Narration.dialogues[i].bgmID, rR_Narration.dialogues[i].voiceActID);
                }
                if (GUILayout.Button("Remove", GUILayout.Width(80))) rR_Narration.dialogues.RemoveAt(i);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.Width(100));
                rR_Narration.dialogues[i].isInverted = EditorGUILayout.Toggle("Is Inverted: ", rR_Narration.dialogues[i].isInverted);
                rR_Narration.dialogues[i].animationLoop = EditorGUILayout.Toggle("Loop Animation: ", rR_Narration.dialogues[i].animationLoop);
                rR_Narration.dialogues[i].useShake = EditorGUILayout.Toggle("Use Shake: ", rR_Narration.dialogues[i].useShake);
                rR_Narration.dialogues[i].useSilhouette = EditorGUILayout.Toggle("Use Silhouette: ", rR_Narration.dialogues[i].useSilhouette);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Position X: ", GUILayout.Width(60));
                rR_Narration.dialogues[i].actorPosition.x = EditorGUILayout.FloatField(rR_Narration.dialogues[i].actorPosition.x, GUILayout.Width(60));
                GUILayout.Label("Position Y: ", GUILayout.Width(60));
                rR_Narration.dialogues[i].actorPosition.y = EditorGUILayout.FloatField(rR_Narration.dialogues[i].actorPosition.y, GUILayout.Width(60));
                GUILayout.Label("Actor Scale: ", GUILayout.Width(80));
                rR_Narration.dialogues[i].scale = EditorGUILayout.FloatField(rR_Narration.dialogues[i].scale, GUILayout.Width(60));
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                GUILayout.EndVertical();
            }
        GUILayout.EndScrollView();
    }

    public string DialoguesToString(string fileData = "") {
        for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
            if (i > 0) fileData += "||";
            fileData += DialogueToString(rR_Narration.dialogues[i], ";");
        }
        return fileData;
    }
    static string DialogueToString(RR_Dialogue dialogue, string separator, string dialoguedata = "") {
        dialoguedata += dialogue.actorName + separator;
        dialoguedata += dialogue.expression + separator;
        dialoguedata += dialogue.dialogue.Replace(System.Environment.NewLine, "</n>") + separator;
        dialoguedata += dialogue.tags + separator;
        dialoguedata += dialogue.index.ToString() + separator;
        dialoguedata += (int)dialogue.nameMode + separator;
        dialoguedata += dialogue.actorPosition.x + separator;
        dialoguedata += dialogue.actorPosition.y + separator;
        dialoguedata += dialogue.scale + separator;
        dialoguedata += dialogue.isInverted + separator;
        dialoguedata += dialogue.animationLoop + separator;
        dialoguedata += dialogue.useShake + separator;
        dialoguedata += dialogue.useSilhouette + separator;
        dialoguedata += dialogue.sfxID + separator;
        dialoguedata += dialogue.bgmID + separator;
        dialoguedata += dialogue.voiceActID;
        return dialoguedata;
    }
    public RR_DialogueTools_Visual GetVisualAsset() {
        for (int i = 0; i < rR_Narration.dialogues.Count; i++) {
            if (rR_DialogueTools_Visualization.visual.visualDatas.Count < rR_Narration.dialogues.Count) rR_DialogueTools_Visualization.visual.visualDatas.Add(new RR_DialogueTools_VisualData());
            rR_DialogueTools_Visualization.visual.visualDatas[i].tags = rR_Narration.dialogues[i].tags;
            rR_DialogueTools_Visualization.visual.visualDatas[i].index = rR_Narration.dialogues[i].index;
        }
        for (int i = rR_DialogueTools_Visualization.visual.visualDatas.Count - 1; i >= 0; i--) {
            if (rR_DialogueTools_Visualization.visual.visualDatas.Count > rR_Narration.dialogues.Count) rR_DialogueTools_Visualization.visual.visualDatas.RemoveAt(i);
        }
        return rR_DialogueTools_Visualization.visual;
    }
    private void OnOpen(string fileName, string fileData, RR_Narration selected_RR_Narration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        this.fileName = fileName;
        this.fileData = fileData;
        this.rR_Narration = selected_RR_Narration;
        this.rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
        this.ready = true;
    }

    private void OnAudioApply(int index, string sfxID, string bgmID, string voiceActID) {
        rR_Narration.dialogues[index].sfxID = sfxID;
        rR_Narration.dialogues[index].bgmID = bgmID;
        rR_Narration.dialogues[index].voiceActID = voiceActID;
    }

    private void OnActorApply(RR_Narration selected_RR_Narration, RR_DialogueTools_Visualization selected_RR_DialogueTools_Visualization) {
        this.rR_Narration = selected_RR_Narration;
        this.rR_DialogueTools_Visualization = selected_RR_DialogueTools_Visualization;
    }

    private void OnClose() {
        this.ready = true;
    }

    private void OnDestroy() {
        RR_EditorTools.Refresh_RR_DialogueTools();
    }
}