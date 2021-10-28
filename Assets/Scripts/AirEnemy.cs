using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirEnemy : Enemy
{
    [SerializeField] protected GameObject projectilePrefab;

    [SerializeField] protected float projectileLaunchDelay = 1f;
    [SerializeField] protected float randomForceRangeX = 20f;
    [SerializeField] protected float randomForceRangeZ = 20f;
    [SerializeField] protected float yOffset = 10f;
    [SerializeField] protected float wanderDelay = 0.5f;
    [SerializeField] protected float wanderMoveSpeed = 100f;


    public float Offset
    {
        get { return yOffset; }
        protected set
        {
            if (value <= 2f)
            {
                Debug.LogError("Air enemies must have an offset value of at least 2!");
            }
            else if(value > 20f)
            {
                Debug.LogWarning("Air enemies will increasingly be offscreen with offset values above 20");
            }
        }
    }

    protected Rigidbody rb;
    protected bool isChasing;


    void OnEnable()
    {
        isGroundEnemy = false;
        StartCoroutine(WanderState());
    }

    private IEnumerator WanderState()
    {
        if (!isProvoked && !isAttacking)
        {
            yield return new WaitForSeconds(wanderDelay);
            rb.AddForce(CalculateRandomForce() * wanderMoveSpeed * Time.deltaTime, ForceMode.Impulse);
            print("Wander");          
        }
        else
        {
            yield break;
        }
    }

    protected IEnumerator MoveTowardsTarget(Vector3 _target = new Vector3())
    {
        yield return new WaitForEndOfFrame();
        print("MoveTowardsTarget");
        rb.AddRelativeForce(_target - transform.position, ForceMode.Impulse);
    }

    protected Vector3 CalculateRandomForce()
    {
        float _forceX = Random.Range(-randomForceRangeX, randomForceRangeX);
        float _forceZ = Random.Range(-randomForceRangeZ, randomForceRangeZ);

        Vector3 _forceDirection = new Vector3(_forceX, 0f, _forceZ);

        return _forceDirection;
    }

    //Calculate the position directly above the player, on the same plane as this enemy
    protected Vector3 CalculateTargetSpot()
    {
        Vector3 _targetSpot = target.transform.position + new Vector3(0f, yOffset, 0f);
        print(_targetSpot);
        return _targetSpot;
    }

    protected override void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void ChaseTarget()
    {
        if(!isChasing)
        {
            isAttacking = false;
            isProvoked = true;
            isChasing = true;
            StopCoroutine(HandleProjectiles());
            StartCoroutine(MoveTowardsTarget(CalculateTargetSpot()));
        }
    }

    protected override void AttackTarget()
    {
        if(!isAttacking)
        {
            isChasing = false;
            isAttacking = true;
            isProvoked = true;
            StartCoroutine(HandleProjectiles());
        }   
    }

    protected override void LeaveProvokeState()
    {
        if(!IsDetected() && (isProvoked || IsAttacking))
        {
            isProvoked = false;
            isAttacking = false;
            isChasing = false;

            StopCoroutine(HandleProjectiles());
            StopCoroutine(MoveTowardsTarget());
            StartCoroutine(WanderState());
        }
    }

    protected virtual IEnumerator HandleProjectiles(float _launchDelay = 1f)
    {
        print("HandleProjectiles");
        Instantiate(projectilePrefab);

        yield return new WaitForSeconds(_launchDelay);
    }
}

