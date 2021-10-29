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
    [SerializeField] protected float wanderDelay = 0.5f;
    [SerializeField] protected float wanderMoveSpeed = 100f;
    [SerializeField] protected float chaseMoveSpeed = 100f;

    protected Rigidbody rb;
    protected bool isChasing;


    private void OnEnable()
    {
        isGroundEnemy = false;
        StartCoroutine(WanderState());
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
        Vector3 _targetVector = target.transform.position - transform.position;
        _targetVector.y = 0;

        return _targetVector;
    }

    protected override void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    //POLYMORPHISM
    protected override void ChaseTarget()
    {
        if(!isChasing)
        {
            isAttacking = false;
            isProvoked = true;
            isChasing = true;
            StopCoroutine(AttackState());
            StartCoroutine(ChaseState());
        }
    }

    //POLYMORPHISM
    protected override void AttackTarget()
    {
        if(!isAttacking)
        {
            isChasing = false;
            isAttacking = true;
            isProvoked = true;
            StartCoroutine(AttackState());
        }   
    }

    //POLYMORPHISM
    protected override void ReturnToStartingState()
    {
        if(!IsDetected() && (isProvoked || IsAttacking))
        {
            isProvoked = false;
            isAttacking = false;
            isChasing = false;

            StopCoroutine(AttackState());
            StopCoroutine(ChaseState());
            StartCoroutine(WanderState());
        }
    }

    protected virtual IEnumerator WanderState()
    {
        while (!isProvoked && !isAttacking)
        {
            yield return new WaitForSeconds(wanderDelay);
            rb.AddRelativeForce(CalculateRandomForce() * wanderMoveSpeed * Time.deltaTime, ForceMode.Impulse);
            print("Wander");
        }
    }

    protected virtual IEnumerator ChaseState()
    {
        while (isProvoked)
        {
            print("MoveTowardsTarget");
            rb.AddForce(CalculateTargetSpot() * chaseMoveSpeed, ForceMode.Force);
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual IEnumerator AttackState(float _launchDelay = 1f)
    {
        while(isAttacking)
        {
            yield return new WaitForSeconds(_launchDelay);

            LaunchProjectile();

        }
    }

    //ENCAPSULATION
    private void LaunchProjectile()
    {
        Projectile _projectile = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<Projectile>();

       _projectile.SetTarget(target);
    }

}

