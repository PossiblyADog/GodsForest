using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlimeAI : AbstractEnemyBase
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
    private bool facingForward;

    [SerializeField]
    private float detectDelay = .05f;


    private Animator animator;

    private List<Vector2Int> availableTiles = new List<Vector2Int>();

    [SerializeField]
    private GameObject poisonCloud;

    [SerializeField]
    private Transform poisonSpawnPoint;
    

    [SerializeField]
    private WaypointAI waypointAI;
    int t;
    bool isAttacking;



    public override void Initialize()
    {

               
        foreach (var pos in homeRoom.CurrentRoomFloor)
        {
            availableTiles.Add(pos);
        }

        Vector2Int startPos = availableTiles[Random.Range(0, availableTiles.Count)];
        if (randomRoomPos)
        { GetComponentInParent<Transform>().position = new Vector3(startPos.x + .5f, startPos.y + .5f); }
        knockForce = 15f;
        maxHP = 20;
        currentHP = maxHP;
        enemyDamage = 1;
        enemySpeed = 30;
        armor = 1;

        //Debug.Log(transform.parent.GetChild(1).name);
        waypointAI = transform.parent.GetChild(1).GetComponent<WaypointAI>();

        waypointAI.SetWaypointData(homeRoom.CurrentRoomFloor, homeRoom.CurrentRoomCenter, homeRoom.CurrentRoomType);

        enemyAudio = GetComponent<AudioSource>();
        enemyAudio.volume = GameManager.instance.enemyVolume;
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
        targetDetector.targetChoice = 1;
        //StartCoroutine(PerformAttack());

    }

    private void OnEnable()
    {
        StartCoroutine(PerformAttack());
    }

    private void PerformDetection()
    {

        if (waypointAI != null && enemyBody.velocity.x <= 0.01f && enemyBody.velocity.y <= 0.01f && data.targets.Count > 0)
        {
            waypointAI.ChangeWaypointPosition();
        }
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
        if (moveInput != null && isAlive)
        {
            enemyBody.AddRelativeForce(moveInput * enemySpeed * speedMultiplier, ForceMode2D.Force);
        }
    }
    private void Update()
    {

        if(moveInput.magnitude != 0)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

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

    private IEnumerator PerformAttack()
    {
        if (isAttacking)
        {
            var cloud = Instantiate(poisonCloud, new Vector3(poisonSpawnPoint.position.x + Random.Range(-.250f, .250f), poisonSpawnPoint.position.y + Random.Range(-.200f, .200f), transform.position.z), Quaternion.identity);
            cloud.GetComponent<EnemyAttackBox>().SetAttackParams(knockForce, enemyDamage, 2.0f, false, 0.0f);

            yield return new WaitForSeconds(.1f);
            StartCoroutine(PerformAttack());
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(PerformAttack());
        }
    }

}
