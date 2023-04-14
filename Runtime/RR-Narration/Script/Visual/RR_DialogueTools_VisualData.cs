using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RR_DialogueTools_VisualData
{
    public string tags;
    public int index;
    public float transitionDuration;
    public List<string> actorName;
    public List<string> expression;
    public List<bool> isLooping;
    public List<Vector3> endPos;
    public List<Vector3> endScale;
    public List<Vector3> startPos;
    public List<Vector3> startScale;

    public RR_DialogueTools_VisualData() {
        this.actorName = new List<string>();
        this.expression = new List<string>();
        this.isLooping = new List<bool>();
        this.endPos = new List<Vector3>();
        this.endScale = new List<Vector3>();
        this.startPos = new List<Vector3>();
        this.startScale = new List<Vector3>();
    }
}