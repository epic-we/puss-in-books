using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] private GameObject stopA;
    [SerializeField] private GameObject stopB;

    [SerializeField] private float speed;
    private float speedX;
    private float speedY;

    private float distanceY;
    private float distanceX;



    // Start is called before the first frame update
    void Start()
    {
        
        distanceY = stopA.transform.position.y - stopB.transform.position.y;
        distanceX = stopA.transform.position.x - stopB.transform.position.x;

        //Debug.Log(distanceY);
        //Debug.Log(distanceX);

        distanceX = Mathf.Abs(distanceX);
        distanceY = Mathf.Abs(distanceY);

        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        speedX = speed * (distanceX / distance);
        speedY = speed * (distanceY / distance);

        Debug.Log("speedX: " + speedX);
        Debug.Log("speedY: " + speedY);
        Debug.Log("speed: " + speed);

        //float scale = 0;

        //if (distanceY != 0 && distanceX != 0)
        //    scale = distanceX / distanceY;

        //if(scale != 0)
        //    speedY = speed / scale;

        //else if(distanceY != 0)
        //{
        //    speedY = speed;
        //}

    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (distanceX > distanceY)
        {
            if (gameObject.transform.position.x > stopB.transform.position.x)
            {
                speedY *= -1;
                speedX *= -1;

            }

            if (gameObject.transform.position.x < stopA.transform.position.x)
            {
                speedY *= -1;
                speedX *= -1;
            }
        }
        else
        { 

            if (gameObject.transform.position.y > stopB.transform.position.y)
            {
                speedY *= -1;
                speedX *= -1;
            }

            if (gameObject.transform.position.y < stopA.transform.position.y)
            {
                speedY *= -1;
                speedX *= -1;
            }

        }

        gameObject.transform.position = new Vector2(gameObject.transform.position.x + speedX * Time.deltaTime, gameObject.transform.position.y - speedY * Time.deltaTime);
    }
}
