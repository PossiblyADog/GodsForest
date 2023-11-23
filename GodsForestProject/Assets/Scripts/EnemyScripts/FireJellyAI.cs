using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireJellyAI : AbstractEnemyBase
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

    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();
    private SpriteRenderer enemySprite;

    [SerializeField]
    private NecromancerWaypointAI waypointAI;

    private Transform playerPos;
    private bool isAttacking, facingForward;
    private float attackDelay, nextBlastTime, nextTBDTime, detectDelay = .05f;

    public GameObject daddy;
    public Transform daddySpawnPoint;


    [SerializeField]
    private GameObject radialBlast, projectile;


    public override void Initialize()
    {

        knockForce = 30f;
        enemySpeed = 900;
        maxHP = 800;
        currentHP = maxHP;
        enemyDamage = 15;
        projectileSpeed = 5.75f;
        enemyBody = GetComponent<Rigidbody2D>();
        facingForward = false;
        deathSpeed = .95f;
        lootChance = 0;
        armor = 0;
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
        playerPos = PlayerController.instance.transform;
        isAttacking = false;
        var targetDetector = (TargetDetector)detectors[0];
        targetDetector.targetChoice = 3;
        BossBarData();
        InvokeRepeating(nameof(PerformDetection), 0, detectDelay);
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
        if (isAlive)
        {
            isAlive = false;
            GetComponent<Animator>().SetTrigger("isDead");
            GetComponent<Collider2D>().enabled = false;
            if (GetComponent<LootDrop>() != null)
            {
                GetComponent<LootDrop>().SetDrop(lootChance);
            }

            if (enemyAudio != null)
            {
                enemyAudio.PlayOneShot(enemySounds[0]);
            }

            if (Random.Range(1, 101) < favorChance)
            {
                var coin = Instantiate(Resources.Load<GameObject>("Lootables/FavorItem"), transform.position, Quaternion.identity);
                coin.GetComponent<FavorItem>().favorAmount = maxHP / 2;
            }

            if (Random.Range(1, 101) < healChance)
            {
                var heart = Instantiate(Resources.Load<GameObject>("Lootables/HealingItem"), transform.position, Quaternion.identity);
            }

            if (PlayerController.instance.items.Count > 0)
            {
                foreach (var i in PlayerController.instance.items)
                {
                    i.item.OnEnemyDeath(i.itemStacks);
                }
            }

            if (transform.parent != null)
            { Destroy(transform.parent.gameObject, deathSpeed); }
            else
            { Destroy(gameObject, deathSpeed); }
            Invoke(nameof(SummonDaddy), deathSpeed * .89f);
        }
    }

    public void SummonDaddy()
    {
        var dad = Instantiate(daddy, daddySpawnPoint.position, Quaternion.identity); 
        dad.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(homeRoom);
        dad.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
    }

    private void Update()
    {
        if (data.targets != null && data.targets.Count > 0 && isAlive)
        {

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

    }


    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }

    public override void Attack()
    {
        isAttacking = true;
        int selector = Random.Range(0, 10);
        if (selector <= 7)
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
        yield return new WaitForSeconds(.1f);
        animator.SetTrigger("isAttacking");
        enemyAudio.PlayOneShot(enemySounds[2]);
        var effect = Instantiate(radialBlast, transform.position, Quaternion.identity);      
        StartCoroutine(effect.GetComponent<RadialBlast>().PerformRadialBlast(projectile, projectileSpeed, enemyDamage, knockForce, 1, 1));        
        yield return new WaitForSeconds(.5f);
        isAttacking = false;

    }
    private IEnumerator PerformAttackTwo()
    {
        yield return new WaitForSeconds(.35f);

            animator.SetTrigger("isAttackTwo");
        yield return new WaitForSeconds(.1f);

        for (int i = 0; i < 20; i++)
        {
            enemyAudio.PlayOneShot(enemySounds[3]);
            float speedMultiplier = Random.Range(.750f, 1.501f);
            var effect = Instantiate(projectile, transform.position, Quaternion.identity);
            effect.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed*speedMultiplier, enemyDamage, knockForce, target.position - transform.position, .5f, 0, false);
            yield return new WaitForSeconds(.1f);
        }
        animator.SetTrigger("isDone");

        yield return new WaitForSeconds(.5f);
        isAttacking = false;

    }

}