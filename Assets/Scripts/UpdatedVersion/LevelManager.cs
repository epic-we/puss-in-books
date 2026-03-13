using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private ActionWord missingWord;
    [SerializeField] private int numberOfMissingLetters = 3;
    private List<int> missingLetters;
    private string missingWordLocalized;
    [SerializeField] private Transform[] letterPositions;
    [SerializeField] private LetterPickUp letterPrefab;
    [SerializeField] private Transform letterCollectTarget; // Optional target for letters to move towards
    [SerializeField] private Transform wordColletTarget;
    private Vector3 wordCollectTarget;
    private Animator noteBookAnimator;

    [SerializeField] private GameObject particleExplosion;

    private List<LetterPickUp> spawnedLetters = new List<LetterPickUp>();

    private int lettersCollected = 0;

    [SerializeField] private Color letterColor = Color.black;

    [SerializeField] private string currentLevelName;

    [SerializeField] private TextMeshProUGUI wordMissingUI;
    [SerializeField] private TextMeshProUGUI wordMissingUIBackground;

    [SerializeField] private Scaler[] scalers;

    [SerializeField] private float letterScale = 1f;

    [SerializeField] private UnityEvent onCollectingFirstLetter;

    private WriteToUI writeToUI;

    [SerializeField] private ActionPoint finalActionPoint;
    [SerializeField] private GameObject initialStory;


    [SerializeField] private AudioSource notebookWriting;
    [SerializeField] private AudioSource wordSliding;
    [SerializeField] private AudioSource wordFinish;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Settings.instance.ResetBlur();
        yield return new WaitForSeconds(0.1f); // Wait for Settings to initialize

        writeToUI = FindAnyObjectByType<WriteToUI>();
        wordCollectTarget = Camera.main.ScreenToWorldPoint(FindAnyObjectByType<TAG_WORDS>().transform.position);
        noteBookAnimator = FindAnyObjectByType<TAG_WORDS>().GetComponent<Animator>();

        UnlockLevel(currentLevelName);



        if (letterPositions.Length < numberOfMissingLetters)
        {
            Debug.LogError("Not enough letter positions for the number of missing letters.");
            yield break;
        }

        for (int i = 0; i < numberOfMissingLetters; i++)
        {
            Transform pos = letterPositions[i];
            LetterPickUp letterObj = Instantiate(letterPrefab, pos.position, Quaternion.identity);
            letterObj.text.color = letterColor;
            letterObj.transform.localScale *= letterScale;
            LetterPickUp letterPickUp = letterObj.GetComponent<LetterPickUp>();

            spawnedLetters.Add(letterObj);

        }

        UpdateLetters();

        LocalizationEvents.OnLanguageChanged += UpdateLetters;

        if (Settings.instance.jumpToFinal && finalActionPoint != null)
        {
            CatState catState = FindObjectOfType<CatState>();
            catState.StartAtActionPoint(finalActionPoint);

            if (initialStory != null)
            {
                initialStory.SetActive(false);
            }

            Settings.instance.TurnBlurOff();

            Settings.instance.jumpToFinal = false;

            LevelReady();
        }
    }

    [ContextMenu("Level Ready")]
    public void LevelReady()
    {
        writeToUI.ReadyToReveal();
        writeToUI.ShowUI();

        foreach ( LetterPickUp letter in spawnedLetters)
        {
            letter.GetComponent<Scaler>().Play();
        }

        foreach (Scaler s in scalers)
        {
            s.Play();
        }

        Settings.instance.ScaleWordbook();
        Settings.instance.TurnBlurOff();
    }

    private void UpdateLetters()
    {

        missingLetters = new List<int>();

        string missingWordStr = Settings.GetText(missingWord.ToString());
        missingWordLocalized = missingWordStr;
        wordMissingUI.text = missingWordStr;
        wordMissingUIBackground.text = missingWordStr;
        List<int> availableIndices = Enumerable.Range(0, missingWordStr.Length).ToList();

        // Shuffle indices
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int swapIndex = Random.Range(i, availableIndices.Count);
            (availableIndices[i], availableIndices[swapIndex]) = (availableIndices[swapIndex], availableIndices[i]);
        }

        
        for (int i = 0; i < spawnedLetters.Count; i++)
        {
            if(spawnedLetters[i] == null)
            {
                Debug.LogError("Spawned letter is null");
                continue;
            }
            if (spawnedLetters[i].gameObject.activeSelf)
            {
                // Use shuffled indices instead of random.Range
                char randomLetter = missingWordStr[availableIndices[i]];
                spawnedLetters[i].Instantiate(missingWord, availableIndices[i], this, letterCollectTarget);
                missingLetters.Add(availableIndices[i]);

                wordMissingUI.text = ReplaceLetter(wordMissingUI.text, availableIndices[i], '_');
            }
        }
    }

    private string ReplaceLetter(string word, int index, char c)
    {
        Debug.Log($"Try replace {c} letter ({index}) in word {word}"); 
        return word.Substring(0, index) + c + word.Substring(index + 1);
    }

    public void CollectLetter(int index)
    {

        if(lettersCollected == 0)
        {
            onCollectingFirstLetter.Invoke();
        }

        if (missingLetters.Contains(index))
        {
            lettersCollected++;
            wordMissingUI.text = ReplaceLetter(wordMissingUI.text, index, missingWordLocalized[index]);
            missingLetters.Remove(index);
            PopUpAndDown(letterCollectTarget.GetComponent<RectTransform>());
        }
        else
        {
            Debug.Log($"Letter {index} is not part of the missing word.");
        }

        if (lettersCollected >= numberOfMissingLetters)
        {
            Debug.Log($"All letters collected! You can now perform the action: {missingWord}");
            
            CatState catState = FindObjectOfType<CatState>();

            wordFinish.Play();

            ScaleDownAndDeactivateWord(letterCollectTarget.GetComponent<RectTransform>(), 2f);

            if (catState != null) catState.UnlockAction(missingWord);

            Settings.UpdateUnlockedWordsDisplayGlobal();
        }
    }

    private void PopUpAndDown(RectTransform target)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 originalScale = target.localScale;
        // POP UP
        seq.Append(
            target.DOScale(originalScale * 1.2f, 0.2f)
                  .SetEase(Ease.OutBack)
        );
        // small settle back to normal scale
        seq.Append(
            target.DOScale(originalScale, 0.1f)
        );

    }

    private void ScaleDownAndDeactivateWord(RectTransform target, float duration)
    {
        Sequence seq = DOTween.Sequence();

        Vector3 originalScale = target.localScale;

        // POP UP
        seq.Append(
            target.DOScale(originalScale * 1.2f, 0.2f)
                  .SetEase(Ease.OutBack)
        );

        // Change text color to gold (if TMP exists)
        seq.AppendCallback(() =>
        {

            if (wordMissingUI != null)
            {
                wordMissingUI.color = new Color(1f, 0.5f, 0f, 1f); // gold
                //wordMissingUIBackground.gameObject.SetActive(true);
            }

            // Instantiate particles at popup moment
            var particles = Instantiate(particleExplosion);
            particles.transform.position = target.position;
        });

        // small settle back to normal scale
        seq.Append(
            target.DOScale(originalScale, 0.1f)
        );

        // wait before disappearing
        seq.AppendInterval(2f);

        // SCALE DOWN
        seq.Append(
            target.DOScale(Vector3.zero, duration)
                  .SetEase(Ease.InBack)
        ).Join(
            target.DOMove(wordColletTarget.position, duration).SetEase(Ease.InBack).OnStart(() =>
            {
                wordSliding.Play();

            })
        );

        // FINALIZE
        seq.OnComplete(() =>
        {
            target.gameObject.SetActive(false);
            notebookWriting.Play();
            noteBookAnimator.SetBool("Writing", true);
        });
    }

    /// <summary>
    /// Unlocks a new level and saves it in PlayerPrefs
    /// </summary>
    public void UnlockLevel(string levelName)
    {
        string unlocked = PlayerPrefs.GetString("UnlockedLevels", "");

        if (!unlocked.Contains(levelName))
        {
            unlocked += levelName + ";"; // Add separator
            PlayerPrefs.SetString("UnlockedLevels", unlocked);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked: {levelName}");
        }
    }

    public void BlurScreen()
    {
        Settings.instance.TurnBlurOn();
    }

    public void BlurScreenOff()
    {
        Settings.instance.TurnBlurOff();
    }

    public void ScaleUIDown()
    {
        foreach(Scaler s in scalers)
        {
            s.ScaleDown();
        }
    }

    public void ScaleUIUp()
    {
        foreach (Scaler s in scalers)
        {
            s.Play();
        }
    }
}
