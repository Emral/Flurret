using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public AudioClip musicClip;

    public void Play(bool skipFade)
    {
        AudioManager.PlayMusic(musicClip, skipFade);
    }

    public void FadeOut()
    {
        AudioManager.FadeOut();
    }
}