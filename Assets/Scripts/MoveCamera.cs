using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour
{

    private bool moving;
    private float target;
    private float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Camera.main.transform.position.x > target && cameraSpeed > 0)
        {
            moving = false;
        }
        else if (Camera.main.transform.position.x < target && cameraSpeed < 0)
        {
            moving = false;
        }

        if (moving)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + cameraSpeed, Camera.main.transform.position.y, -10f);
            
        }
        
    }

    public void SetMoving(float xPos, float camSpeed)
    { 
        moving = true;
        target = xPos;
        cameraSpeed = camSpeed;
    }
    
}
