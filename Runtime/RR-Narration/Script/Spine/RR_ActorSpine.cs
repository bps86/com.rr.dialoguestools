using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RR_ActorSpine
{
    public string name;
    public SkeletonDataAsset skeletonDataAsset;
    public RR_ActorSpine(string _name, string path) {
        this.name = _name;
        this.skeletonDataAsset = Resources.Load<SkeletonDataAsset>(path);
    }
}
