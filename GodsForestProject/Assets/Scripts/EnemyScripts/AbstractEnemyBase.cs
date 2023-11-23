using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public abstract class AbstractEnemyBase : MonoBehaviour
{
    public int currentHP;
    [SerializeField]
    protected int /*currentHP*/ maxHP, enemyDamage, enemySpeed, favorChance = 25, healChance = 8, lootChance = 15, armor;
    protected Rigidbody2D enemyBody;
    protected float minimumDistance, knockForce, projectileSpeed, deathSpeed = .75f, hurtSoundCD = .5f, nextHurtSound;
    protected Transform target;
    protected bool isAlive = true;
    public DungeonRoomGenerator.RoomData homeRoom;
    protected bool randomRoomPos = true;
    protected AudioSource enemyAudio;
    public float speedMultiplier = 1.0f;
    

    [SerializeField]
    protected List<AudioClip> enemySounds = new List<AudioClip>();


    public int EnemyDamage
    {
        get { return enemyDamage; }
    }

    public float KnockForce
    { get { return knockForce; } }

    public virtual void EnemyTakeDamage(int damageToTake, bool armorPiercing)
    {
        if (isAlive)
        {
            var damText = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), transform.position + new Vector3(.1f, .1f), Quaternion.identity);
            damText.transform.localScale *= 3.0f;
            if (armorPiercing) { damageToTake += armor; }

            if (damageToTake > armor)
            {

                damText.GetComponentInChildren<TMPro.TMP_Text>().text = (damageToTake - armor).ToString();
            }
            Destroy(damText, .5f);

            if (currentHP > (damageToTake - armor))
            {
               
                currentHP -= (damageToTake - armor); 

                if (enemyAudio != null && Time.time > nextHurtSound)
                {
                    GetComponent<Animator>().SetTrigger("isHurt");
                    enemyAudio.PlayOneShot(enemySounds[1]);
                    nextHurtSound = Time.time + hurtSoundCD;
                }

                if (BossHealthUpdater.instance.bossBar.activeSelf && gameObject.layer == 12)
                {
                    BossHealthUpdater.instance.SetCurrentHP(currentHP);
                }
            }
            else
            {
                EnemyDeath();
            }
            }
        }
    

    public virtual void Attack()
    {

    }

    public void IsMinion()
    {
        favorChance = 0;
        healChance = 0;
        lootChance = 0;
    }

    public virtual void EnemyGetKnocked(float knockForce, Vector2 direction)
    {
        enemyBody.AddForce(direction * knockForce, ForceMode2D.Impulse);
     
    }


    public virtual void SetStaticSpawnPos()
    {
        randomRoomPos = false;
    }

    public virtual void BossBarData()
    {
        BossHealthUpdater.instance.Switch();
        BossHealthUpdater.instance.SetBossBar(gameObject.name, maxHP);
    }

    public virtual void EnemyDeath()
    {
        if (isAlive)
        {
            isAlive = false;
            GetComponent<Animator>().StopPlayback();
            GetComponent<Animator>().SetTrigger("isDead");
            GetComponent<Collider2D>().enabled = false;
            if (GetComponent<LootDrop>() != null)
            {
                GetComponent<LootDrop>().SetDrop(lootChance);
            }

            if (enemyAudio != null)
            {
                enemyAudio.PlayOneShot(enemySounds[0]);
            }

            if(Random.Range(1, 101) < favorChance)
            { 
                var coin = Instantiate(Resources.Load<GameObject>("Lootables/FavorItem"), transform.position, Quaternion.identity);
                coin.GetComponent<FavorItem>().favorAmount = maxHP / 2;
            }

            if (Random.Range(1, 101) < healChance)
            { 
                var heart = Instantiate(Resources.Load<GameObject>("Lootables/HealingItem"),transform.position, Quaternion.identity);
            }

            if (PlayerController.instance.items.Count > 0)
            {
                foreach (var i in PlayerController.instance.items)
                {
                    i.item.OnEnemyDeath(i.itemStacks);
                }
            }

                if (transform.parent != null)
            { Destroy(transform.parent.gameObject, deathSpeed); }
            else
            { Destroy(gameObject, deathSpeed); }
        }
    }

    public virtual void MiniBossTrigger()
    {

    }

    public virtual void SetRoomData(DungeonRoomGenerator.RoomData room)
    {
        homeRoom = room;
        Initialize();
    }

    public virtual void UpdateEnemyVolume()
    {
        try
        {
            enemyAudio.volume = GameManager.instance.enemyVolume;
        }
        catch
        {
            Debug.Log("Didnt work");
        }
    }

    public virtual void Initialize()
    {

    }

    public virtual void SetTarget()
    {

    }

}
