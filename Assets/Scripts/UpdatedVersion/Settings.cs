using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes;

public static class LocalizationEvents
{
    public static event System.Action OnLanguageChanged;

    public static void NotifyLanguageChanged()
    {
        OnLanguageChanged?.Invoke();
    }
}

public class Settings : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI unlockedWordsText;

    public static Settings instance;

    [Header("General Settings")]
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject aboutMenu;
    [SerializeField] private GameObject wordsMenu;
    [SerializeField] private GameObject popUpConfirmation;
    [SerializeField] private Image wordsMenuImage;
    [SerializeField] private Sprite wordsMenuOpenSprite;
    [SerializeField] private Sprite wordsMenuClosedSprite;
    [SerializeField] private Animator wordsMenuAnimator;

    [Header("Localization Settings")]
    public string startingLanguage = "English";
    private string currentLanguage; 
    public string csvFileName = "localization.csv";
    [SerializeField] private TMP_Dropdown languageDropdown;

    [SerializeField] public Scaler scalerBlock;

    private string csvContent;
    private Dictionary<string, Dictionary<string, string>> localizedData;

    [SerializeField] private AudioMixer music;
    [SerializeField] private AudioMixer sfx;

    [SerializeField] private Volume blurEffect;

    public bool jumpToFinal = false;

    [SerializeField] private AudioSource notebookOpenAudioSource;
    [SerializeField] private AudioClip[] notebookSound;


    public static string GetText(string key)
    {
        Debug.Log("Getting text for key: " + key + " in language: " + instance.currentLanguage);
        return instance.localizedData[instance.currentLanguage][key];
    }

    public static void UpdateUnlockedWordsDisplayGlobal()
    {
        instance.UpdateUnlockedWordsDisplay();
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if(gameMenu != null) gameMenu.SetActive(false);

        if(aboutMenu != null) aboutMenu.SetActive(false);
        
        if(wordsMenu != null) wordsMenu.SetActive(false);

        scalerBlock.transform.localScale = Vector3.zero;

        if (scene.name == "NewMainMenu")
        {
            
        }
        else
        {
            ResetBlur();
        }

        //if(scalerBlock != null && scene.name != "NewMainMenu")
        //{
        //    Debug.Log("Playing scalerBlock animation on scene load");
        //    scalerBlock.Play();
        //}
        //else
        //{
        //    Debug.Log(scalerBlock != null ? "In Main Menu scene" : "scalerBlock is null scene");
        //    if(scalerBlock != null)
        //        scalerBlock.transform.localScale = Vector3.zero;
        //    Debug.Log("scalerBlock is null or in Main Menu scene");
        //}

        //Debug.Log("Scene loaded: " + scene.name);

    }

    public void ScaleWordbook()
    {
        scalerBlock.Play();
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("Language"))
        {
            Debug.Log("Loading saved language: " + PlayerPrefs.GetString("Language"));
            currentLanguage = PlayerPrefs.GetString("Language");
        }
        else
            currentLanguage = startingLanguage;
        int width = Screen.width;
        int height = Screen.height;

        Debug.Log($"Resolution: {width} x {height}");


        StartCoroutine(LoadCSV());
    }

    private void Update()
    {

        
        // Chjeck if current scene is main menu
        if(SceneManager.GetActiveScene().name == "NewMainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && gameMenu.activeSelf)
            {
                Debug.Log("Escape pressed in Main Menu");
                ToggleSettingsMenu();
            }
               

            return;
        }

        // This is just to demonstrate that the localization works
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TAG_CREDITS credits = FindAnyObjectByType<TAG_CREDITS>();
            if (credits == null)
                ToggleMenu();
            else if(credits != null && !credits.gameObject.activeSelf)
                ToggleMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleWordsMenu();
        }
    }

    public void TogglePopUp()
    {
        popUpConfirmation.SetActive(!popUpConfirmation.activeSelf);
    }

    public void ToggleMenuAndSoundMenu()
    {
        if (!(SceneManager.GetActiveScene().name == "NewMainMenu"))
            mainMenu.SetActive(!mainMenu.activeSelf);
        else
        {
            gameMenu.SetActive(false);
            mainMenu.SetActive(false);
        }
                        
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void ToggleSettingsMenu()
    {
        popUpConfirmation.SetActive(false);
        settingsMenu.SetActive(true);
        gameMenu.SetActive(!gameMenu.activeSelf);
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        wordsMenu.SetActive(false);
    }

    public void ToggleMenu()
    {

        popUpConfirmation.SetActive(false);
        gameMenu.SetActive(!gameMenu.activeSelf);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        aboutMenu.SetActive(false);
        wordsMenu.SetActive(false);
    }


    public void ToggleWordsMenu()
    {
        wordsMenu.SetActive(!wordsMenu.activeSelf);

        Material mat = wordsMenuImage.material; // creates an instance
        

        if (wordsMenu.activeSelf)
        {
            mat.SetFloat("_OutlineEnabled", 1f);
        }
        else
        {
            mat.SetFloat("_OutlineEnabled", 0f);
        }

        notebookOpenAudioSource.clip = (notebookSound[UnityEngine.Random.Range(0, notebookSound.Length)]);
        notebookOpenAudioSource.Play();

        wordsMenuAnimator.SetBool("Writing", false);
    }

    [Button]
    public void TurnBlurOff()
    {
        DepthOfField dof;
        // Tween over Focal Length variable of the Depth of Field effect
        blurEffect.profile.TryGet(out dof);

        // Tween focal length from 10 to 60 over 2 seconds
        DOTween.To(
            () => dof.focalLength.value,
            x => dof.focalLength.value = x,
            0f,
            2f
        );
    }

    [Button]
    public void TurnBlurOn()
    {
        DepthOfField dof;
        // Tween over Focal Length variable of the Depth of Field effect
        blurEffect.profile.TryGet(out dof);

        // Tween focal length from 10 to 60 over 2 seconds
        DOTween.To(
            () => dof.focalLength.value,
            x => dof.focalLength.value = x,
            300f,
            2f
        );
    }

    public void ResetBlur()
    {
        Debug.Log("Resetting blur effect");
        DepthOfField dof;
        // Tween over Focal Length variable of the Depth of Field effect
        blurEffect.profile.TryGet(out dof);

        dof.focalLength.value = 300f;
    }

    IEnumerator LoadCSV()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (File.Exists(filePath))
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    csvContent = reader.ReadToEnd();
                }

                Debug.Log("Loaded CSV:\n" + csvContent);
            }
            catch (IOException ex)
            {
                Debug.LogError("Error reading CSV: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }

        yield return null;

        ParseDict();
    }


    private void ParseDict()
    {
        localizedData = new Dictionary<string, Dictionary<string, string>>();

        string[] lines = csvContent.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return; // need header + at least 1 row

        // --- First line is the header ---
        string[] headers = lines[0].Split(',');
        // headers[0] == "word", headers[1] == "Portuguese", headers[2] == "English"

        // Create dictionary entry for each language column
        for (int i = 1; i < headers.Length; i++)
        {
            string lang = headers[i].Trim();
            if (!localizedData.ContainsKey(lang))
            {
                localizedData[lang] = new Dictionary<string, string>();
            }
        }

        // --- Now process each row ---
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = SplitCsvLine(lines[i]); // custom split to handle quotes
            if (parts.Length != headers.Length) continue;

            string key = parts[0].Trim();

            for (int j = 1; j < headers.Length; j++)
            {
                string lang = headers[j].Trim();
                string value = parts[j].Trim().Trim('"');
                value = value.Replace("\\n", "\n");
                localizedData[lang][key] = value;
            }
        }

        Debug.Log("Dictionary built!");
        Debug.Log("Portuguese[right] = " + localizedData["Português"]["right"]);
        Debug.Log("English[right] = " + localizedData["English"]["right"]);
        //Debug.Log("Spanish[right] = " + localizedData["Espanol"]["right"]);

        InitiateDropdown();
    }

    // Handles CSV lines with quotes (like tutorial_direita row)
    private string[] SplitCsvLine(string line)
    {
        var values = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        values.Add(current);
        return values.ToArray();
    }

    private void InitiateDropdown()
    {
        languageDropdown.ClearOptions();

        // Create a list of strings
        List<string> options = new List<string>(localizedData.Keys.ToList());

        //options.Remove("Espanol"); // Remove Spanish option
        //options.Remove("Francais"); // Remove French option)

        // Add them to the dropdown
        languageDropdown.AddOptions(options);

        languageDropdown.onValueChanged.AddListener(ChangeLanguage);

        if(PlayerPrefs.HasKey("Language"))
        {
            string savedLang = PlayerPrefs.GetString("Language");
            int index = options.IndexOf(savedLang);
            if(index >= 0)
            {
                languageDropdown.value = index;
                ChangeLanguage(index);

            }
        }
        else
        {
            languageDropdown.value = 0; // Set default value to English (index 1)

            ChangeLanguage(0);// Set default language to English (index 1)
        }


        UpdateUnlockedWordsDisplay();
    }

    public void ChangeLanguage(int index)
    {
        Debug.Log("Changing language to index: " + index);
        Debug.Log(languageDropdown.options[index].text);

        currentLanguage = languageDropdown.options[index].text;
        LocalizationEvents.NotifyLanguageChanged();

        //Save language on playerprefs
        PlayerPrefs.SetString("Language", currentLanguage);

        UpdateUnlockedWordsDisplay();
    }

    public void UpdateUnlockedWordsDisplay()
    {
        if (unlockedWordsText == null) return;

        string data = PlayerPrefs.GetString("UnlockedActions", "");
        if (string.IsNullOrEmpty(data))
        {
            unlockedWordsText.text = "\n";
            return;
        }

        string[] actions = data.Split(',');

        // Get localized text for each unlocked action
        var localizedWords = new List<string>();
        foreach (string action in actions)
        {
            if (string.IsNullOrEmpty(action)) continue;
            if(action == ActionWord.ok.ToString()) continue; // Skip "ok" action

            try
            {
                localizedWords.Add(GetText(action));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Could not get localized text for {action}: {ex.Message}");
            }
        }

        unlockedWordsText.text = "\n" + string.Join("\n", localizedWords);
    }

}
