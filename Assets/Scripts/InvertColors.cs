using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertColors : MonoBehaviour
{

    [SerializeField] private Material[] material;

    // Start asdasdis called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame  
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Invert();
        }
    }

    public void Invert()
    {
        foreach (Material mat in material)
        {
            if(mat.GetFloat("_Threshold") == 0)
                mat.SetFloat("_Threshold", 1);
            else
                mat.SetFloat("_Threshold", 0);
        }
    }
}
