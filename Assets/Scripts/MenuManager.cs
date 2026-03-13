using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {

    }

    public void Exit()
    {
        Invoke(nameof(ExitGame), 0.1f);

    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
