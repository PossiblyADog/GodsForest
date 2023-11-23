using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBotAI : MonoBehaviour
{
    public Transform rotatePoint;
    private int damage = 5;
    private float laserCD = 1f;
    public GameObject proj;
    public LayerMask enemyLayer;
    public AudioClip shootSound;
    public AudioSource botSource;

    public void UpgradeBot()
    {
        damage += 2;
        laserCD *= .85f;
    }

    private void FixedUpdate()
    {
        rotatePoint.Rotate(0, 0, 3.0f);
    }

    public void InitializeBot()
    {
        StartCoroutine(FireLaser());
    }

    IEnumerator FireLaser()
    {
        var target = Physics2D.OverlapCircle(transform.position, 5.0f, enemyLayer);

        if (target != null)         
        {
            botSource.pitch = 1.00f + Random.Range(-.150f, .150f);
            botSource.PlayOneShot(shootSound);
            var bullet = Instantiate(proj, transform.position, Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(10.0f, damage, 0, target.transform.position - transform.position, false, 0, true, 4);
        }
        else
        {

        }
        yield return new WaitForSeconds(laserCD);
        StartCoroutine(FireLaser());
    }
}
