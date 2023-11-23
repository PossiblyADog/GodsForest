using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApothocaryAI : AbstractEnemyBase
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


    private float detectDelay = .1f, attackDelay = 1.5f, timeBetweenCasts;


    private float attackDistance = 2.5f;

    private bool facingForward;

    private bool isAttacking = false;

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();



    public override void Initialize()
    {
        maxHP = 175;
        currentHP = maxHP;
        enemyDamage = 15;
        knockForce = 50f;
        enemySpeed = 1400;
        armor = 4;
        deathSpeed = 1.1f;
        lootChance = 30;

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
        if (data.targets != null)
        {
            if (!isAttacking && Time.time > timeBetweenCasts && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackDistance)
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

    IEnumerator PerformAttack()
    {
        if (transform.position.x < data.targets[0].transform.position.x && !facingForward)
        { Flip(); }

        yield return new WaitForSeconds(.25f);
        animator.SetTrigger("isAttacking");
        enemyAudio.PlayOneShot(enemySounds[2]);
        enemyBody.AddForce(moveInput * enemySpeed * speedMultiplier * .75f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.25f);
        timeBetweenCasts = Time.time + attackDelay;
        isAttacking = false;

    }

}