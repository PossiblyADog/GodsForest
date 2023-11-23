using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBullet : MonoBehaviour
{
    public int bounces;

    [SerializeField]
    private Transform target, spawnPos;

    public GameObject projPrefab;

    bool enemyProj;
    public void SetBounce(int bounceCount)
    {
        if(gameObject.GetComponent<EnemyProjectile>() != null) 
        { 
            enemyProj = true;
            bounces = bounceCount;
            target = PlayerController.instance.transform;
            spawnPos = transform.GetChild(0);   
        }
        else
        {
            //bounces = bounceCount;
        }
    }

    public void Bounce()
    {
        if(bounces > 0)
        {
            if (enemyProj)
            {
                float speedHolder = gameObject.GetComponent<EnemyProjectile>().travelSpeed;
                int damage = gameObject.GetComponent<EnemyProjectile>().damageToDeal;
                float knock = gameObject.GetComponent<EnemyProjectile>().knockbackForce;
                var bullet = Instantiate(projPrefab, spawnPos.position, Quaternion.identity);
                bullet.GetComponent<BounceBullet>().SetBounce(--bounces);
                bullet.GetComponent<EnemyProjectile>().SetBulletParams(speedHolder, damage, knock, target.position - transform.position, 0, 0, true);
            }
            else
            {

            }
        }
    }
}
