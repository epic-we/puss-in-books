using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void OpenOptions()
    {
        Settings.instance.ToggleSettingsMenu();
    }
}
