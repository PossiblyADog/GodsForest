using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{

    public int damage, laserLength = 15, t, damageStack = 0;
    float effectCD, effectTimer;
    Vector2 targetDir, endPoint;
    LineRenderer laserLine;
    bool isActive = false;
    

    public void SetLaser(int damage, float cooldown)
    {
        transform.parent = PlayerController.instance.transform.GetChild(1).GetChild(0);
        transform.localPosition = new Vector2(0, .15f);
        t = 0;
        laserLine = GetComponent<LineRenderer>();
        this.damage = damage;
        effectCD = cooldown;
        isActive = true;
        effectTimer = Time.time + .05f;
    }

    // Update is called once per frame
    void Update()
    {

        
        t++;
        if(t > 90)
        {
            damageStack++;
            damageStack = Mathf.Clamp(damageStack, 0, damage * 2);
            t = 0;
        }
        if (isActive)
        {   
            var cursorPoint = (Vector2)CursorController.instance.transform.position ;
            targetDir = cursorPoint - (Vector2)transform.position;
            endPoint = cursorPoint + targetDir.normalized * laserLength;
            LaserEffect();
            laserLine.SetPositions(new Vector3[] { transform.position, endPoint});

            if(cursorPoint.x < transform.position.x)
            {
                transform.localPosition = new Vector2(0, -.15f);
            }
            else
            {
                transform.localPosition = new Vector2(0, .15f);
            }
        }

        

    }

    private void LaserEffect()
    {
        var hits = Physics2D.RaycastAll(transform.position, targetDir, (endPoint - (Vector2)transform.position).magnitude);

        if (Time.time > effectTimer)
        {
            bool hitSomething = false;

            for (int i = 0; i < hits.Length; i++)
            {
                

                if (hits[i].collider.gameObject.layer == 8 || hits[i].collider.gameObject.layer == 12)
                {
                    hits[i].transform.GetComponent<AbstractEnemyBase>().EnemyTakeDamage(damage + damageStack, true);
                    hitSomething = true;
                }
                else if (hits[i].collider.gameObject.layer == 11)
                {
                    if (hits[i].transform.GetComponent<EnemySpawner>() != null)
                    { hits[i].transform.GetComponent<EnemySpawner>().TakeDamage(damage + damageStack); }
                    else if(hits[i].transform.GetComponent<ExplodingBarrel>() != null)
                    {
                        hits[i].transform.GetComponent<ExplodingBarrel>().TakeDamage(damage + damageStack);
                    }
                    hitSomething = true;
                }

            }

            if (!hitSomething)
            {
               damageStack = 0;
            }
            effectTimer = Time.time + effectCD;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.layer == 3)
            {
                //Debug.Log("Happens");
                if ((hits[i].point - (Vector2)transform.position).magnitude < ((Vector3)endPoint - transform.position).magnitude)
                {
                    endPoint = hits[i].point;
                }
            }
        }
    }
}
