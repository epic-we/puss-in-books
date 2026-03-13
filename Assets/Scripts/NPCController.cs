using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{

    [SerializeField] private GameObject characterDefault;
    [SerializeField] private GameObject characterAnnoyed;
    [SerializeField] private GameObject characterStartled;
    [SerializeField] private GameObject characterAlert;

    [SerializeField] private AudioClip[] startledSoundsFemale;
    [SerializeField] private AudioClip[] startledSoundsMale;
    [SerializeField] private AudioSource audioSourceFemale;
    [SerializeField] private AudioSource audioSourceMale;

    public void DisableAllCharacters()
    {
        characterDefault.SetActive(false);
        characterAnnoyed.SetActive(false);
        characterStartled.SetActive(false);
        characterAlert.SetActive(false);
    }

    public void SetCharacterDefault()
    {
        if (characterStartled.activeSelf)
            return;

        DisableAllCharacters();
        characterDefault.SetActive(true);
    }

    public void SetCharacterAnnoyed()
    {
        if(characterStartled.activeSelf)
            return;

        DisableAllCharacters();
        characterAnnoyed.SetActive(true);

        audioSourceFemale.PlayOneShot(startledSoundsFemale[Random.Range(0, startledSoundsFemale.Length)]);

    }

    public void SetCharacterStartled()
    {
        if(characterStartled.activeSelf)
            return;

        DisableAllCharacters();
        characterStartled.SetActive(true);

        audioSourceFemale.PlayOneShot(startledSoundsFemale[Random.Range(0, startledSoundsFemale.Length)]);
        audioSourceMale.PlayOneShot(startledSoundsMale[Random.Range(0, startledSoundsMale.Length)]);
    }

    public void SetCharacterAlert()
    {
        if(characterStartled.activeSelf)
            return;

        DisableAllCharacters();
        characterAlert.SetActive(true);
    }
}
