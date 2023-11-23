using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSlimeAI : AbstractEnemyBase
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

    
    private float detectDelay = .05f, attackMeleeDelay = .75f, attackRangedDelay = 5.0f;

   
    private float attackMeleeDistance = 1.5f, attackRangedDistance = 4f;
    private float timeBetweenCasts, timeBetweenBites;

    private bool isMeleeAttacking = false, isRangedAttacking = false, facingForward;

    private Collider2D attackCollider;

    private EnemyAttackBox hitbox;
    private Animator animator;

    [SerializeField]
    private GameObject projectile;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();


    public override void Initialize()
    {
        maxHP = 80;
        currentHP = maxHP;
        enemyDamage = 10;
        knockForce = 30f;
        enemySpeed = 140;
        projectileSpeed = 5.5f;
        lootChance = 100;
        armor = 1;

        if (randomRoomPos)
        {
            foreach (var pos in homeRoom.CurrentRoomFloor)
            {
                availableTiles.Add(pos);
            }
            Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
            GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);
        }

        animator = GetComponent<Animator>();
        hitbox = GetComponentInChildren<EnemyAttackBox>();
        hitbox.SetAttackParams(knockForce, enemyDamage, 0.0f, false, 0.0f);
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
         if (moveInput != null && !isMeleeAttacking && !isRangedAttacking && isAlive)
         {
             enemyBody.AddRelativeForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
         }        
    }
    private void Update()
    {
        if (data.targets != null && data.targets.Count > 0 && isAlive)
        {          
            
            if(!isMeleeAttacking && !isRangedAttacking && Time.time > timeBetweenCasts && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackRangedDistance)
            {

                isRangedAttacking = true;
                StartCoroutine(PerformRangedAttack());
            }       
            
            
            if(!isMeleeAttacking && !isRangedAttacking && Time.time > timeBetweenBites && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackMeleeDistance)
            {
                animator.SetTrigger("isMeleeAttacking");
                isMeleeAttacking = true;
                Invoke("PerformAttack", .15f);
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

    private void OnEnable()
    {
        timeBetweenCasts = Time.time + 2.0f;
    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
    private IEnumerator PerformRangedAttack()
    {
        yield return new WaitForSeconds(.25f);
        animator.SetTrigger("isRangedAttacking");
        enemyAudio.PlayOneShot(enemySounds[3]);
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < 4; i++)//amount of bursts
        {
            for (int j = 0; j < 3; j++)//shots in burst
            {
                var bullet = Instantiate(projectile, transform.position, transform.rotation);

                try
                {
                    bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage / 2, knockForce / 2,  moveInput, .20f, 0, false);
                }
                catch
                {
                    Destroy(bullet);
                }
            }

            yield return new WaitForSeconds(.05f);
        }

        yield return new WaitForSeconds(.35f);

        timeBetweenCasts = Time.time + attackRangedDelay;
        isRangedAttacking = false;

    }

    private void PerformAttack()
    {
        if (transform.position.x > data.targets[0].transform.position.x && facingForward)
        { Flip(); }
        enemyAudio.PlayOneShot(enemySounds[2]);
        attackCollider.enabled = true;
        Invoke("EndAttack", .25f);

    }

    private void EndAttack()
    {
        attackCollider.enabled = false;
        isMeleeAttacking = false;
        timeBetweenBites = Time.time + attackMeleeDelay;
    }

}

