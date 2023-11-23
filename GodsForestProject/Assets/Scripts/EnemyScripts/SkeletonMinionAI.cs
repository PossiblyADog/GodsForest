using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMinionAI : AbstractEnemyBase
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


    private float detectDelay = .1f, attackDelay = .5f, timeBetweenCasts;


    private float attackDistance = 1f;

    private bool facingForward;

    private bool isAttacking = false;

    private Collider2D attackCollider;

    private EnemyAttackBox hitbox;
    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();



    public override void Initialize()
    {
        maxHP = 30;
        currentHP = maxHP;
        enemyDamage = 10;
        knockForce = 35f;
        enemySpeed = 50;
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
        target = PlayerController.instance.transform;
        enemyAudio.volume = GameManager.instance.enemyVolume;
        InvokeRepeating("PerformDetection", .5f, detectDelay);
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
        if (data.targets != null)
        {
            if (!isAttacking && Time.time > timeBetweenCasts && Vector2.Distance(transform.position, target.position) < attackDistance)
            {
                if (Random.Range(0, 99) > 49) 
                { animator.SetTrigger("isAttackTwo"); }
                else 
                { animator.SetTrigger("isAttackOne"); }     
                
                isAttacking = true;
                Invoke("PerformAttack", .1f);
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


        if (enemyBody.velocity.x > 0f && facingForward)
        {
            Flip();
        }
        else if (enemyBody.velocity.x < 0f && !facingForward)
        {
            Flip();
        }


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

    private void PerformAttack()
    {
        if (transform.position.x < target.position.x && !facingForward)
        { Flip(); }
        enemyAudio.PlayOneShot(enemySounds[2]);
        attackCollider.enabled = true;
        Invoke("EndAttack", .2f);

    }

    private void EndAttack()
    {
        attackCollider.enabled = false;
        isAttacking = false;
        timeBetweenCasts = Time.time + attackDelay;
    }
}