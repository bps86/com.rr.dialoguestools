using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

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
    public bool useShake;
    public NameMode nameMode;
    public Sprite sprite;
    public SkeletonDataAsset skeletonDataAsset;
    public Vector3 actorPosition;
    public Vector3 actorScale;
    public float scale;
    public RR_Dialogue(string _name = "name", string _expression = "expression", string _dialogue = "dialogue", string _tags = "tags", int _index = 0, NameMode _nameMode = NameMode.Normal, float _positionX = 0, float _positionY = 0, float _scale = 1, bool _isInverted = false, bool _useShake = false, string _sfxID = "", string _bgmID = "", string _voiceActID = "") {
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
        this.useShake = _useShake;
        this.sfxID = _sfxID;
        this.bgmID = _bgmID;
        this.voiceActID = _voiceActID;
    }
    public string GetName(NameMode _nameMode) {
        switch (_nameMode) {
            case NameMode.Hidden:
                return "?";
            case NameMode.None:
                return "";
            case NameMode.Normal:
                return this.actorName;
        }
        return "Error";
    }
}
