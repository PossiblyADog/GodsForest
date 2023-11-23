using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDwarfAI : AbstractEnemyBase
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
    private Collider2D attackCollider;

    private EnemyAttackBox hitbox;

    private float detectDelay = .1f, timeBetweenMelee;


    private float attackDistance = 3f;

    private bool facingForward = true;

    private bool isAttacking = false;

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();

    private Transform playerTransform;



    public override void Initialize()
    {
        maxHP = 25;
        currentHP = maxHP;
        enemyDamage = 8;
        knockForce = 30f;
        enemySpeed = 150;
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

        playerTransform = PlayerController.instance.transform;
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();
        data = GetComponent<AIData>();
        hitbox = GetComponentInChildren<EnemyAttackBox>();
        hitbox.SetAttackParams(knockForce, enemyDamage, 0.0f, false, 0.0f);
        attackCollider = hitbox.GetComponent<Collider2D>();
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
            if (!isAttacking && Time.time > timeBetweenMelee && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackDistance)
            {
                isAttacking = true;
                StartCoroutine(PerformAttack());
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

    IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(.25f);
            if (playerTransform.position.x < transform.position.x && !facingForward)
            {
                Flip();
            }
            animator.SetTrigger("isAttacking");
            //enemyAudio.PlayOneShot(enemySounds[2]);

            var targetPos = (PlayerController.instance.transform.position - transform.position);
            float timeCap = 0f;
            attackCollider.enabled = true;
            while (Vector3.Distance(transform.position, targetPos) > .5f && timeCap < 90)
            {
                enemyBody.AddForce(targetPos.normalized * enemySpeed * 2.5f, ForceMode2D.Force);
                timeCap++;
                yield return new WaitForFixedUpdate();
            }
           animator.SetTrigger("isDone");
           attackCollider.enabled = false;
           isAttacking = false;
           yield return new WaitForSeconds(.25f);
        
    }

    public override void SetTarget()
    {
        GetComponentInChildren<TargetDetector>().targetDetectionRadius = 20;
        //data.targets = new List<Transform>() { PlayerController.instance.gameObject.transform };
    }

    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
}