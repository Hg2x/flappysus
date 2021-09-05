using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public Sprite pipeHeadSprite;
    public Transform pfPipeHead;
    public Transform pfPipeBody;
    public SoundAudioClip[] soundAudioClipArray;
    
    private static GameAssets instance;
    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.SoundType sound;
        public AudioClip audioClip;
    }
}
