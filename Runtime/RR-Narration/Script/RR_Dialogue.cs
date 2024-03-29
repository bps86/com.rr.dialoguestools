using UnityEngine;

public class RR_Dialogue
{
    public string actorName;
    public string displayName;
    public string expression;
    public string dialogue;
    public string tags;
    public string sfxID;
    public string bgmID;
    public string voiceActID;
    public int index;
    public bool isInverted;
    public bool animationLoop;
    public bool useShake;
    public bool useSilhouette;
    public RR_DialogueTools_NameMode nameMode;
    public Vector3 actorPosition;
    public Vector3 actorScale;
    public float scale;
    public RR_Dialogue(string _name = "name", string _expression = "expression", string _dialogue = "dialogue", string _tags = "tags", int _index = 0, RR_DialogueTools_NameMode _nameMode = RR_DialogueTools_NameMode.Normal, float _positionX = 0, float _positionY = 0, float _scale = 1, bool _isInverted = false, bool _animationLoop = false, bool _useShake = false, bool _useSilhouette = false, string _sfxID = "", string _bgmID = "", string _voiceActID = "") {
        this.actorName = _name;
        this.expression = _expression;
        this.dialogue = _dialogue;
        this.tags = _tags;
        this.index = _index;
        this.nameMode = _nameMode;
        this.displayName = GetName(_nameMode);
        this.actorPosition = new Vector3(_positionX, _positionY, 0);
        this.isInverted = _isInverted;
        this.scale = _scale;
        if (!isInverted) {
            this.actorScale = new Vector3(scale, scale, scale);
        } else {
            this.actorScale = new Vector3(-scale, scale, scale);
        }
        this.animationLoop = _animationLoop;
        this.useShake = _useShake;
        this.useSilhouette = _useSilhouette;
        this.sfxID = _sfxID;
        this.bgmID = _bgmID;
        this.voiceActID = _voiceActID;
    }
    public string GetName(RR_DialogueTools_NameMode _nameMode) {
        switch (_nameMode) {
            case RR_DialogueTools_NameMode.Hidden:
                return "?";
            case RR_DialogueTools_NameMode.None:
                return "";
            case RR_DialogueTools_NameMode.Normal:
                if (this.actorName.Contains("_")) {
                    return this.actorName.Substring(0, this.actorName.IndexOf('_'));
                } else {
                    return this.actorName;
                }
        }
        return "Error";
    }
}
