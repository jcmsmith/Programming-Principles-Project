using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected ParticleSystem explosionVFX;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this);

        if(explosionVFX != null)
        {
            Instantiate(explosionVFX);
        }
    }
}
