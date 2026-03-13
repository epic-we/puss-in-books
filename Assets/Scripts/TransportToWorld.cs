using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransportToWorld : MonoBehaviour
{

    [SerializeField] private string sceneName;


    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeScene();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {


        ChangeScene();



    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
