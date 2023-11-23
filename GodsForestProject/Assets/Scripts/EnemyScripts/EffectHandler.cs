using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    [SerializeField]
    private AbstractEnemyBase parentEnemy;

    private float timeToEnd;
    bool currentlyEffected;

    private void Start()
    {
        parentEnemy = GetComponentInParent<AbstractEnemyBase>();
    }
    public void ApplyDOTEffect(GameObject effect, float lifespan, int damage, float tickInterval)
    {
        if (!currentlyEffected)
        {
            var vfx = Instantiate(effect, transform.position, Quaternion.identity);
            vfx.transform.SetParent(parentEnemy.transform);
            vfx.transform.localScale = new Vector3(parentEnemy.transform.localScale.x/10.0f, parentEnemy.transform.localScale.y/10.0f, 0);
            vfx.GetComponent<EffectDestroyer>().SayWhen((lifespan * PlayerStateManager.playerManager.dotDurationMultiplier) + .01f);
            currentlyEffected = true;
            timeToEnd = (lifespan * PlayerStateManager.playerManager.dotDurationMultiplier) + Time.time;
            StartCoroutine(StartDOTEffect(damage, tickInterval));
        }
    }

    public IEnumerator ApplyMovespeedEffect(GameObject effect, float lifespan, float speedChange)
    {
        if (!currentlyEffected)
        {
            currentlyEffected = true;
            var vfx = Instantiate(effect, transform.position, Quaternion.identity);
            vfx.transform.SetParent(parentEnemy.transform);
            vfx.transform.localScale = new Vector3(parentEnemy.transform.localScale.x / 10.0f, parentEnemy.transform.localScale.y / 10.0f, 0);
            parentEnemy.speedMultiplier += speedChange;
            vfx.GetComponent<EffectDestroyer>().SayWhen(lifespan * PlayerStateManager.playerManager.dotDurationMultiplier + .05f);
            yield return new WaitForSeconds(lifespan * PlayerStateManager.playerManager.dotDurationMultiplier);
            parentEnemy.speedMultiplier -= speedChange;
        }
    }
    private IEnumerator StartDOTEffect(int damage, float tickInterval)
    {
        if(Time.time < timeToEnd)
        {
            if (parentEnemy != null)
            {
                parentEnemy.EnemyTakeDamage(damage, true);
                yield return new WaitForSeconds(tickInterval);
                StartCoroutine(StartDOTEffect(damage, tickInterval));
            }
            else
            {
            yield return null;
            }
        }
        else
        {
            currentlyEffected = false;
        }
        
    }
}
