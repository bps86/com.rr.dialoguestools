using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RR_NarrationVisualData
{
    public string tags;
    public int index;
    public List<string> actor = new List<string>();
    public List<Vector3> pos = new List<Vector3>();
    public List<Vector3> scale = new List<Vector3>();
    public List<Vector3> endpos = new List<Vector3>();
    public List<Vector3> endscale = new List<Vector3>();
    public List<float> lerpDuration = new List<float>();
}