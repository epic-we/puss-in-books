using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows;

public class ReadInput : MonoBehaviour
{

    [SerializeField] private string[] wordsJump;
    [SerializeField] private string[] wordsRight;
    [SerializeField] private string[] wordsLeft;
    [SerializeField] private string[] wordsStop;
    [SerializeField] private string[] wordsLick;
    //[SerializeField] private string[] wordsHit;
    [SerializeField] private string[] wordsMeow;

    private CatMovement catMovement;

    // Start is called before the first frame update
    void Start()
    {
        catMovement = GetComponent<CatMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetInput(string input)
    {
        string[] words = input.Split("_");
        
        foreach (string word in words)
        {

            string wordNoCaps = word.ToLower();


            foreach (string wordJump in wordsJump)
            {
                if (!wordNoCaps.Except(wordJump).Any() && wordNoCaps.Length >= 4)
                {
                    catMovement.Jump();
                }
            }

            foreach (string wordRight in wordsRight)
            {
                if (!wordNoCaps.Except(wordRight).Any() && wordNoCaps.Length >= 5)
                {
                    catMovement.Right();
                }
            }

            foreach (string wordLeft in wordsLeft)
            {
                if (!wordNoCaps.Except(wordLeft).Any() && wordNoCaps.Length >= 4)
                {
                    catMovement.Left();
                }
            }

            foreach (string wordLick in wordsLick)
            {
                if (!wordNoCaps.Except(wordLick).Any() && wordNoCaps.Length >= 4)
                {
                    catMovement.Lick();
                }
            }

            foreach (string wordStop in wordsStop)
            {
                if (!wordNoCaps.Except(wordStop).Any() && wordNoCaps.Length >= 4)
                {
                    catMovement.Stop();
                }
            }

            foreach (string wordMeow in wordsMeow)
            {
                if (!wordNoCaps.Except(wordMeow).Any() && wordNoCaps.Length >= 4)
                {
                    catMovement.Meow();
                }
            }

        }
        

        
    }
}
