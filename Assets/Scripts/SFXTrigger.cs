using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXTrigger : MonoBehaviour
{
    public SFX soundEffect;

    private AudioSource _clipReference;

    public void Play()
    {
        _clipReference = AudioManager.PlaySFX(soundEffect);
    }

    public void Stop()
    {
        _clipReference.Stop();
    }
}