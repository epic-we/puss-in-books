using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    // This method is called when the object is instantiated
    private void Awake()
    {
        // Check if another instance of this object already exists
        if (FindObjectsOfType<DontDestroyOnLoadScript>().Length > 1)
        {
            Destroy(gameObject); // Destroy the duplicate instance
            return;
        }

        // Make this object persistent across scenes
        DontDestroyOnLoad(gameObject);
    }
}

