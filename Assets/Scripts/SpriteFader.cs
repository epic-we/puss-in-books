using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class SpriteFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private bool fadeOnStart = true;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private AudioClip fadeIn;
    [SerializeField] private AudioClip fadeOut;

    private AudioSource audioSource;

    private void Awake()
    {
        // Get SpriteRenderer on this object
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f); // Ensure starting alpha is 1

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.2f;
        audioSource.outputAudioMixerGroup = audioMixer;
    }

    private void Start()
    {
        if (fadeOnStart)
            FadeOut();
    }

    public void FadeOut()
    {
        if (fadeOut != null)
            {
            audioSource.clip = fadeOut;
            audioSource.Play();
        }
        if (spriteRenderer != null)
        {
            // Fade alpha to 0
            spriteRenderer.DOFade(0f, fadeDuration);
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name);
        }
    }

    public void FadeIn()
    {
        if (fadeIn != null)
        {
            audioSource.clip = fadeIn;
            audioSource.Play();
        }
        if (spriteRenderer != null)
        {
            // Fade alpha to 1
            spriteRenderer.DOFade(1f, fadeDuration);
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name);
        }
    }
}
