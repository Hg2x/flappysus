using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundType
    {
        playerJump, Score, Lose, ButtonClick
    }
    public static void PlaySound(SoundType soundType)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(soundType));
        Destroy(gameObject, 1f);  // 1 second delay
    }

    private static AudioClip GetAudioClip (SoundType soundType)
    {
        foreach(GameAssets.SoundAudioClip soundClip in GameAssets.GetInstance().soundAudioClipArray)
        {
            if (soundClip.sound == soundType)
            {
                Debug.Log(soundType);
                return soundClip.audioClip;
            }
        }
        Debug.LogError("Sound " + soundType + " not found");
        return null;
    }
}
