using UnityEngine;
using UnityEngine.UI;

public class KeyboardInteractionDisabler : MonoBehaviour
{
    private Button targetButton; // Assign your Button component here

    void Start()
    {
        targetButton = GetComponent<Button>();
        // Call this function when you want to disable keyboard interaction
        DisableKeyboardNavigation(targetButton);
    }

    public void DisableKeyboardNavigation(Button button)
    {
        // 1. Get the current Navigation settings
        Navigation nav = button.navigation;

        // 2. Change the mode to None
        nav.mode = Navigation.Mode.None;

        // 3. Apply the modified navigation settings back to the button
        button.navigation = nav;

        Debug.Log($"Keyboard navigation disabled for {button.name}.");
    }

    public void EnableKeyboardNavigation(Button button)
    {
        // To re-enable, you would typically set it back to Automatic or Explicit
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.Automatic;
        button.navigation = nav;

        Debug.Log($"Keyboard navigation re-enabled for {button.name}.");
    }
}