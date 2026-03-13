using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListenForWord : MonoBehaviour
{

    [SerializeField] private WriteToUI writeToUI;
    [SerializeField] private BoxCollider2D boxCollider;

    private string word;

    private TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        word = textMeshProUGUI.text;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(writeToUI.GetWord());

        if (writeToUI.GetWord().ToLower().Replace("_", " ") == word)
        {
            boxCollider.enabled = true;

            textMeshProUGUI.color = Color.black;

            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }
}
