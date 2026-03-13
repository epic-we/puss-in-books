using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TypeSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] type;
    private AudioSource audioSourceType;
    [SerializeField] private AudioMixerGroup typeMixer;

    [SerializeField] private AudioClip[] ding;
    private AudioSource audioSourceDing;
    [SerializeField] private AudioMixerGroup dingMixer;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceType = gameObject.AddComponent<AudioSource>();
        audioSourceType.outputAudioMixerGroup = typeMixer;

        audioSourceDing = gameObject.AddComponent<AudioSource>();
        audioSourceDing.outputAudioMixerGroup = dingMixer;
    }

    public void PlayTypeSound()
    {
        audioSourceType.clip = type[Random.Range(0, type.Length)];

        audioSourceType.pitch = Random.Range(0.90f, 1.10f);

        audioSourceType.Play();
    }

    public void PlayDingSound()
    {
        //audioSourceDing.clip = ding[Random.Range(0, ding.Length)];

        //audioSourceDing.pitch = Random.Range(0.90f, 1.10f);

        //audioSourceDing.Play();
    }
}
