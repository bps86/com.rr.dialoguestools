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
    public List<string> actorName = new List<string>();
    public List<string> expression = new List<string>();
    public List<Vector3> endPos = new List<Vector3>();
    public List<Vector3> endScale = new List<Vector3>();
    public List<Vector3> startPos = new List<Vector3>();
    public List<Vector3> startScale = new List<Vector3>();
}