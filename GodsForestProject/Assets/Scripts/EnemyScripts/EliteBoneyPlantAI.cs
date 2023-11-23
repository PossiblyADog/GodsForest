using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteBoneyPlantAI : AbstractEnemyBase
{

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private SpriteRenderer enemySprite;
    [SerializeField]
    private WaypointAI waypointAI;
    private Transform playerPos;
    private bool isAttacking, facingForward;
    private float attackDelay, nextBurrowTime, timeBetweenCasts;
    private float distanceFromPlayer;



    [SerializeField]
    private GameObject projectile;


    public override void Initialize()
    {

        knockForce = 40f;
        maxHP = 200;
        currentHP = maxHP;
        armor = 4;
        enemyDamage = 16;
        projectileSpeed = 5.5f;
        enemyBody = GetComponent<Rigidbody2D>();
        facingForward = true;
        lootChance = 250;
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
        attackDelay = 3.0f;
        isAttacking = false;

    }

    private void OnEnable()
    {
        timeBetweenCasts = Time.time + .75f;
        StartCoroutine(ManualUpdate());
    }


    private IEnumerator ManualUpdate()
    {
        if (isAlive)
        {
            try
            {
                if (playerPos.position.x < transform.position.x && facingForward)
                { Flip(); }
                else if (!facingForward)
                {
                    Flip();
                }
            }
            catch
            {

            }


            if (!isAttacking && Time.time > timeBetweenCasts)//if player is near
            {
                isAttacking = true;
                StartCoroutine(PerformAttack());
            }

        }
        yield return new WaitForSeconds(.2f);
        StartCoroutine(ManualUpdate());
    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
    public void Burrow()
    {
        if (Time.time > nextBurrowTime && !isAttacking)
        {
            nextBurrowTime = Time.time + 10.0f;
            StartCoroutine(PerformBurrow());
        }
    }

    private IEnumerator PerformBurrow()
    {
        yield return null;
        animator.SetTrigger("isMoving");
        enemyAudio.PlayOneShot(enemySounds[2]);
        yield return new WaitForSeconds(.20f);
        transform.position = waypointAI.transform.position;
        enemyAudio.PlayOneShot(enemySounds[3]);

        yield return new WaitForSeconds(.15f);

    }

    private IEnumerator PerformAttack()
    {

        yield return new WaitForSeconds(.20f);
        animator.SetTrigger("isAttacking");
        for (int i = -8; i < 9; i += 2)
        {
            var bullet = Instantiate(projectile, transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (Vector2)playerPos.position - (Vector2)transform.position, 0, i, false);
        }

        enemyAudio.PlayOneShot(enemySounds[4]);

        yield return new WaitForSeconds(.5f);
        timeBetweenCasts = Time.time + attackDelay;
        isAttacking = false;

    }
}
