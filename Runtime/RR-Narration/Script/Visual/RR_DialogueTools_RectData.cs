using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RR_DialogueTools_RectData
{
    public Vector3 ActorPivot;
    public Vector3 ActorScale;

    private const int X_POSITION_INDEX = 0;
    private const int Y_POSITION_INDEX = 1;
    private const int Z_POSITION_INDEX = 2;
    private const int X_SCALE_INDEX = 3;
    private const int Y_SCALE_INDEX = 4;
    private const int Z_SCALE_INDEX = 5;

    public RR_DialogueTools_RectData(string[] rectData = null) {
        if (rectData != null) {
            Debug.Log("Loading rectData with length " + rectData.Length);
            ActorPivot = new Vector3(
                x: float.Parse(rectData[X_POSITION_INDEX]),
                y: float.Parse(rectData[Y_POSITION_INDEX]),
                z: float.Parse(rectData[Z_POSITION_INDEX])
            );
            ActorScale = new Vector3(
                x: float.Parse(rectData[X_SCALE_INDEX]),
                y: float.Parse(rectData[Y_SCALE_INDEX]),
                z: float.Parse(rectData[Z_SCALE_INDEX])
            );
        } else {
            Debug.LogError("Failed to load rectData, loading default pivot");
            ActorPivot = Vector3.zero;
            ActorScale = Vector3.one;
        }
    }
}
