using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCat : MonoBehaviour
{

    [SerializeField] private CatMovement catMovement;
    [SerializeField] private DeathReload death;

    [SerializeField] private bool dontStop;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        death.SetCheckpoint(transform);

        if(!dontStop)
            catMovement.Stop();

        //GetComponent<BoxCollider2D>().enabled = false;

        
    }
}
