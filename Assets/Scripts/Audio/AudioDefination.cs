using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEvetSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
            PlaAudioClip();
    }

    public void PlaAudioClip()
    {
        playAudioEvent.RaiseEvent(audioClip);
    }
}
