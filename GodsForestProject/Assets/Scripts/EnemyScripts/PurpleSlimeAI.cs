using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleSlimeAI : AbstractEnemyBase
{


    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private SpriteRenderer enemySprite;
    [SerializeField]
    private WaypointAI waypointAI;
    private Transform playerPos;
    private bool isMoving, isAttacking;
    private float teleportDelay, attackDelay;
    private float distanceFromPlayer;
    private float nextAttackTime;



    [SerializeField]
    private GameObject projectile;


    public override void Initialize()
    {

        knockForce = 10f;
        maxHP = 15;
        currentHP = maxHP;
        enemyDamage = 6;
        projectileSpeed = 5.5f;
        enemyBody = GetComponent<Rigidbody2D>();

        foreach (var pos in homeRoom.CurrentRoomFloor)
        {
            availableTiles.Add(pos);
        }
        Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
        GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);
        waypointAI = transform.parent.GetChild(1).GetComponent<WaypointAI>();
        waypointAI.SetWaypointData(homeRoom.CurrentRoomFloor, homeRoom.CurrentRoomCenter, homeRoom.CurrentRoomType);
        animator = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        enemySprite = GetComponent<SpriteRenderer>();
        playerPos = PlayerController.instance.transform;
        teleportDelay = Random.Range(4.0f, 7.0f);
        attackDelay = Random.Range(2.0f, 4.0f);
        isAttacking = false;
        isMoving = false;      
        
    }

    private void OnEnable()
    {
        nextAttackTime = Time.time + Random.Range(.75f, 1.25f);
    }


    private void FixedUpdate()
    {
        if (isAlive)
        {


            distanceFromPlayer = Vector2.Distance(homeRoom.CurrentRoomCenter, (Vector2)playerPos.position);
            if (distanceFromPlayer < 7)//if player is near
            {
                if (!isMoving && !isAttacking)
                {
                    isMoving = true;
                    Invoke("TeleportAnim", teleportDelay - .3f);
                    Invoke("PerformMove", teleportDelay);
                }

                if (Time.time > nextAttackTime && distanceFromPlayer < 10)
                {
                    if (transform.position.x > playerPos.position.x)
                    {
                        enemySprite.flipX = false;
                    }
                    else
                    {
                        enemySprite.flipX = true;
                    }

                    if (!isAttacking)
                    {
                        isAttacking = true;
                        StartCoroutine(PerformAttack());
                    }
                }
            }
        }
    }
   
    private void PerformMove()
    {
        nextAttackTime = nextAttackTime + .5f;
        transform.position = waypointAI.transform.position;
        enemyAudio.PlayOneShot(enemySounds[2]);
        isMoving = false;
        animator.SetBool("isMoving", false);
        
    }

    private IEnumerator PerformAttack()
    {
            animator.SetTrigger("isAttacking");
            yield return new WaitForSeconds(.15f);
            nextAttackTime = Time.time + attackDelay;
            var bullet = Instantiate(projectile, transform.position, transform.rotation);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (Vector2)playerPos.position - (Vector2)transform.position, 0, 0, false);
            enemyAudio.PlayOneShot(enemySounds[3]);
            yield return new WaitForSeconds(.1f);
            isAttacking = false;   
      
    }

    private void TeleportAnim()
    {
        animator.SetBool("isMoving", true);
    }


    









}

