using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class ButtonUI : MonoBehaviour
{
    public float scaleUpSize = 1.2f;  // Scale size when hovering
    public float scaleDownSize = 1f;  // Normal size
    public float moveDistance = 10f;  // Distance moved to the right
    public float exitDistance = 20f;  // Distance moved to the left for exit
    public float animationTime = 0.2f; // Animation duration
    public Ease easeType = Ease.OutBack; // Easing effect

    private Vector3 originalPosition;
    private float originalScale;

    private bool exiting = false;

    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private AudioClip[] hoverSound;
    [SerializeField] private AudioClip[] clickSound;

    private AudioSource audioSource;

    void Start()
    {
        originalPosition = transform.position; // Save initial position
        originalScale = transform.localScale.x;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer;

        audioSource.playOnAwake = false;

        RegisterClickEvent();
    }

    // Scale up (hover effect)
    public void ScaleUp()
    {
        if (exiting) return;

        transform.DOScale(originalScale * scaleUpSize, animationTime).SetEase(Ease.OutQuad);

        if (hoverSound.Length == 0) return;
        AudioClip clip = hoverSound[Random.Range(0, hoverSound.Length)];
        audioSource.volume = 0.6f;
        audioSource.clip = clip;
        audioSource.Play();

        
    }

    // Scale down (return to normal size)
    public void ScaleDown()
    {
        if (exiting) return;

        transform.DOScale(originalScale * scaleDownSize, animationTime).SetEase(Ease.InQuad);

        //if (hoverSound.Length == 0) return;
        //AudioClip clip = hoverSound[Random.Range(0, hoverSound.Length)];
        //audioSource.clip = clip;
        //audioSource.Play();
    }

    // Move slightly to the right
    public void MoveRight()
    {
        if (exiting) return;
        transform.DOMoveX(originalPosition.x + moveDistance, animationTime).SetEase(easeType);
    }

    // Move back to the original position
    public void ResetPosition()
    {
        if (exiting) return;
        transform.DOMove(originalPosition, animationTime).SetEase(easeType);
    }

    // Click animation (scales down slightly then back up)
    public void ClickEffect()
    {
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(transform.DOScale(originalScale * 0.9f, 0.1f).SetEase(Ease.InQuad));
        clickSequence.Append(transform.DOScale(originalScale * scaleDownSize, 0.15f).SetEase(Ease.OutBack));

        if (clickSound.Length == 0) return;
        AudioClip clip = clickSound[Random.Range(0, clickSound.Length)];

        if (!audioSource.enabled || !audioSource.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("AudioSource disabled — handling fallback.");

            HandleDisabledAudioSource(clip);

            return;
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
            
    }

    public void HandleDisabledAudioSource(AudioClip clip)
    {         // Fallback: Create a temporary AudioSource to play the clip
        GameObject tempAudioSourceObj = new GameObject("TempAudioSource");
        AudioSource tempAudioSource = tempAudioSourceObj.AddComponent<AudioSource>();
        tempAudioSource.outputAudioMixerGroup = audioMixer;
        tempAudioSource.clip = clip;
        tempAudioSource.Play();
        tempAudioSource.playOnAwake = false;
        // Destroy the temporary AudioSource after the clip finishes playing
        Destroy(tempAudioSourceObj, clip.length);
    }


    // **Exit animation (moves far left)**
    public void ExitLeft()
    {

        exiting = true;
        transform.DOMoveX(originalPosition.x - exitDistance, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => exiting = false); // Deactivate after animation
    }

    public void RestartGame()
    {
        //AudioManager.Instance.StopAllLoopingSounds();
        //AudioManager.Instance.StopAllOneShots();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // **Exit the game (works in both Editor and Build)**
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Editor
#else
            Application.Quit(); // Quit game in build
#endif
    }

    void RegisterClickEvent()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null) return;

        // Check if already registered (avoid duplicates)
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerClick)
            {
                // Already has a pointer click — don't double add
                return;
            }
        }

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((eventData) => ClickEffect());

        trigger.triggers.Add(clickEntry);
    }

}