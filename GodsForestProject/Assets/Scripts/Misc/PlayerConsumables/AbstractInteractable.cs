using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteractable : MonoBehaviour
{
    public float cooldown, nextUseTime;
    private bool canPickup = false;
    public AudioClip useSound;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickup = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickup = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (canPickup && Input.GetKeyDown(KeyCode.F))
        {           
            PerformInteraction();
            if (useSound != null)
            {
                AudioSource.PlayClipAtPoint(useSound, transform.position, GameManager.instance.sfxVolume);
            }
        }
    }

    public virtual void PerformInteraction()
    {

    }
}
