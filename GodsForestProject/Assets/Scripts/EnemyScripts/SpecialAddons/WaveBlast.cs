using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBlast : MonoBehaviour
{

    private List<Transform> waveLines = new List<Transform>();
    public AudioClip waveSound;



    private void Awake()
    {
        Debug.Log("wave line count: " + transform.childCount);
        for(int i = 0; i < transform.childCount; i++)
        {          
            waveLines.Add(transform.GetChild(i));
        }
        
    }

    public void SetWaveStats(GameObject shotEffect, float knockForce, int damage, float destroyTime, bool destroyOnHit, float delayActivation, float blastSpacing, Vector2 playerPos)
    {
        StartCoroutine(StartWaveParams(shotEffect, knockForce, damage, destroyTime, destroyOnHit, delayActivation, blastSpacing, playerPos));
    }

    private IEnumerator StartWaveParams(GameObject shotEffect, float knockForce, int damage, float destroyTime, bool destroyOnHit, float delayActivation, float blastSpacing, Vector2 playerPos)
    {
        SetDirection(playerPos);
        for (int i = 0; i < waveLines.Count; i++)
        {
            {
                for (int j = 0; j < waveLines[i].transform.childCount; j++)
                {
                    {
                         var bullet = Instantiate(shotEffect, waveLines[i].GetChild(j).transform.position, Quaternion.identity);
                         bullet.GetComponent<EnemyAttackBox>().SetAttackParams(knockForce, damage, destroyTime, destroyOnHit, delayActivation);
                        
                    }
                }
                AudioSource.PlayClipAtPoint(waveSound, transform.position, GameManager.instance.enemyVolume);
                yield return new WaitForSeconds(blastSpacing);
            }
        }

        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }

    private void SetDirection(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
