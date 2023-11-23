using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatekeeperAI : AbstractEnemyBase
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

    private float detectDelay = .1f, timeBetweenMelee, timeBetweenCasts, meleeDelay = 1.5f, castDelay = 12.0f;


    private float attackDistance = 2.75f;

    private bool facingForward = true;

    private bool isAttacking = false;

    private Animator animator;

    public GameObject handEffect;

    private Transform playerPos;
    private int shots = 10;

    private ParticleEffectStarter fireParticle;



    public override void Initialize()
    {
        maxHP = 1000;
        currentHP = maxHP;
        enemyDamage = 18;
        knockForce = 50f;
        enemySpeed = 3500;
        armor = 5;
        deathSpeed = 1.25f;
        lootChance = 500;

        fireParticle = GetComponentInChildren<ParticleEffectStarter>(); 
        playerPos = PlayerController.instance.transform;
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
        timeBetweenCasts = Time.time + 5.0f;
        transform.parent.GetChild(1).GetComponent<GatekeeperChildren>().Activate(homeRoom);
        BossBarData();
    }
    public override void MiniBossTrigger()
    {
        DungeonEnemyGenerator.instance.ActivateLevelBoss();
    }

    public override void EnemyDeath()
    {
        base.EnemyDeath();
        for (int i = 0; i < 2; i++)
        {
            if (GetComponent<LootDrop>() != null)
            {
                GetComponent<LootDrop>().SetDrop(lootChance);
            }
        }
        MiniBossTrigger();
        transform.parent.GetChild(1).GetComponent<GatekeeperChildren>().TimeToDie();
        
    }

    public void ParticleSwitch()
    {
        fireParticle.OnOffSwitch();
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
        if (isAlive)
        {
            if (data.targets != null && data.targets.Count > 0)
            {
                if (!isAttacking && Time.time > timeBetweenMelee && Vector2.Distance(transform.position, data.targets[0].transform.position) < attackDistance)
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttack());
                }
                else if (!isAttacking && Time.time > timeBetweenCasts)
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttackTwo());
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


    }

    IEnumerator PerformAttack()
    {
        if(playerPos.position.x > transform.position.x && facingForward)
        {
            Flip();
        }
        yield return new WaitForSeconds(.1f);
        animator.SetTrigger("isAttackOne");
        yield return new WaitForSeconds(.8f);
        timeBetweenMelee = Time.time + meleeDelay;
        isAttacking = false;
    }

    IEnumerator PerformAttackTwo()
    {
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("isAttackTwo");
        yield return new WaitForSeconds(.05f);
        for (int i = 0; i < shots; i++)
        {
            float x = Random.Range(-1.50f, 1.50f);
            float y = Random.Range(-1.50f, 1.50f);
            Vector2 offset = new Vector2(x, y);
            var hand = Instantiate(handEffect, (Vector2)playerPos.position + offset + (PlayerController.instance.CurrentVelocity()/10.0f), Quaternion.identity);
            StartCoroutine(hand.GetComponent<PointStrike>().PointStrikeSet(.05f));
            yield return new WaitForSeconds(.15f);
        }

        castDelay = Mathf.Clamp((15.0f * (currentHP / maxHP)), 7.0f, 15.0f);
        timeBetweenCasts = Time.time + castDelay;
        shots += 2;
        isAttacking = false;
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
