using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOrb : MonoBehaviour
{
    public GameObject proj, shotPoint;
    
    private float shotCD = .15f, shotSpeed, shotKnock;
    public int damage;

    public void Initialize(int dam, float speed, float knock)
    {
        damage = dam;
        shotSpeed = speed;
        shotKnock = knock;
        StartCoroutine(FireShot());
    }

    IEnumerator FireShot()
    {
        var target = (shotPoint.transform.position - transform.position);
        var bullet = Instantiate(proj, transform.position, Quaternion.identity);
        bullet.GetComponent<PlayerProjectile>().SetBulletParams(shotSpeed/2, damage, shotKnock, target, false, 0, true, 3);
        yield return new WaitForSeconds(shotCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
        StartCoroutine(FireShot());
    }

}
