using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class LevelPicker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer whiteBG;
    [SerializeField] private SpriteRenderer twirl;

    [SerializeField] private GameObject levelPicker;

    [SerializeField] private TypingHighlighter levelWordPrefab;

    private List<TypingHighlighter> typingHighlighters;

    [SerializeField] private WriteToUI writeToUI;

    [SerializeField] private Vector2 startingLevelPos = new Vector2(-275f, 180f);
    [SerializeField] private float levelSpacingY = 60f;
    [SerializeField] private float levelSpacingX = 250f;
    private int numberOfLevelsSpawned = 0;

    // Dictionary: Key = Level name, Value = Scene name
    private Dictionary<string, string> allLevels = new Dictionary<string, string>
    {
        { "Floresta" , "Chapter1" },
        { "Porta Traicao 1.1" , "Chapter1" },
        { "Castelo" , "Chapter2" },
        { "Quarto Princesa" , "Chapter3" },
    };

    private const string unlockedKey = "UnlockedLevels";

    private void Start()
    {
        // Initialize unlocked levels if not set yet
        if (!PlayerPrefs.HasKey(unlockedKey))
        {
            // Unlock the first defined level
            string firstLevelName = new List<string>(allLevels.Keys)[0];
            UnlockLevel(firstLevelName);
        }
        FindAnyObjectByType<CatState>().ResetGlossary();
    }

    public void ResetLevels()
    {
        PlayerPrefs.DeleteKey(unlockedKey);
        PlayerPrefs.Save();
        Debug.Log("Reset unlocked levels.");

        // Initialize unlocked levels if not set yet
        if (!PlayerPrefs.HasKey(unlockedKey))
        {
            // Unlock the first defined level
            string firstLevelName = new List<string>(allLevels.Keys)[0];
            UnlockLevel(firstLevelName);

        }
    }

    /// <summary>
    /// Unlocks a new level and saves it in PlayerPrefs
    /// </summary>
    public void UnlockLevel(string levelName)
    {
        string unlocked = PlayerPrefs.GetString(unlockedKey, "");

        if (!unlocked.Contains(levelName))
        {
            unlocked += levelName + ";"; // Add separator
            PlayerPrefs.SetString(unlockedKey, unlocked);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked: {levelName}");
        }
    }

    /// <summary>
    /// Returns a list of unlocked levels
    /// </summary>
    public List<string> GetUnlockedLevels()
    {
        string unlocked = PlayerPrefs.GetString(unlockedKey, "");
        List<string> unlockedLevels = new List<string>(unlocked.Split(';'));

        // Remove empty entries
        unlockedLevels.RemoveAll(string.IsNullOrEmpty);
        return unlockedLevels;
    }

    /// <summary>
    /// Checks if a level is unlocked
    /// </summary>
    public bool IsLevelUnlocked(string levelName)
    {
        return GetUnlockedLevels().Contains(levelName);
    }

    public void OpenMenu()
    {
        writeToUI.ClearWord();
        writeToUI.enabled = false;

        // Fade in background + twirl, then show menu
        whiteBG.DOFade(1f, 2f);
        twirl.DOFade(1f, 2f).OnComplete(() =>
        {
            levelPicker.SetActive(true);
        });

        numberOfLevelsSpawned = 0;

        typingHighlighters = new List<TypingHighlighter>();
        foreach (var levelName in GetUnlockedLevels())
        {
            if (allLevels.TryGetValue(levelName, out string sceneName))
            {
                var highlighter = Instantiate(levelWordPrefab, levelPicker.transform);
                highlighter.Initialize(levelName, allLevels[levelName], this); 
                
                float posX = startingLevelPos.x + (int)(numberOfLevelsSpawned / 6) * levelSpacingX;
                float posY = startingLevelPos.y - (numberOfLevelsSpawned % 6) * levelSpacingY;
                highlighter.transform.localPosition = new Vector3(posX, posY, 0f);
                numberOfLevelsSpawned++;

                typingHighlighters.Add(highlighter);
            }
            else
            {
                Debug.LogWarning($"Unlocked level '{levelName}' not found in allLevels dictionary.");
            }
        }
    }

    public void CloseMenu()
    {
        writeToUI.enabled = true;

        foreach (var highlighter in typingHighlighters)
        {
            Destroy(highlighter.gameObject);
        }
        typingHighlighters.Clear();

        levelPicker.SetActive(false);

        whiteBG.DOFade(0f, 2f);
        twirl.DOFade(0f, 2f);
    }

    public void AnimateDown(string sceneName)
    {
        levelPicker.transform.DOLocalMoveY(-800f, 1f).SetEase(Ease.InBack).OnComplete(() => { SceneManager.LoadScene(sceneName); });
    }
}
