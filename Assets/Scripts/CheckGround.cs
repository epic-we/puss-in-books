using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{

    [SerializeField] private LayerMask floorLayer;

    [SerializeField] private LayerMask movingPlatformLayer;

    [SerializeField] private CatMovement catMovement;

    [SerializeField] private GameObject cat;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int x = 1 << collision.gameObject.layer;


        Debug.Log(collision.gameObject.name);
        // Trigger Clown Falling
        if (x == floorLayer.value || x == movingPlatformLayer)
        {
            catMovement.SetJump(true);

            if(x == movingPlatformLayer)
            {
                Debug.Log("moving platform");
                cat.transform.SetParent(collision.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cat.transform.SetParent(null);
    }
}
