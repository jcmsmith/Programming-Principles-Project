using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirEnemy : Enemy
{
    [SerializeField] protected GameObject projectilePrefab;

    [SerializeField] protected float launchDelay = 1f;
    [SerializeField] protected float forceRangeX = 20f;
    [SerializeField] protected float forceRangeZ = 20f;

    [SerializeField] protected float offset = 10f;
    public float Offset
    {
        get { return offset; }
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


    void OnEnable()
    {
        isGroundEnemy = false;
    }

    private void LateUpdate()
    {
        if(!isProvoked && !isAttacking)
        {
            rb.AddForce(CalculateRandomForce(), ForceMode.VelocityChange);
        }
    }

    protected Vector3 CalculateRandomForce()
    {
        float _forceX = Random.Range(-forceRangeX, forceRangeX);
        float _forceZ = Random.Range(-forceRangeZ, forceRangeZ);

        Vector3 _forceDirection = new Vector3(_forceX, 0f, _forceZ);

        return _forceDirection;
    }

    //Calculate the position directly above the player, on the same plane as this enemy
    protected Vector3 CalculateTargetSpot()
    {
        Vector3 _targetSpot = target.transform.position + new Vector3(0f, offset, 0f);

        return _targetSpot;
    }


    protected override void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void ChaseTarget()
    {
        Vector3 _targetSpot = CalculateTargetSpot();

        rb.AddForce(_targetSpot, ForceMode.Impulse);
    }

    protected override void AttackTarget()
    {
        StartCoroutine(HandleProjectiles());
    }

    protected virtual IEnumerator HandleProjectiles(float _launchDelay = 1f)
    {
        Instantiate(projectilePrefab);

        yield return new WaitForSeconds(_launchDelay);
    }
}

