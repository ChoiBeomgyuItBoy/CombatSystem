using UnityEngine;
using System.Collections;

namespace MagicArsenal
{
    public class MagicProjectileScript : MonoBehaviour
    {
        public GameObject impactParticle;
        public GameObject projectileParticle;
        public GameObject muzzleParticle;
        public GameObject[] trailParticles;
        [Header("Adjust if not using Sphere Collider")]
        public float colliderRadius = 1f;
        [Range(0f, 1f)]
        public float collideOffset = 0.15f;

        private Rigidbody rb;
        private Transform myTransform;
        private SphereCollider sphereCollider;

        private float destroyTimer = 0f;
        private bool destroyed = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            myTransform = transform;
            sphereCollider = GetComponent<SphereCollider>();

            projectileParticle = Instantiate(projectileParticle, myTransform.position, myTransform.rotation) as GameObject;
            projectileParticle.transform.parent = myTransform;

            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, myTransform.position, myTransform.rotation) as GameObject;

                Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
            }
        }
		
        void FixedUpdate()
        {
            if (destroyed)
            {
                return;
            }

            float rad = sphereCollider ? sphereCollider.radius : colliderRadius;

            Vector3 dir = rb.velocity;
            float dist = dir.magnitude * Time.deltaTime;

            if (rb.useGravity)
            {
                // Handle gravity separately to correctly calculate the direction.
                dir += Physics.gravity * Time.deltaTime;
                dist = dir.magnitude * Time.deltaTime;
            }

            RaycastHit hit;
            if (Physics.SphereCast(myTransform.position, rad, dir, out hit, dist))
            {
                myTransform.position = hit.point + (hit.normal * collideOffset);

                GameObject impactP = Instantiate(impactParticle, myTransform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;

                if (hit.transform.tag == "Destructible") // Projectile will destroy objects tagged as Destructible
                {
                    Destroy(hit.transform.gameObject);
                }

                foreach (GameObject trail in trailParticles)
                {
                    GameObject curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                    curTrail.transform.parent = null;
                    Destroy(curTrail, 3f);
                }
                Destroy(projectileParticle, 3f);
                Destroy(impactP, 5.0f);
                DestroyMissile();
            }
            else
            {
                // Increment the destroyTimer if the projectile hasn't hit anything.
                destroyTimer += Time.deltaTime;

                // Destroy the missile if the destroyTimer exceeds 5 seconds.
                if (destroyTimer >= 5f)
                {
                    DestroyMissile();
                }
            }

            RotateTowardsDirection();
        }

        private void DestroyMissile()
        {
            destroyed = true;

            foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }
            Destroy(projectileParticle, 3f);
            Destroy(gameObject);

            ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
            //Component at [0] is that of the parent i.e. this object (if there is any)
            for (int i = 1; i < trails.Length; i++)
            {
                ParticleSystem trail = trails[i];
                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null);
                    Destroy(trail.gameObject, 2f);
                }
            }
        }

        private void RotateTowardsDirection()
        {
            if (rb.velocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
                float angle = Vector3.Angle(myTransform.forward, rb.velocity.normalized);
                float lerpFactor = angle * Time.deltaTime; // Use the angle as the interpolation factor
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, lerpFactor);
            }
        }
    }
}
