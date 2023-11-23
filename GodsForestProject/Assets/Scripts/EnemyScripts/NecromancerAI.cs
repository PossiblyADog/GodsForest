using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerAI : AbstractEnemyBase
{
    [SerializeField]
    private List<Detector> detectors;
    private bool rageMode = false;

    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private AIData data;

    [SerializeField]
    private ContextSolver moveDirectionSolver;
    private Vector2 moveInput = Vector2.zero;

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private SpriteRenderer enemySprite;

    [SerializeField]
    private NecromancerWaypointAI waypointAI;

    private bool isAttacking, facingForward;
    private float attackDelay, nextSummonTime, detectDelay = .05f, summonDelay = 12.0f;



    [SerializeField]
    private GameObject radialBlast, projectile, skeletonSummon;


    public override void Initialize()
    {

        knockForce = 50f;
        enemySpeed = 530;
        maxHP = 600;
        currentHP = maxHP;
        enemyDamage = 15;
        projectileSpeed = 5.5f;
        enemyBody = GetComponent<Rigidbody2D>();
        facingForward = false;
        deathSpeed = 1.25f;
        lootChance = 400;
        favorChance = 100;
        armor = 3;
        foreach (var pos in homeRoom.CurrentRoomFloor)
        {
            availableTiles.Add(pos);
        }
        BossBarData();
        // GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f);
        waypointAI = transform.parent.GetChild(1).GetComponent<NecromancerWaypointAI>();
        waypointAI.SetWaypointData(homeRoom.CurrentRoomFloor, homeRoom.CurrentRoomCenter, homeRoom.CurrentRoomType);
        animator = GetComponent<Animator>();
        target = PlayerController.instance.transform;
        data = GetComponent<AIData>();
        detectors.Add(GetComponentInChildren<TargetDetector>());
        detectors.Add(GetComponentInChildren<ObstacleDetector>());
        steeringBehaviours.Add(GetComponentInChildren<SeekBehaviour>());
        steeringBehaviours.Add(GetComponentInChildren<AvoidanceBehaviour>());
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        moveDirectionSolver = GetComponentInChildren<ContextSolver>();
        enemySprite = GetComponent<SpriteRenderer>();
        isAttacking = false;
        var targetDetector = (TargetDetector)detectors[0];
        targetDetector.targetChoice = 3;

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
            enemyBody.AddRelativeForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
        }
    }
    public override void EnemyDeath()
    {
        base.EnemyDeath();
        GetComponent<BossDeath>().OnBossDeath(homeRoom);
        BossHealthUpdater.instance.Switch();
        for (int i = 0; i < 2; i++)
        {
            if (GetComponent<LootDrop>() != null)
            {
                GetComponent<LootDrop>().SetDrop(lootChance);
            }
        }
    }

    public override void EnemyTakeDamage(int damageToTake, bool armorPiercing)
    {
        base.EnemyTakeDamage(damageToTake, armorPiercing);

        if(currentHP < (maxHP / 2) && !rageMode)
        {
            rageMode = true;
            StartCoroutine(RageAnim());
        }
    }

    IEnumerator RageAnim()
    {
        waypointAI.gameObject.SetActive(false);
        isAttacking = true;
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("isRaged");
        yield return new WaitForSeconds(1.1f);
        //transform.position += new Vector3(-1.5f, 0);
        transform.parent.localScale = new Vector3(2.75f, 2.75f, 1.0f);
        yield return new WaitForSeconds(.75f);
        waypointAI.gameObject.SetActive(true);
        waypointAI.transform.position = (Vector3Int)homeRoom.CurrentRoomCenter;
        isAttacking = false;
    }

    private void Update()
    {
        if (data.targets != null && data.targets.Count > 0 && isAlive)
        {
            if (!isAttacking && Time.time > nextSummonTime)
            {
                isAttacking = true;
                StartCoroutine(PerformSummon());
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


    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }

    public override void Attack()
    {
        isAttacking = true;
        int selector = Random.Range(0, 2);
        if(selector == 0)
        {
            StartCoroutine(PerformAttackOne());
        }
        else
        {
            StartCoroutine(PerformAttackTwo());
        }
    }

    private IEnumerator PerformAttackOne()
    {
        yield return new WaitForSeconds(.5f);
        if (currentHP > (maxHP / 2))
        {
            animator.SetTrigger("isAttackOne");
            enemyAudio.PlayOneShot(enemySounds[2]);
            yield return new WaitForSeconds(.25f);
            var effect = Instantiate(radialBlast, transform.position, Quaternion.identity);
            StartCoroutine(effect.GetComponent<RadialBlast>().PerformRadialBlast(projectile, projectileSpeed, enemyDamage, knockForce, 1, 0));
        }
        else
        {
            enemyAudio.PlayOneShot(enemySounds[4]);
            animator.SetTrigger("isRageAttackOne");
            yield return new WaitForSeconds(.2f);
            var effect = Instantiate(radialBlast, transform.position, Quaternion.identity);
            StartCoroutine(effect.GetComponent<RadialBlast>().ExecuteRageRadialBlast(projectile, projectileSpeed, enemyDamage, knockForce, 1, 3, .25f, 0));
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(.75f);
        isAttacking = false;

    }
    private IEnumerator PerformAttackTwo()
    {
        yield return new WaitForSeconds(.5f);
        if (currentHP > maxHP / 2)
        {
            enemyAudio.PlayOneShot(enemySounds[3]);
            animator.SetTrigger("isAttackTwo");
            var effect = Instantiate(projectile, transform.position, Quaternion.identity);
            effect.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, target.position - transform.position, 0, 0, true);
            effect.GetComponent<BounceBullet>().SetBounce(2);
        }
        else
        {
            enemyAudio.PlayOneShot(enemySounds[5]);
            animator.SetTrigger("isRageAttackTwo");
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(.25f);
                var effect = Instantiate(projectile, transform.position, Quaternion.identity);
                effect.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, target.position - transform.position, 0, 0, true);
                effect.GetComponent<BounceBullet>().SetBounce(3);
                yield return new WaitForSeconds(.2f);
            }

        }
        yield return new WaitForSeconds(.75f);
        isAttacking = false;

    }
    private IEnumerator PerformSummon()
    {

        yield return new WaitForSeconds(.25f);
        enemyAudio.PlayOneShot(enemySounds[6]);
        if (currentHP > maxHP / 2)
        {
            animator.SetTrigger("isSummon");
            yield return new WaitForSeconds(.25f);
            var skeleton = Instantiate(skeletonSummon, (Vector3Int)availableTiles[Random.Range(0, availableTiles.Count)], Quaternion.identity);
            skeleton.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(homeRoom);
            skeleton.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                animator.SetTrigger("isSummon");
                yield return new WaitForSeconds(.25f);
                var skeleton = Instantiate(skeletonSummon, (Vector3Int)availableTiles[Random.Range(0, availableTiles.Count)], Quaternion.identity);
                skeleton.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(homeRoom);
                skeleton.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
            }
        }
        yield return new WaitForSeconds(.5f);
        nextSummonTime = Time.time + summonDelay;
        isAttacking = false;

    }
}