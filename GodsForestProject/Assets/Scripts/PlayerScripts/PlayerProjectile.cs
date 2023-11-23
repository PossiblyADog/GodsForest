using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private int damageToDeal;
    private float knockbackForce, travelSpeed, splitVal;
    private Rigidbody2D projBody;
    private Vector2 targetPosition;
    private bool destroyOnHit;
    private GameObject hitEffect;
    private int animSelector;
    private bool rocketFire = false;
    public bool armorPiercing;
    public float dotInterval, dotLifespan;
    public int dotDamage;
    public GameObject dotEffect;
    public bool destructs = true;

    [SerializeField]
    protected AudioClip hitSFX, travelSFX;

    protected AudioSource bulletAudio;


    void Awake()
    {
        hitEffect = Resources.Load<GameObject>("Sfx/MissleHitEffects/DefaultBulletHit");
        projBody = GetComponent<Rigidbody2D>();
    }

    public void SetBulletParams(float speed, int damage, float knockForce, Vector2 targetPos, bool isSprayRandom, float degree, bool destroyOnEnemyHit, int hitAnim)
    {
        animSelector = hitAnim;
        travelSpeed = speed;
        damageToDeal = damage;
        targetPosition = targetPos.normalized;
        destroyOnHit = destroyOnEnemyHit;
        splitVal = degree;

        if (PlayerController.instance.items.Count > 0)
        {
            foreach (var i in PlayerController.instance.items)
            {
                i.item.OnEffectProjectile(this.gameObject, i.itemStacks);
            }
        }

        if (knockForce > 0)
        {
            knockbackForce = knockForce;
        }

        if (isSprayRandom)
        {
            targetPosition.x += Random.Range(-.50f, .50f);
            targetPosition.y += Random.Range(-.50f, .50f);
        }

        if (!rocketFire)
        {
            Fire(targetPosition);
        }

        if (GetComponent<AudioSource>() != null)
        {
            bulletAudio = GetComponent<AudioSource>();
            bulletAudio.volume = GameManager.instance.sfxVolume;
        }


        if(travelSFX != null)
        {
            bulletAudio.clip = travelSFX;
            bulletAudio.Play();
        }

    }

    public void SetStaticAttack(int damage, float knockForce, bool destructable, int onHitAnim)
    {
        damageToDeal = damage;
        knockbackForce = knockForce;
        destructs = destructable;
        destroyOnHit = false;
        animSelector = onHitAnim;


        if (GetComponent<AudioSource>() != null)
        {
            bulletAudio = GetComponent<AudioSource>();
            bulletAudio.volume = GameManager.instance.sfxVolume;
        }


        if (travelSFX != null)
        {
            bulletAudio.clip = travelSFX;
            bulletAudio.Play();
        }
    }

    private void Update()
    {
        if (rocketFire && projBody != null)
        {
            transform.right = projBody.velocity;
            projBody.AddForce(targetPosition * travelSpeed * PlayerStateManager.playerManager.projectileTravelSpeedMultiplier, ForceMode2D.Force);
        }
    }

    private void Fire(Vector2 direction)
    {
        
        projBody.AddForce(direction * travelSpeed * PlayerStateManager.playerManager.projectileTravelSpeedMultiplier, ForceMode2D.Impulse);
        transform.right = projBody.velocity;
        if (splitVal != 0)
        {
            projBody.AddForce(transform.up * splitVal, ForceMode2D.Impulse);
        }

    }

    public void SetRocket()
    {
        rocketFire = true;
    }

    public void SetDestroy(float seconds)
    {
        Destroy(gameObject, seconds);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 12)//8 is current Layer number for all enemies, 12 bosses
        {
            if (bulletAudio != null)
            {
                bulletAudio.Stop();
                bulletAudio.PlayOneShot(hitSFX);
            }
            else
            {
                AudioSource.PlayClipAtPoint(hitSFX, transform.position, GameManager.instance.sfxVolume);
            }

            var enemy = collision.gameObject.GetComponent<AbstractEnemyBase>();

            if (enemy != null)
            {
                
                
                    enemy.EnemyTakeDamage(damageToDeal, armorPiercing);
                    if (dotEffect != null)
                    { enemy.GetComponentInChildren<EffectHandler>().ApplyDOTEffect(dotEffect, dotLifespan, dotDamage, dotInterval); }

                    if (knockbackForce > 0)
                    {
                        enemy.EnemyGetKnocked(knockbackForce, (Vector2)enemy.transform.position - Physics2D.ClosestPoint(enemy.transform.position, GetComponent<Collider2D>()));
                    }

                    if (collision.gameObject.tag == "EliteEnemy" && destructs)
                    { destroyOnHit = true; }

                    if (destroyOnHit == true)
                    {
                    if (projBody != null)
                    { projBody.velocity = Vector2.zero; }
                        travelSpeed = 0.0f;
                        if (animSelector != 0)
                        {
                            var hit = Instantiate(hitEffect, transform.position, Quaternion.identity);
                            hit.GetComponent<BulletAnimDecider>().SetAnim(animSelector);
                        try
                        {
                            gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        catch
                        {
                            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                        }
                            Destroy(this.gameObject, .05f); 
                        }
                        else
                        {
                            if (transform.GetComponent<Animator>() != null)
                            {
                                transform.GetComponent<Animator>().SetTrigger("isHit");
                            }
                            else
                            {
                                transform.GetComponentInChildren<Animator>().SetTrigger("isHit");
                            }

                            transform.Rotate(0, 0, -transform.localRotation.z);
                            Destroy(this.gameObject, .4f); 
                        }
                    }
                
            }
        }
        else if (collision.gameObject.layer == 3 || collision.gameObject.layer == 11)//3 is current Layer number for 'Obstacles' like walls and rocks, 11 is Destructables
        {
            if (destructs)
            {
                if(transform.parent != null)
                {
                    if( transform.parent.GetComponent<Tornado>() != null)
                    {
                        transform.parent.GetComponent<Tornado>().Deactivate();
                    }
                }

                projBody.velocity = Vector2.zero;
                travelSpeed = 0.0f;

                if (bulletAudio != null)
                {
                    bulletAudio.Stop();
                    bulletAudio.PlayOneShot(hitSFX);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(hitSFX, transform.position, GameManager.instance.sfxVolume);
                }

                if (animSelector != 0)
                {
                    var hit = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    hit.GetComponent<BulletAnimDecider>().SetAnim(animSelector);
                    try
                    {
                        gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    catch
                    {

                    }
                    Destroy(this.gameObject, .05f);
                }
                else
                {
                    if (transform.GetComponent<Animator>() != null)
                    {
                        transform.GetComponent<Animator>().SetTrigger("isHit");
                    }
                    else
                    {
                        transform.GetComponentInChildren<Animator>().SetTrigger("isHit");
                    }
                    Destroy(this.gameObject, .5f);
                }

                if (collision.gameObject.GetComponent<AbstractDestructable>() != null)
                {
                    collision.gameObject.GetComponent<AbstractDestructable>().TakeDamage(damageToDeal);
                }
            }
            else if(collision.gameObject.layer == 11)
            {
                if (collision.gameObject.GetComponent<AbstractDestructable>() != null)
                {
                    collision.gameObject.GetComponent<AbstractDestructable>().TakeDamage(damageToDeal);
                }
            }
        }
        else
        {

        }

    }
}
