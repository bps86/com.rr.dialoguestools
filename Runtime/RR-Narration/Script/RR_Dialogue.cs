using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RRDialogue
{
    public string name;
    public string nameShown;
    public string expression;
    [TextArea]
    public string dialogue;
    public string tags;
    public int index;
    public enum NameMode { Normal = 0, Hidden = 1, None = 2 }
    public NameMode nameMode;
    public enum CharPos { Left = -1, Center = 0, Right = 1 }
    public CharPos charPos;
    public Sprite sprite;
    public SkeletonDataAsset skeletonDataAsset;
    public RRDialogue(string _name = "name", string _expression = "expression", string _dialogue = "dialogue", string _tags = "tags", int _index = 0, NameMode _nameMode = NameMode.Normal, CharPos _charPos = CharPos.Center)
    {
        this.name = _name;
        this.expression = _expression;
        this.dialogue = _dialogue;
        this.tags = _tags;
        this.index = _index;
        this.nameMode = _nameMode;
        this.nameShown = GetName(_nameMode);
        this.charPos = _charPos;
    }
    public string GetName(NameMode _nameMode)
    {
        if (_nameMode == NameMode.Hidden) return "?";
        if (_nameMode == NameMode.None) return "";
        return this.name;
    }
}
