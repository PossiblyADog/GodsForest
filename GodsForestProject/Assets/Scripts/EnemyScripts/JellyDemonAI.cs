using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDemonAI : AbstractEnemyBase
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


    public GameObject swordStrikeEffect, demonMissle;

    private float detectDelay = .2f, timeBetweenAttackOne, timeBetweenAttackTwo, timeBetweenAttackThree, timeBetweenAttackFour;


    private float shortDistance = 1.25f, midDistance = 5.5f;

    private bool facingForward = true;

    private bool isAttacking = false;

    private Animator animator;

    [SerializeField]
    private Transform strikePoint, castPoint;
    private ParticleEffectStarter fireParticle;



    public override void Initialize()
    {
        StartCoroutine(JellyDemonEnterDelay());
        maxHP = 2500;
        currentHP = maxHP;
        enemyDamage = 20;
        knockForce = 50f;
        enemySpeed = 73000;
        armor = 0;
        deathSpeed = 1.75f;
        BossBarData();
        fireParticle = GetComponentInChildren<ParticleEffectStarter>();
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

    IEnumerator JellyDemonEnterDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1.25f);
        timeBetweenAttackOne = Time.time + 1.0f;
        timeBetweenAttackTwo = Time.time + 5.0f;
        timeBetweenAttackThree = Time.time + 10.0f;
        timeBetweenAttackFour = Time.time + 12.0f;
        isAttacking = false;
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

    public override void EnemyDeath()
    {
        StopAllCoroutines();
        base.EnemyDeath();
    }

    private void Update()
    {
        if (isAlive)
        {
            if (Mathf.Abs(enemyBody.velocity.x) > .01f || Mathf.Abs(enemyBody.velocity.y) > .01f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }


            if (enemyBody.transform.position.x < PlayerController.instance.transform.position.x && facingForward)
            {
                Flip();
            }
            else if (enemyBody.transform.position.x > PlayerController.instance.transform.position.x && !facingForward)
            {
                Flip();
            }

            if (!isAttacking && data.targets != null && data.targets.Count > 0)
            {
                var distance = Vector2.Distance(transform.position, PlayerController.instance.transform.position);
                Debug.Log(distance);

                if (!isAttacking && distance < shortDistance && Time.time > timeBetweenAttackOne)
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttackOne());

                }
                else if (!isAttacking && distance > shortDistance && Time.time > timeBetweenAttackTwo)
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttackTwo());

                }
                else if (!isAttacking && distance > midDistance && Time.time > timeBetweenAttackThree)
                {
                    isAttacking = true;
                    StartCoroutine(PerformAttackThree());

                }
                else
                {
                    if (!isAttacking && Time.time > timeBetweenAttackFour)
                    {
                        isAttacking = true;
                        StartCoroutine(PerformAttackFour());

                    }
                }

            }
        }
        


    }

    IEnumerator PerformAttackOne() // extreme fire breath
    {
        timeBetweenAttackOne = Time.time + 2.0f;
        animator.SetTrigger("isAttackOne");
        if (transform.position.x < PlayerController.instance.transform.position.x && facingForward)
        { Flip(); }
        yield return new WaitForSeconds(1.5f);
        timeBetweenAttackOne = timeBetweenAttackOne + 1.0f;
        timeBetweenAttackTwo = timeBetweenAttackTwo + 1.0f;
        timeBetweenAttackThree = timeBetweenAttackThree + 1.0f;
        timeBetweenAttackFour = timeBetweenAttackFour + 2.0f;
        isAttacking = false;


    }
    IEnumerator PerformAttackTwo() //Sword attack w/ explosion chain
    {
        timeBetweenAttackTwo = Time.time + 10.0f;
        animator.SetTrigger("isAttackTwo");
        if (transform.position.x < PlayerController.instance.transform.position.x && facingForward)
        { Flip(); }
        yield return new WaitForSeconds(1.25f);
        timeBetweenAttackOne = timeBetweenAttackOne + 1.0f;
        timeBetweenAttackThree = timeBetweenAttackThree + 1.0f;
        timeBetweenAttackFour = timeBetweenAttackFour + 2.0f;
        isAttacking = false;

    }

    public void CastStrikePoint()
    {
        var effect = Instantiate(swordStrikeEffect, strikePoint.position, Quaternion.identity);
        PerformDetection();
        effect.GetComponent<EnemyProjectile>().SetBulletParams(5.5f, 0, 0, moveInput + PlayerController.instance.CurrentVelocity() / 8, 0, 0, false);
        StartCoroutine(effect.GetComponent<CarpetBombShot>().StartBombing(.15f));
    }
    public void ParticleSwitch()
    {
        fireParticle.OnOffSwitch();
    }
    IEnumerator PerformAttackThree()//Jump towards player
    {
        Debug.Log(timeBetweenAttackThree);
        timeBetweenAttackThree = Time.time + 18.0f;
        Debug.Log(timeBetweenAttackThree);
        yield return new WaitForSeconds(.45f);
        animator.SetTrigger("isAttackThree");
        yield return new WaitForSeconds(1.25f);
        timeBetweenAttackOne = timeBetweenAttackOne + 1.0f;
        timeBetweenAttackTwo = timeBetweenAttackTwo + 1.0f;
        timeBetweenAttackFour = timeBetweenAttackFour + 2.0f;
        isAttacking = false;

    }

    public void Jump()
    {
        enemyBody.AddForce(moveInput * enemySpeed * speedMultiplier * 1.4f, ForceMode2D.Impulse);
    }
    IEnumerator PerformAttackFour()//Fire volley
    {
        timeBetweenAttackFour = Time.time + 12.0f;
        animator.SetTrigger("isAttackFour");
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < 15; i++)
        {
            if (transform.position.x < PlayerController.instance.transform.position.x && facingForward)
            { Flip(); }
            var missle = Instantiate(demonMissle, castPoint.position, Quaternion.identity);
            missle.GetComponent<EnemyProjectile>().SetBulletParams(5.50f + Random.Range(-1.00f, 1.01f), enemyDamage, knockForce / 2, moveInput, .50f, 0, true);
            missle.GetComponent<BounceBullet>().SetBounce(1);
            yield return new WaitForSeconds(.15f);
        }
        animator.SetTrigger("isAttackFourEnd");
        yield return new WaitForSeconds(.5f);
        timeBetweenAttackOne = timeBetweenAttackOne + 1.0f;
        timeBetweenAttackTwo = timeBetweenAttackTwo + 1.0f;
        timeBetweenAttackThree = timeBetweenAttackThree + 1.0f;
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