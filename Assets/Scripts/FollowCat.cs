using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCat : MonoBehaviour
{

    [SerializeField] private Transform cat;
    private bool following;

    // Start is called before the first frame update
    void Start()
    {
        following = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
            transform.position = new Vector3 (cat.position.x + 7f, cat.position.y, -10f);
    }

    public void Follow() => following = true;   
}
