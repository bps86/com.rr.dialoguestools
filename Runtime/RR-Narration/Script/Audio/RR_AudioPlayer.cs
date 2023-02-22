using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public static class RR_AudioPlayer
{
    public static void PlayBeep(AudioSource audioSource, float minPitch = -0.2f, float maxPitch = 0.2f)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}