using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private AudioClip[] hoverSound;
    [SerializeField] private AudioClip[] clickSound;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer;
    }

    public void PlayHoverSound()
    {
        
    }

    public void PlayClickSound()
    {
        
    }
}
