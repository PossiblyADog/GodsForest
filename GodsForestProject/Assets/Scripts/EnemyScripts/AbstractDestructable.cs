using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDestructable : MonoBehaviour
{
    protected int hp, maxHP, favorChance = 100, healChance = 15;
    protected float deathTimer;
    public AudioClip deathSound;
    bool alive = true;

    public virtual void TakeDamage(int damage)
    {
        if (alive)
        {
            transform.GetComponent<Animator>().SetTrigger("isHurt");
            hp -= damage;

            if (hp < 0)
            {
                alive = false;
                DeathSequence();
            }
        }
    }

    public virtual void DeathSequence()
    {
        if(deathSound != null)
        { AudioSource.PlayClipAtPoint(deathSound, transform.position, GameManager.instance.enemyVolume); }

        if(GetComponent<LootDrop>() != null)
        {
            GetComponent<LootDrop>().SetDrop(75);
        }

        if (Random.Range(1, 101) < favorChance)
        {
            var coin = Instantiate(Resources.Load<GameObject>("Lootables/FavorItem"), transform.position, Quaternion.identity);
            coin.GetComponent<FavorItem>().favorAmount = maxHP/2;
        }

        if (Random.Range(1, 101) < healChance)
        {
            var heart = Instantiate(Resources.Load<GameObject>("Lootables/HealingItem"), transform.position, Quaternion.identity);
        }

        transform.GetComponent<Animator>().SetTrigger("isDead");
        Destroy(gameObject, deathTimer);
    }
}
