using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CheckCatThrough : MonoBehaviour
{
    [SerializeField] private GameObject nextPlatform;
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        nextPlatform.SetActive(true);
    }
}
