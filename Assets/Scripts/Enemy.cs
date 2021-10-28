using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Enemy : MonoBehaviour
{
    [SerializeField] protected float damageDealt = 10f;
    [SerializeField] protected float turnSpeed = 5f;


    [SerializeField] protected float detectionRange = 20f;
    public float DetectionRange
    {
        get { return detectionRange; }
        protected set
        {
            if (value <= 0.0f) { Debug.LogError("Negative detection range values not allowed!"); }
        }
    }


    [SerializeField] protected float attackRange = 5f;
    public float AttackRange
    {
        get { return attackRange; }
        protected set
        {
            if (value <= 0.0f) { Debug.LogError("Negative attack range values not allowed!"); }
        }
    }


    [SerializeField] protected float chaseRange = 17f;
    public float ChaseRange
    {
        get { return chaseRange; }
        protected set
        {
            if (value <= 0.0f) { Debug.LogError("Negative attack range values not allowed!"); }
        }
    }


    protected bool isProvoked = false;
    public bool IsProvoked { get { return isProvoked; } }


    protected bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }


    protected GameObject target = null;
    public GameObject Target
    {
        get { return target; }
        protected set { target = value; }
    }

    protected NavMeshAgent navMeshAgent = null; 
    protected float distanceToTarget = Mathf.Infinity;
    protected bool isGroundEnemy = true;
    protected bool isOnGround = false;


    private void Awake()
    {
        InitializeComponents();
    }

    void Start()
    {
        FindTarget();
    }

    void Update()
    {
        ProcessBehavior();
        //print("isDetected: " + isDetected());
    }

    private void FindTarget()
    {
        target = FindObjectOfType<Player>().gameObject;

        if (target == null) { Debug.LogError("Target not found!"); return; }

        CalculateDistanceToTarget();
    }

    private bool isDetected()
    {
        bool _detection = false;
        distanceToTarget = CalculateDistanceToTarget();

        if (distanceToTarget <= detectionRange)
        {
            _detection = true;
        }
        else
        {
            _detection = false;
        }

        return _detection;
    }


    protected float CalculateDistanceToTarget()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        return distance;
    }

    protected void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    protected virtual void InitializeComponents()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void ProcessBehavior()
    {
        distanceToTarget = CalculateDistanceToTarget();

        FaceTarget();

        if (distanceToTarget <= attackRange)
        {
            AttackTarget();
        }
        else if (distanceToTarget <= chaseRange || isProvoked)
        {
            ChaseTarget();
        }

        ReturnToWanderState();
    }

    protected virtual void ChaseTarget()
    {
        isAttacking = false;
        isProvoked = true;

        navMeshAgent.SetDestination(target.transform.position);
        Debug.Log("Enemy is chasing player");
    }

    protected virtual void AttackTarget()
    {
        isProvoked = true;
        isAttacking = true;
        Debug.Log("Enemy is attacking player");
    }

    protected virtual void ReturnToWanderState()
    {
        if (!isDetected() && (isProvoked || IsAttacking))
        {
            if (isGroundEnemy)
            {
                navMeshAgent.SetDestination(transform.position);
            }

            isProvoked = false;
            isAttacking = false;
        }
    }


    public bool IsDetected()
    {
        bool _targetDetected = isDetected();

        return _targetDetected;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
