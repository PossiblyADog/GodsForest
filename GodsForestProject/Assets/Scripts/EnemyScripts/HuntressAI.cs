using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntressAI : AbstractEnemyBase
{

    [SerializeField]
    private List<Detector> detectors;
    private TargetDetector targetDetector;

    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private AIData data;

    [SerializeField]
    private ContextSolver moveDirectionSolver;

    private Vector2 moveInput = Vector2.zero;
    private bool facingForward, isRunning;


    private float detectDelay = .1f, dodgeSpeed, dodgeTimer;


    private Animator animator;
    private List<Vector2Int> availableTiles = new List<Vector2Int>();

    [SerializeField]
    private GameObject arrow;

    private float timeBetweenSpreads;


    [SerializeField]
    private HuntressWaypointAI waypointAI;

    public override void Initialize()
    {

        transform.position = new Vector3Int(homeRoom.CurrentRoomCenter.x, homeRoom.CurrentRoomCenter.y + 3);
        knockForce = 12f;
        maxHP = 200;
        currentHP = maxHP;
        enemyDamage = 6;
        enemySpeed = 275;
        armor = 1;
        projectileSpeed = 6f;
        isRunning = true;
        dodgeSpeed = 100f;
        deathSpeed = 1.25f;
        lootChance = 1200;
        //Debug.Log(transform.parent.GetChild(1).name);
        waypointAI = transform.parent.GetChild(1).GetComponent<HuntressWaypointAI>();
        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
        waypointAI.SetWaypointData(homeRoom.CurrentRoomFloor, homeRoom.CurrentRoomCenter, homeRoom.CurrentRoomType);

        enemyBody = GetComponent<Rigidbody2D>();
        target = PlayerController.instance.transform;
        data = GetComponent<AIData>();
        detectors.Add(GetComponentInChildren<TargetDetector>());
        detectors.Add(GetComponentInChildren<ObstacleDetector>());
        steeringBehaviours.Add(GetComponentInChildren<SeekBehaviour>());
        steeringBehaviours.Add(GetComponentInChildren<AvoidanceBehaviour>());
        moveDirectionSolver = GetComponentInChildren<ContextSolver>();
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);
        InvokeRepeating("PerformDetection", 0, detectDelay);

        targetDetector = (TargetDetector)detectors[0];
        targetDetector.targetChoice = 2;
        BossBarData();
    }


        
    

    public override void MiniBossTrigger()
    {
        DungeonEnemyGenerator.instance.ActivateLevelBoss();
        BossHealthUpdater.instance.Switch();
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
    public override void EnemyTakeDamage(int damageToTake, bool armorPiercing)
    {
        if (isAlive)
        {
            if (armorPiercing) { damageToTake += armor; }
            if (currentHP > (damageToTake - armor))
            {
                GetComponent<Animator>().SetTrigger("isHurt");
                currentHP -= (damageToTake - armor);

                if (enemyAudio != null)
                {
                    enemyAudio.PlayOneShot(enemySounds[Random.Range(1, 3)]);
                }

                if (BossHealthUpdater.instance.bossBar.activeSelf)
                {
                    BossHealthUpdater.instance.SetCurrentHP(currentHP);
                }
            }
            else
            {
                EnemyDeath();
                MiniBossTrigger();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAlive && isRunning && moveInput != null)
        {           
            enemyBody.AddForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
        }


    }

    private IEnumerator PerformDodge()
    {
        dodgeTimer = Time.time + 15.0f;
        waypointAI.SetWaypointByQuadrant();

        PerformDetection();

        yield return new WaitForSeconds(.2f);
        float jumpforce = dodgeSpeed * Vector2.Distance(transform.position, target.position);
        enemyBody.AddForce(moveInput*jumpforce, ForceMode2D.Impulse);
        StartCoroutine(PerformBarrageAttack());
        yield return new WaitForSeconds(1.75f);
        isRunning = true;
        
       
    }

    private IEnumerator PerformSpreadAttack()//Shoot 3 arrows at player 3 times, with space between to dodge
    {
        yield return new WaitForSeconds(.2f);
        enemyBody.velocity = Vector2.zero;
        if (target.position.x - transform.position.x < 0 && !facingForward)
        {
            Flip();
        }
        else if(target.position.x - transform.position.x > 0 && facingForward)
        {
            Flip();
        }

            if (Time.time > timeBetweenSpreads)
            {

            for (int j = 0; j < 3; j++)
            {
                Vector2 currentTargPos = target.position;
                Vector2 castPosition = transform.position;
                animator.SetTrigger("isAttacking");

                for (int i = -2; i < 3; i++)
                {
                    var bullet = Instantiate(arrow, castPosition, Quaternion.identity);
                    bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed, enemyDamage, knockForce, currentTargPos - castPosition, 0, i * 2, false);

                }
                yield return new WaitForSeconds(.45f);
            }
               timeBetweenSpreads = Time.time + 1.0f;
            }
        isRunning = true;
    }

    private IEnumerator PerformBarrageAttack()
    {       
        yield return new WaitForSeconds(1.0f);
        enemyBody.velocity = Vector2.zero;
        if (target.position.x - transform.position.x < 0 && !facingForward)
        {
            Flip();
        }
        else if (target.position.x - transform.position.x > 0 && facingForward)
        {
            Flip();
        }

        for (int i = 0; i < 16; i++)
        {
            var bullet = Instantiate(arrow, transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(projectileSpeed*1.25f, enemyDamage, knockForce, (Vector2)target.position - (Vector2)transform.position + PlayerController.instance.CurrentVelocity()/2, 0, 0, false);
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.25f);

    }

    internal void AttemptDodge()
    {
        if (Time.time > dodgeTimer)
        {
            animator.SetTrigger("isDodging");
            isRunning = false;
            StartCoroutine(PerformDodge());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.name.Contains("WaypointAI"))
        {
            isRunning=false;
        }
    }
    public void PerformXAttack(int attackIndex)
    {      
        if(attackIndex == 1)
        {
            StartCoroutine(PerformSpreadAttack());
        }
        else if(attackIndex == 2)
        {
            StartCoroutine(PerformBarrageAttack());
        }
        else
        {

        }
       
    }
 
    private void Update()
    {


        if (enemyBody.velocity.x != 0 || enemyBody.velocity.y != 0)
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
    private void Flip()
    {
        facingForward = !facingForward;
        transform.Rotate(0f, 180f, 0f);
    }
}
