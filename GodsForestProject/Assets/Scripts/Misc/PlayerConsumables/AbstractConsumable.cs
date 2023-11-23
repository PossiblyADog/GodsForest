using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractConsumable : MonoBehaviour
{

    public GameObject entryEffect;
    public AudioClip pickupSound;
    public virtual void Start()
    {
        var effect = Instantiate(entryEffect, transform.position, Quaternion.identity);
        Destroy(effect, .5f);
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
            }
            ApplyEffect();
            Remove();
        }
    }

    public virtual void ApplyEffect()
    {

    }

    public virtual void Remove()
    {
        Destroy(gameObject);
    }
}


