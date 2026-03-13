using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevel : MonoBehaviour
{

    [SerializeField] private MoveCamera moveCamera;

    [SerializeField] private float target;

    [SerializeField] private float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GetComponent<BoxCollider2D>().enabled = false;

        moveCamera.SetMoving(target, cameraSpeed);
    }
}
