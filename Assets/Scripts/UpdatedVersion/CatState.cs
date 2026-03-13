using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class CatState : MonoBehaviour
{
    [SerializeField] private ActionPoint startingActionPoint;
    private ActionPoint currentActionPoint;

    private Animator animator;

    [SerializeField] private float speed = 5f;
    public bool moving;

    [SerializeField] private AnimationClip idleAnimation;

    private SpriteRenderer spriteRenderer;

    private List<ActionWord> unlockedActions;
    private const string PlayerPrefsKey = "UnlockedActions";

    [SerializeField] private GameObject confused;
    [SerializeField] private GameObject blocked;

    [SerializeField] private AudioSource blockedSource;
    [SerializeField] private AudioClip[] blockedClip;

    [SerializeField] private bool cheats = false;

    // Start is called before the first frame update
    void Start()
    {
        currentActionPoint = startingActionPoint;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.position = startingActionPoint.transform.position;

        unlockedActions = LoadUnlockedActions();



        // If no saved actions yet, initialize defaults
        if (unlockedActions.Count == 0)
        {
            unlockedActions.Add(ActionWord.right);
            unlockedActions.Add(ActionWord.left);
            //unlockedActions.Add(ActionWord.sleep);
            //unlockedActions.Add(ActionWord.awake);
            unlockedActions.Add(ActionWord.up);
            unlockedActions.Add(ActionWord.down);
            unlockedActions.Add(ActionWord.ok);

            SaveUnlockedActions(); // save defaults once
        }
    }

    public void StartAtActionPoint(ActionPoint actionPoint)
    {
        currentActionPoint = actionPoint;
        transform.position = actionPoint.transform.position;
    }

    public void UnlockAllWords()
    {
        UnlockAction(ActionWord.right);
        UnlockAction(ActionWord.left);
        UnlockAction(ActionWord.sleep);
        //UnlockAction(ActionWord.awake);
        UnlockAction(ActionWord.up);
        UnlockAction(ActionWord.down);
        UnlockAction(ActionWord.ok);
        UnlockAction(ActionWord.jump);
        UnlockAction(ActionWord.scratch);
        UnlockAction(ActionWord.meow);

        Settings.UpdateUnlockedWordsDisplayGlobal();

        SaveUnlockedActions();
    }

    public void ResetGlossary()
    {

        PlayerPrefs.DeleteKey(PlayerPrefsKey); // Uncomment to reset unlocked actions during testing

        unlockedActions = LoadUnlockedActions();

        // If no saved actions yet, initialize defaults
        if (unlockedActions.Count == 0)
        {
            unlockedActions.Add(ActionWord.right);
            unlockedActions.Add(ActionWord.left);
            //unlockedActions.Add(ActionWord.awake);
            unlockedActions.Add(ActionWord.up);
            unlockedActions.Add(ActionWord.down);
            unlockedActions.Add(ActionWord.ok);

            SaveUnlockedActions(); // save defaults once
        }

        Settings.UpdateUnlockedWordsDisplayGlobal();
    }

    private void Update()
    {

        if (!cheats)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
        {
            ResetGlossary();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            UnlockAllWords();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            ToggleUI();
        }
    }

    public void UnlockAction(ActionWord action)
    {
        if (!unlockedActions.Contains(action))
        {
            unlockedActions.Add(action);
            SaveUnlockedActions();
            Debug.Log("Unlocked new action: " + action);
        }
    }

    private void SaveUnlockedActions()
    {
        // Convert enum list to comma-separated string
        string data = string.Join(",", unlockedActions);
        PlayerPrefs.SetString(PlayerPrefsKey, data);
        PlayerPrefs.Save();
    }

    private List<ActionWord> LoadUnlockedActions()
    {
        List<ActionWord> result = new List<ActionWord>();

        string data = PlayerPrefs.GetString(PlayerPrefsKey, "");
        if (!string.IsNullOrEmpty(data))
        {
            string[] parts = data.Split(',');
            foreach (string part in parts)
            {
                if (Enum.TryParse(part, out ActionWord action))
                {
                    result.Add(action);
                }
            }
        }

        return result;
    }

    private bool uiOn = true;

    public void ToggleUI()
    {

        uiOn = !uiOn;

        Canvas[] canvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas c in canvas)
        {
            if (c.gameObject.activeSelf)
                c.enabled = uiOn;
        }

        int uiLayer = LayerMask.NameToLayer("UI");
        int uiMask = 1 << uiLayer;


        if (!uiOn)
        {
            Camera.main.cullingMask &= ~uiMask;
        }
        else
        {
            Camera.main.cullingMask |= uiMask;
        }
    }




    public void CheckInput(string input)
    {

        // Strip trailing digits from input
        string normalizedInput = StripTrailingDigits(input);

        normalizedInput.Replace("_", " "); // allow underscores as spaces

        Debug.Log("Normalized input: " + normalizedInput);

        if (cheats)
        {


            if (normalizedInput.Equals("hub", StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
            }

            if (normalizedInput.Equals("levelone", StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Chapter1");
                ResetGlossary();
                UnlockAction(ActionWord.sleep);
                Settings.UpdateUnlockedWordsDisplayGlobal();
            }

            if (normalizedInput.Equals("leveltwo", StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Chapter2");
                ResetGlossary();
                UnlockAction(ActionWord.sleep);
                UnlockAction(ActionWord.scratch);
                Settings.UpdateUnlockedWordsDisplayGlobal();
            }

            if (normalizedInput.Equals("levelthree", StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Chapter3");
                ResetGlossary();
                UnlockAction(ActionWord.sleep);
                UnlockAction(ActionWord.scratch);
                UnlockAction(ActionWord.jump);
                Settings.UpdateUnlockedWordsDisplayGlobal();
            }

            if (normalizedInput.Equals("levelthreefinal", StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Chapter3");
                Settings.instance.jumpToFinal = true;
                ResetGlossary();
                UnlockAction(ActionWord.sleep);
                UnlockAction(ActionWord.scratch);
                UnlockAction(ActionWord.jump);
                UnlockAction(ActionWord.meow);
                Settings.UpdateUnlockedWordsDisplayGlobal();
            }

        }

        if (normalizedInput.Equals(Settings.GetText("pause"), StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Pause command received.");
            FindAnyObjectByType<Settings>().ToggleMenu();
            return;
        }

        if (moving)
        {
            Debug.LogWarning("Cat is currently moving. Please wait until the action is completed.");
            return;
        }

        if (currentActionPoint == null)
        {
            Debug.LogWarning("No current action point set.");
            return;
        }

        //// Try parse normalized input
        //if (!Enum.TryParse(normalizedInput, true, out ActionWord actionWord))
        //{
        //    Debug.LogWarning("Invalid action word: " + normalizedInput);
        //    return;
        //}



        // Check if the input matches any of the possible actions in the current action point
        foreach (Decision action in currentActionPoint.PossibleActions)
        {
            string decisionName = StripTrailingDigits(action.action.ToString());

            if (Settings.GetText(decisionName).Equals(normalizedInput, StringComparison.OrdinalIgnoreCase)
                && action.active)
            {
                Debug.Log("Found interaction: " + decisionName);

                if (!unlockedActions.Contains((ActionWord)Enum.Parse(typeof(ActionWord), decisionName, true)))
                {
                    StartCoroutine(TurnOnObjectTimer(confused, 2f));
                    Debug.LogWarning("Action not unlocked: " + decisionName);
                    return;
                }

                if (confused.activeSelf)
                {
                    confused.SetActive(false);
                }

                if (blocked.activeSelf)
                {
                    blocked.SetActive(false);
                }

                // Perform the action
                PerformAction(action);
                return;
            }
        }

        StartCoroutine(TurnOnObjectTimer(blocked, 2f));
        Debug.Log("No matching action found for input: " + input);
    }

    private IEnumerator TurnOnObjectTimer(GameObject targetObject, float duration)
    {
        targetObject.SetActive(true);

        blockedSource.clip = blockedClip[UnityEngine.Random.Range(0, blockedClip.Length)];
        blockedSource.Play();

        yield return new WaitForSeconds(duration);
        targetObject.SetActive(false);
    }

    // Helper: removes any trailing digits from a string
    private string StripTrailingDigits(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        int i = s.Length - 1;
        while (i >= 0 && char.IsDigit(s[i])) i--;
        return s.Substring(0, i + 1);
    }



    public async void PerformAction(Decision action)
    {
        bool deactivatedActions = false;
        moving = true;

        action.onActivate.Invoke();

        for (int i = 0; i < action.path.Length; i++)
        {
            PathPoint point = action.path[i];
            Transform target = point.point;

            // --- Play animation for this path point ---
            if (point.animation != null)
                animator.Play(point.animation.name);

            // --- Horizontal facing via SpriteRenderer flip ---
            bool movingLeft = transform.position.x - target.position.x > 0;
            spriteRenderer.flipX = movingLeft;

            if (point.rotate)
            {
                // --- Head tilt (Z rotation) ---
                Vector3 direction = (target.position - transform.position).normalized;
                float angleZ = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
                if (movingLeft) angleZ = -angleZ;

                // Snap Z rotation instantly
                transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
            }
            else if (action.action == ActionWord.down && point.animation.name == "Climb")
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }

            // --- Move with speed (manual duration calculation) ---
            float distance = Vector3.Distance(transform.position, target.position);
            float duration = distance / Mathf.Max(0.01f, point.speed); // avoid division by zero

            Debug.Log($"Distance: {distance}, Speed: {point.speed}, Duration: {duration}, Position: {transform.position} -> {target.position}");


            await transform.DOMove(target.position, duration)
                           .SetEase(Ease.Linear)
                           .AsyncWaitForCompletion();

            // Reset rotation back to neutral

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // --- Invoke UnityEvent ---
            if (point.hasEvent && point.onReachPoint != null)
                point.onReachPoint.Invoke();

            // --- Activate dependent decisions ---
            if (point.activateDependentDecisions && action.dependentDecisions != null)
            {
                Debug.Log("Activating dependent decisions...");
                foreach (DependentDecision dep in action.dependentDecisions)
                {
                    Debug.Log("Activating action: " + dep.action + " on ActionPoint: " + (dep.actionPoint != null ? dep.actionPoint.name : "null"));
                    if (dep.actionPoint != null)
                        dep.actionPoint.ActivateInteraction(dep.action);
                }
            }

            // --- Deactivate decisions ---
            if (action.deactivateDecisions != null && !deactivatedActions)
            {
                Debug.Log("Deactivating decisions...");
                foreach (DependentDecision dep in action.deactivateDecisions)
                {
                    Debug.Log("Deactivating action: " + dep.action + " on ActionPoint: " + (dep.actionPoint != null ? dep.actionPoint.name : "null"));
                    if (dep.actionPoint != null)
                        dep.actionPoint.DeactivateInteraction(dep.action);
                }

                deactivatedActions = true;
            }
        }

        // Advance to next action point
        currentActionPoint = action.nextActionPoint;
        moving = false;

        // --- Play idle animation after movement ---
        if (idleAnimation != null)
            animator.Play(idleAnimation.name);
    }


}
