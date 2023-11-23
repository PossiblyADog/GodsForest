using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageBatAI : AbstractEnemyBase
{


    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private Transform playerPos;
    private bool isMoving, isAttacking;
    private float distanceFromPlayer, nextTeleportTime;
    private float barrageSpacing = 1.0f;
    private bool facingForward;



    [SerializeField]
    private GameObject projectile;


    public override void Initialize()
    {

        knockForce = 5.0f;
        maxHP = 120;
        currentHP = maxHP;
        enemyDamage = 12;
        projectileSpeed = 6.5f;
        armor = 2;
        enemyBody = GetComponent<Rigidbody2D>();
        lootChance = 30;
        foreach (var pos in homeRoom.CurrentRoomFloor)
        {
            availableTiles.Add(pos);
        }
        Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
        GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);
        animator = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        playerPos = PlayerController.instance.transform;
        isAttacking = false;
        isMoving = false;
        nextTeleportTime = Time.time + Random.Range(1.0f, 6.0f);


    }


    private void FixedUpdate()
    {
        if (isAlive)
        {
            distanceFromPlayer = Vector2.Distance(homeRoom.CurrentRoomCenter, (Vector2)playerPos.position);
            if (distanceFromPlayer < 10)//if player is near
            {

                if (!isMoving && !isAttacking && (playerPos.position - transform.position).magnitude < 4f)
                {

                    isAttacking = true;
                    StartCoroutine(PerformAttack());

                }          
                else if (Time.time > nextTeleportTime && !isAttacking)
                {
                    isMoving = true;
                    StartCoroutine(PerformMove());
                }
                else
                {

                }


                if (transform.position.x < playerPos.position.x && facingForward)
                {
                    Flip();
                }
                else if (transform.position.x > playerPos.position.x && !facingForward)
                {
                    Flip();
                }

            }
        }


    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }

    private IEnumerator PerformMove()
    {

        nextTeleportTime = Time.time + Random.Range(4.0f, 6.0f);
        Vector2Int movePos = Vector2Int.zero;
        while(((Vector2)playerPos.position - movePos).magnitude > 4.5f)
        {
            movePos = availableTiles[Random.Range(0, availableTiles.Count)];
        }
            animator.SetTrigger("isMoving");
            yield return new WaitForSeconds(.5f);
            transform.position = new Vector3(movePos.x, movePos.y, 0);
            yield return new WaitForSeconds(.5f);
            isMoving = false;       
    }

    private IEnumerator PerformAttack()
    {
        while((playerPos.position - transform.position).magnitude < 7.0f)                     
        {
            barrageSpacing *= .90f;
            barrageSpacing = Mathf.Clamp(barrageSpacing, .1f, 1.0f);
            yield return new WaitForSeconds(barrageSpacing);
            animator.SetTrigger("isAttacking");
            yield return new WaitForSeconds(.1f);
            enemyAudio.PlayOneShot(enemySounds[3]);
            var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (playerPos.position - transform.position), 0, 0, false);
            yield return null;
        }

        barrageSpacing = 1.0f;
        isAttacking = false;

    }


}