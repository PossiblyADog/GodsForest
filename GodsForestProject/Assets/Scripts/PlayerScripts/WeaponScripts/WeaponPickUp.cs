using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    private bool canPickup = false;
    public GameObject entryEffect;
    public AudioClip pickupSound;
    public virtual void Start()
    {
        var effect = Instantiate(entryEffect, transform.position, Quaternion.identity);
        Destroy(effect, .5f);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            canPickup = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        canPickup = false;
        transform.GetChild(0).gameObject.SetActive(false);
        //collision.gameObject.GetComponent<PlayerController>()
    }

    public void Update()
    {
        if (canPickup && Input.GetKeyDown(KeyCode.F))
        {
            if (gameObject.name.Contains("Random")) { SendRandomWeapon(); }
            else { PickupStaff(); }           
        }

    }

    private void PickupStaff()
    {
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
        var currentStaff = PlayerStateManager.playerManager.weaponDropList.Find(match => gameObject.name.Contains(match.GiveName()));
        PlayerController.instance.AddWeapon(currentStaff);
        Destroy(this.gameObject);
    }

    private void SendRandomWeapon()
    {
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
        int selector = Random.Range(0, PlayerStateManager.playerManager.weaponDropList.Count);
        PlayerController.instance.AddWeapon(PlayerStateManager.playerManager.weaponDropList[selector]);       
        Destroy(this.gameObject);
        
    }
}
