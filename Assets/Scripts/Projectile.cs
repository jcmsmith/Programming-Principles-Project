using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected ParticleSystem explosionVFX;
    [SerializeField] protected float speedMultiplier = 1f;
    public float SpeedMultiplier 
    { 
        get { return speedMultiplier; }
        protected set 
        { 
            if(value <= 0f)
            {
                Debug.LogError("projectile speed must be greater than 0!");
            }
        }
    }

    [SerializeField] protected float selfDestructionDelay = 2f;

    private GameObject target;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce((target.transform.position - transform.position) * speedMultiplier, ForceMode.VelocityChange);

        StartCoroutine(SelfDestruct(30));
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(SelfDestruct(selfDestructionDelay));
    }

    protected virtual IEnumerator SelfDestruct(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }
}
