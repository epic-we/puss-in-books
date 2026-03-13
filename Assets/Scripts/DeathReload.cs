using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathReload : MonoBehaviour
{

    private Transform checkpoint;
    [SerializeField] private GameObject cat;

    [SerializeField] private GameObject[] objetcsToDeactivate;

    [SerializeField] private bool resetLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(GameObject go in objetcsToDeactivate)
        {
            go.SetActive(false);
        }

        if(resetLevel)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        { 
            cat.GetComponent<CatMovement>().Stop();
            cat.transform.position = checkpoint.position;
        }
    }

    public void SetCheckpoint(Transform check)
    {
        
        checkpoint = check;

        Debug.Log(checkpoint.position);
    }
}
