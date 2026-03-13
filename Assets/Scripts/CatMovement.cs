using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class CatMovement : MonoBehaviour
{

    private Vector2 velocity;

    [SerializeField] private float speed;

    [SerializeField] private float jumpForce;
    private Vector2 jumpVector;

    private Rigidbody2D rb;

    private Animator animator;

    private bool jump;

    [SerializeField] private Vector2 randomTimeBetween;
    private float justLicked;
    private float timeToLick;


    [SerializeField] private AudioClip[] meow;
    private AudioSource audioSourceMeow;
    [SerializeField] private AudioMixerGroup meowMixer;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;

        jumpVector = new Vector2(0f, jumpForce);

        rb = GetComponent<Rigidbody2D>();

        timeToLick = Random.Range(randomTimeBetween.x, randomTimeBetween.y);

        animator = GetComponentInChildren<Animator>();

        audioSourceMeow = gameObject.AddComponent<AudioSource>();
        audioSourceMeow.outputAudioMixerGroup = meowMixer;
    }

    // Update is called once per frame
    void Update()
    {

        rb.velocity = new Vector2 (velocity.x, rb.velocity.y);

        animator.SetFloat("MovSpeed", Mathf.Abs(rb.velocity.x));

        if(Time.time - justLicked > timeToLick)
        {

            Lick();

        }

        FlipPlayer();
    }

    public void Right()
    {
        velocity.x = speed;   
    }

    public void Left()
    {
        velocity.x = -speed; 
    }

    public void Jump()
    {

        if(jump)
        { 
        
            rb.AddForce(jumpVector);

            animator.SetTrigger("Jumping");

            jump = false;
        }
    }

    public void Stop()
    {
        velocity.x = 0;

    }

    public void Meow()
    {
        audioSourceMeow.clip = meow[Random.Range(0, meow.Length)];

        audioSourceMeow.pitch = Random.Range(0.90f, 1.10f);

        audioSourceMeow.Play();
    }

    
    



private void FlipPlayer()
    {
        if (rb.velocity.x < 0 && transform.right.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        }
        else if (rb.velocity.x > 0 && transform.right.x < 0)
        {
            transform.rotation = Quaternion.identity;

        }
    }

    public void Lick()
    {
        if (rb.velocity.x == 0f && rb.velocity.y == 0f) animator.SetTrigger("Lick");

        timeToLick = Random.Range(randomTimeBetween.x, randomTimeBetween.y);
        justLicked = Time.time;

        Debug.Log(timeToLick);
    }

    public bool SetJump(bool toggle) => jump = toggle;
}
