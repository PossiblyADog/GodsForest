using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceEmitter : AbstractTrapBase
{
    public AudioClip pushSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            if (Time.time > nextCastTime)
            {
                nextCastTime = Time.time + cooldown;
                FireEffect(new Vector3(0, 0, 0));
            }
        }
    }

    public override void FireEffect(Vector3 targetPos)
    {
        GetComponent<Animator>().SetTrigger("isTouched");
        AudioSource.PlayClipAtPoint(pushSound, transform.position, GameManager.instance.sfxVolume);
        var effect = Instantiate(trapEffect, transform.position, Quaternion.identity);
        Destroy(effect, .35f);
        Invoke("ResetAnim", nextCastTime - Time.time);
    }

    private void ResetAnim()
    {
        GetComponent<Animator>().SetTrigger("isReady");
    }

    public override void PositionDecider(DungeonRoomGenerator.RoomData room)
    {
        int posVariableX = Random.Range(-2, 2);
        int posVariableY = Random.Range(-2, 2);
        transform.position = new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y) + new Vector3(posVariableX, posVariableY);
    }
}
