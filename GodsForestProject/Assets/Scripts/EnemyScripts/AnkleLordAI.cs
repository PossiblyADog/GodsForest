using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkleLordAI : AbstractEnemyBase
{
    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private AIData data;

    [SerializeField]
    private ContextSolver moveDirectionSolver;
    private Vector2 moveInput = Vector2.zero;


    private float detectDelay = .05f, attackMeleeDelay = 2.5f, attackRangedDelay = 7.0f;


    private float attackMeleeDistance = 3.5f;
    private float timeBetweenCasts, timeBetweenBites, timeBetweenSucks;

    private bool isMeleeAttacking = false, isRangedAttacking = false, facingForward;

    private Animator animator;

    [SerializeField]
    private GameObject projectile, blob, waveEffect, suckZone;

    private List<Vector2Int> availableTiles = new List<Vector2Int>();


    public override void Initialize()
    {
        maxHP = 500;
        currentHP = maxHP;
        enemyDamage = 10;
        knockForce = 80f;
        enemySpeed = 300;
        projectileSpeed = 10f;
        deathSpeed = 1.75f;
        armor = 1;
        lootChance = 500;
        favorChance = 100;


        foreach (var pos in homeRoom.CurrentRoomFloor)
        {
            availableTiles.Add(pos);
        }
        Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
        GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);

        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();
        data = GetComponent<AIData>();
        detectors.Add(GetComponentInChildren<TargetDetector>());
        GetComponentInChildren<TargetDetector>().targetDetectionRadius = 15.0f;
        detectors.Add(GetComponentInChildren<ObstacleDetector>());
        steeringBehaviours.Add(GetComponentInChildren<SeekBehaviour>());
        steeringBehaviours.Add(GetComponentInChildren<AvoidanceBehaviour>());
        target = PlayerController.instance.transform;
        moveDirectionSolver = GetComponentInChildren<ContextSolver>();
        InvokeRepeating("PerformDetection", 0, detectDelay);
        timeBetweenCasts = Time.time + 1f;
        timeBetweenSucks = Time.time + 10.0f;
        suckZone.gameObject.SetActive(false);
        BossBarData();
    }


    private void PerformDetection()
    {

        if (detectors.Count > 0)
        {
            foreach (Detector detector in detectors)
            {
                detector.Detect(data);
            }
        }
        moveInput = moveDirectionSolver.GetDirectionToMove(steeringBehaviours, data);
    }
    private void FixedUpdate()
    {
        if (moveInput != null && !isMeleeAttacking && !isRangedAttacking && isAlive)
        {
            enemyBody.AddForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
        }
    }

    public override void EnemyDeath()
    {
        base.EnemyDeath();
        GetComponent<BossDeath>().OnBossDeath(homeRoom);
        BossHealthUpdater.instance.Switch();
    }
    private void Update()
    {
        if (data.targets != null && isAlive)
        {
            if (Time.time > timeBetweenCasts && !isMeleeAttacking)
            {
                StartCoroutine(PerformRangedAttack());
            }

            if (Time.time > timeBetweenBites && !isRangedAttacking && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackMeleeDistance)
            {
                StartCoroutine(PerformAttack());
            }

            if (Time.time > timeBetweenSucks && !isMeleeAttacking && !isRangedAttacking)
            {
                StartCoroutine(SuckAttack());
            }


        }


        if (enemyBody.velocity.x > 0.001f || enemyBody.velocity.y > 0.001f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (transform.position.x < target.position.x && facingForward) 
        {
            Flip();
        }
        else if (transform.position.x > target.position.x && !facingForward)
        {
            Flip();
        }


    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
    private IEnumerator PerformRangedAttack()
    {
        timeBetweenCasts = Time.time + attackRangedDelay;
        isRangedAttacking = true;
        if (data.targets[0].position.x - transform.position.x < 0 && !facingForward)
        {
            Flip();
        }
        else if (data.targets[0].position.x - transform.position.x > 0 && facingForward)
        {
            Flip();
        }

 
        animator.SetTrigger("isAttackingRanged");

        yield return new WaitForSeconds(.15f);
        var bullet = Instantiate(blob, transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (Vector2)data.targets[0].position - (Vector2)transform.position, 0, 0, false);
        bullet.GetComponent<SummonOnHit>().SetSpawnType(homeRoom, 3);

        isRangedAttacking = false;

    }

    private IEnumerator SuckAttack()
    {
        if (data.targets[0].position.x - transform.position.x < 0 && !facingForward)
        {
            Flip();
        }
        else if (data.targets[0].position.x - transform.position.x > 0 && facingForward)
        {
            Flip();
        }

        isRangedAttacking = true;
        animator.SetTrigger("isSucking");
        suckZone.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        suckZone.gameObject.SetActive(false);
        timeBetweenSucks = Time.time + 10f;
        timeBetweenCasts = Time.time + 2f;
        animator.SetTrigger("stopSucking");
        yield return new WaitForSeconds(.5f);
        isRangedAttacking = false;
    }

    private IEnumerator PerformAttack()
    {
        timeBetweenBites = Time.time + attackMeleeDelay;

        if (data.targets[0].position.x - transform.position.x < 0 && !facingForward)
        {
            Flip();
        }
        else if (data.targets[0].position.x - transform.position.x > 0 && facingForward)
        {
            Flip();
        }

        isMeleeAttacking = true;
        yield return new WaitForSeconds(.25f);
        animator.SetTrigger("isAttackingMelee");
        yield return new WaitForSeconds(.25f);

        var wave = Instantiate(waveEffect, transform.position, Quaternion.identity);
        var targetPos = new Vector2(data.targets[0].position.x, data.targets[0].position.y);
        wave.GetComponent<WaveBlast>().SetWaveStats(projectile, knockForce, enemyDamage, .8f, false, .35f, .45f, targetPos);


        yield return new WaitForSeconds(1.0f);
        isMeleeAttacking = false;
    }

}
