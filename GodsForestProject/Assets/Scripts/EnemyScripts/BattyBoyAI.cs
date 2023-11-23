using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattyBoyAI : AbstractEnemyBase
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
        maxHP = 120;
        armor = 2;
        currentHP = maxHP;
        enemyDamage = 12;
        projectileSpeed = 6.0f;
        enemyBody = GetComponent<Rigidbody2D>();
        deathSpeed = 1.0f;
        lootChance = 30;
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
        teleportDelay = Random.Range(3.0f, 6.0f);
        attackDelay = Random.Range(6.0f, 10.0f);
        isAttacking = false;
        isMoving = false;


    }


    private void FixedUpdate()
    {
        if (isAlive)
        {


            distanceFromPlayer = Vector2.Distance(homeRoom.CurrentRoomCenter, (Vector2)playerPos.position);
            if (distanceFromPlayer < 10)//if player is near
            {
                if (!isMoving && !isAttacking)
                {
                    isMoving = true;
                    Invoke("TeleportAnim", teleportDelay - .3f);
                    Invoke("PerformMove", teleportDelay);
                }

                if (Time.time > nextAttackTime && distanceFromPlayer < 10)
                {
                    if (transform.position.x < playerPos.position.x)
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
                        animator.SetTrigger("isAttacking");
                        StartCoroutine(PerformAttack());
                    }
                }
            }
        }
    }

    private void PerformMove()
    {
        transform.position = waypointAI.transform.position;
        enemyAudio.PlayOneShot(enemySounds[2]);
        isMoving = false;
        animator.SetTrigger("isMoving");

    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(.2f);
        var bullet = Instantiate(projectile, transform.position, transform.rotation);
        enemyAudio.PlayOneShot(enemySounds[3]);
        yield return new WaitForSeconds(.2f);
        bullet.GetComponent<FormationShot>().CastFormation(playerPos.position - transform.position);
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
        nextAttackTime = Time.time + attackDelay;
    }

    private void TeleportAnim()
    {
        animator.SetTrigger("isMoving");
    }
}