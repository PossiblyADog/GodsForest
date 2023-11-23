using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBlast : MonoBehaviour
{
    public List<Transform> nodes;

    public IEnumerator PerformRadialBlast(GameObject projectile, float projSpeed, int damage, float knock, int charType, int bounces)//0=player 1=enemy
    {
        yield return new WaitForSeconds(.25f);
        if (charType == 1)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                bullet.GetComponent<EnemyProjectile>().SetBulletParams(projSpeed, damage, knock, transform.position - nodes[i].position, 0, 0, true);
                bullet.GetComponent<BounceBullet>().SetBounce(bounces);
            }
        }
        else if (charType == 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                bullet.GetComponent<PlayerProjectile>().SetBulletParams(projSpeed, damage, knock, transform.position - nodes[i].position, false, 0, true, 1);
            }
        }
        Destroy(gameObject);
    }


//0=player 1=enemy
   
   public IEnumerator ExecuteRageRadialBlast(GameObject projectile, float projSpeed, int damage, float knock, int charType, int blastCount, float blastDelay, int bounces) 
    { 
        yield return new WaitForSeconds(.25f);
        if (charType == 1)
         {
            for (int j = 0; j < blastCount; j++)
            {
                transform.Rotate(0, 0, 30);
                for (int i = 0; i < nodes.Count; i++)
                {
                    var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                    bullet.GetComponent<EnemyProjectile>().SetBulletParams(projSpeed, damage, knock, transform.position - nodes[i].position, 0, 0, false);
                    bullet.GetComponent<BounceBullet>().SetBounce(bounces);
                }
                yield return new WaitForSeconds(blastDelay);
            }

         }
         else if (charType == 0)
         {
            for (int j = 0; j < blastCount; j++)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                    bullet.GetComponent<PlayerProjectile>().SetBulletParams(projSpeed, damage, knock, transform.position - nodes[i].position, false, 0, true, 1);
                }
                yield return new WaitForSeconds(blastDelay);
            }
         }
        Destroy(gameObject);

        yield return null;

   }
}
