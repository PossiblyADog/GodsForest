using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShot : MonoBehaviour
{
    int t = 0;
    float chargeSpeed;
    bool charging = false;
    public void StartCharging(float rate)
    {
        GetComponent<Collider2D>().enabled = false;
        chargeSpeed = rate;
        charging = true;
    }

    public void StopChargingShot(int damage, float knock, float speed, Vector2 targetPos)
    {
        charging = true;
        float scale = transform.localScale.x;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<PlayerProjectile>().SetBulletParams(speed * (1.0f + scale / 3.0f), Mathf.RoundToInt(damage * scale), knock * scale, targetPos, false, 0, true, 2);
    }

    public void StopChargingStatic(int damage, float knock, float duration)
    {
        charging = false;
        float scale = transform.localScale.x;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<PlayerProjectile>().SetStaticAttack(Mathf.RoundToInt(damage * scale), knock * scale, false, 4);
        GetComponent<Animator>().SetTrigger("isExplode");
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void LateUpdate()
    {
 
        if (charging)
        {
            transform.position = CursorController.instance.transform.position;

            if(t <= 120)
            {
                t++;
                transform.localScale += new Vector3(chargeSpeed, chargeSpeed, 0);                
            }
            else
            {
                
            }
        }
    }
}
