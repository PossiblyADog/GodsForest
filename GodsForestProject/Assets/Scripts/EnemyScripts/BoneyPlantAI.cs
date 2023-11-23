using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneyPlantAI : AbstractEnemyBase
{

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private SpriteRenderer enemySprite;
    [SerializeField]
    private WaypointAI waypointAI;
    private Transform playerPos;
    private bool isAttacking, facingForward;
    private float attackDelay, nextBurrowTime;
    private float distanceFromPlayer;



    [SerializeField]
    private GameObject projectile;


    public override void Initialize()
    {

        knockForce = 20f;
        maxHP = 40;
        currentHP = maxHP;
        enemyDamage = 8;
        projectileSpeed = 5.25f;
        enemyBody = GetComponent<Rigidbody2D>();
        facingForward = true;

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
        playerPos = FindObjectOfType<PlayerController>().transform;
        attackDelay = Random.Range(3.0f, 5.0f);
        isAttacking = false;


    }

    private void OnEnable()
    {
        StartCoroutine(PlayerDistanceCheck());
    }


    private IEnumerator PlayerDistanceCheck()
    {       
        if (isAlive)
        {
            try
            {
                distanceFromPlayer = Vector2.Distance(homeRoom.CurrentRoomCenter, (Vector2)playerPos.position);

                if (playerPos.position.x < transform.position.x && facingForward)
                { Flip(); }
                else if (!facingForward)
                {
                    Flip();
                }


                if (!isAttacking && distanceFromPlayer < 12)//if player is near
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttack());
                }
            }
            catch
            {

            }
            yield return new WaitForSeconds(.2f);
            StartCoroutine(PlayerDistanceCheck());
        }
    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
    public void Burrow()
    {
        if(Time.time > nextBurrowTime)
        {
            nextBurrowTime = Time.time + Random.Range(3.0f, 5.0f);
            StartCoroutine(PerformBurrow());
        }
    }

    private IEnumerator PerformBurrow()
    {
        animator.SetTrigger("isBurrow");
        enemyAudio.PlayOneShot(enemySounds[2]);
        yield return new WaitForSeconds(.35f);
        transform.position = waypointAI.transform.position;
        enemyAudio.PlayOneShot(enemySounds[3]);
        
        yield return null;

    }

    private IEnumerator PerformAttack()
    {
        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(.2f);
        for (int i = -3; i < 4; i+=3)
        {
            var bullet = Instantiate(projectile, transform.position, transform.rotation);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (Vector2)playerPos.position - (Vector2)transform.position, 0, i, false);
        }
        
        enemyAudio.PlayOneShot(enemySounds[4]);

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }
}
