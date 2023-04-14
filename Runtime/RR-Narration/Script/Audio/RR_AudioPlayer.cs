using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RR_AudioPlayer
{
    public static void PlayBeep(AudioSource audioSource, float minPitch = -0.2f, float maxPitch = 0.2f) {
        if (audioSource.clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public static void PlaySfx(string sfxName, bool isLoop, AudioSource audioSource, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        if (System.String.IsNullOrEmpty(sfxName)) return;

        audioSource.Stop();
        audioSource.clip = rR_DialogueTools_AssetManager.GetSfx(sfxName);
        audioSource.loop = isLoop;
        audioSource.Play();
    }

    public static void PlayBgm(string bgmName, bool isLoop, AudioSource audioSource, RR_DialogueTools_AssetManager rR_DialogueTools_AssetManager) {
        if (System.String.IsNullOrEmpty(bgmName)) return;

        if (audioSource.clip == null || bgmName != audioSource.clip.name) {
            audioSource.clip = rR_DialogueTools_AssetManager.GetBgm(bgmName);
            audioSource.loop = isLoop;
            audioSource.Play();
        }
    }
}