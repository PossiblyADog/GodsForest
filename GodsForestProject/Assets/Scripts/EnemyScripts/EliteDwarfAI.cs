using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteDwarfAI : AbstractEnemyBase
{
    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private AIData data;

    private Collider2D attackCollider;

    private EnemyAttackBox hitbox;

    [SerializeField]
    private ContextSolver moveDirectionSolver;
    private Vector2 moveInput = Vector2.zero;


    private float detectDelay = .1f, attackDelayRanged = 5f,attackDelayMelee = 1.0f, timeBetweenCasts, timeBetweenMelee;


    private float attackDistance = 5f;

    private bool facingForward = true;

    private bool isAttacking = false;

    private Animator animator;
    public GameObject proj;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();

    private Transform playerTransform;



    public override void Initialize()
    {
        maxHP = 200;
        currentHP = maxHP;
        enemyDamage = 15;
        knockForce = 30f;
        enemySpeed = 220;
        projectileSpeed = 5.5f;
        armor = 3;
        lootChance = 150;

        if (randomRoomPos)
        {
            foreach (var pos in homeRoom.CurrentRoomFloor)
            {
                availableTiles.Add(pos);
            }

            Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
            GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);
        }

        playerTransform = PlayerController.instance.transform;
        animator = GetComponent<Animator>();
        hitbox = GetComponentInChildren<EnemyAttackBox>();
        hitbox.SetAttackParams(knockForce*3, enemyDamage, 0.0f, false, 0.0f);
        attackCollider = hitbox.GetComponent<Collider2D>();
        enemyBody = GetComponent<Rigidbody2D>();
        data = GetComponent<AIData>();
        detectors.Add(GetComponentInChildren<TargetDetector>());
        detectors.Add(GetComponentInChildren<ObstacleDetector>());
        steeringBehaviours.Add(GetComponentInChildren<SeekBehaviour>());
        steeringBehaviours.Add(GetComponentInChildren<AvoidanceBehaviour>());
        moveDirectionSolver = GetComponentInChildren<ContextSolver>();
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        InvokeRepeating("PerformDetection", 0, detectDelay);
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
        if (moveInput != null && !isAttacking && isAlive)
        {
            enemyBody.AddForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
        }
    }

    private void Update()
    {
        if (data.targets != null && data.targets.Count > 0)
        {
            if (!isAttacking && Time.time > timeBetweenCasts && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackDistance)
            {
                isAttacking = true;
                StartCoroutine(PerformRangedAttack());
            }
            else if(!isAttacking && Time.time > timeBetweenMelee && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackDistance)
            {
                isAttacking = true;
                StartCoroutine(PerformMeleeAttack());
            }



        }


        if (Mathf.Abs(enemyBody.velocity.x) > .01f || Mathf.Abs(enemyBody.velocity.y) > .01f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }


        if (enemyBody.velocity.x < 0f && facingForward)
        {
            Flip();
        }
        else if (enemyBody.velocity.x > 0f && !facingForward)
        {
            Flip();
        }


    }

    IEnumerator PerformRangedAttack()
    {
        if (playerTransform.position.x < transform.position.x && !facingForward)
        {
            Flip();
        }
        animator.SetTrigger("isRangedAttacking");
        yield return new WaitForSeconds(.2f);
        enemyAudio.PlayOneShot(enemySounds[2]);

        for (int i = -3; i < 4; i+=3)
        {
            var bullet = Instantiate(proj, transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, (Vector2)playerTransform.position - (Vector2)transform.position, 0, i, false);
        }
        timeBetweenCasts = Time.time + attackDelayRanged;
        isAttacking = false;
        yield return new WaitForSeconds(.15f);

    }

    IEnumerator PerformMeleeAttack()
    {
        if (playerTransform.position.x < transform.position.x && !facingForward)
        {
            Flip();
        }
        animator.SetTrigger("isMeleeAttacking");
        enemyAudio.PlayOneShot(enemySounds[2]);
        yield return new WaitForSeconds(.2f);
        attackCollider.enabled = true;
        yield return new WaitForSeconds(.1f);
        attackCollider.enabled = false;
        timeBetweenMelee = Time.time + attackDelayMelee;
        isAttacking = false;
        yield return new WaitForSeconds(.2f);
    }

    public override void SetTarget()
    {
        GetComponentInChildren<TargetDetector>().targetDetectionRadius = 25;
        //data.targets = new List<Transform>() { PlayerController.instance.gameObject.transform };
    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
}