using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapAI : MonoBehaviour
{
    private Animator animator;
    public GameObject arrow;
    private bool shouldShoot = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetInRange(Transform target)
    {
        StopAllCoroutines();
        animator.SetTrigger("inRange");
        shouldShoot = true;
        StartCoroutine(ShootArrow(target));
        
    }

    public void SetOutRange()
    {
        shouldShoot = false;
        animator.SetTrigger("outRange");        
    }



    private IEnumerator ShootArrow(Transform targetPos)
    {
        yield return null;
        if (shouldShoot)
        {
        yield return new WaitForSeconds(1.8f);
        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(.2f);

        var bullet = Instantiate(arrow, transform.position + new Vector3(.25f, -.1f), Quaternion.identity);
        bullet.GetComponent<EnemyProjectile>().SetBulletParams(8f, 5, 15f, transform.right, 0, 0, false);


            StartCoroutine(ShootArrow(targetPos));
        }
    }


}
