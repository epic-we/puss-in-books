using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowUp : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;

    public void Explode()
    {
        if (explosionEffect != null)
        {
            explosionEffect.SetActive(true);
        }
        Destroy(gameObject);
    }
}
