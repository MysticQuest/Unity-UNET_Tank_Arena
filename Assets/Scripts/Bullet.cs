using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : NetworkBehaviour
{
    PlayerShoot m_pShoot;

    Rigidbody m_rigidBody;
    Collider m_collider;
    public List<ParticleSystem> m_allParticles;

    public ParticleSystem m_explosionFX;
    public List<string> m_bounceTags;
    public List<string> m_collisionTags;

    // public int m_speed = 15;
    public float m_lifetime = 5f;
    public int m_bounces = 3;
    public float m_damage = 1f;

    public PlayerManager m_owner;

    public float m_delay = 0.03f;

    public GameObject bulletFXobj;
    public GameObject HitFXobj;

    // Use this for initialization
    void Start()
    {
        m_allParticles = GetComponentsInChildren<ParticleSystem>().ToList();

        m_pShoot = m_owner.GetComponent<PlayerShoot>();
        m_bounces = m_pShoot.m_bBounces;
        m_lifetime = m_pShoot.m_bLifetime;

        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        StartCoroutine("Expires");
    }

    [Command]
    void CmdBulletLook()
    {
        if (m_rigidBody.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_rigidBody.velocity);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        CmdBulletLook();
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckCollisions(collision);

        if (m_bounceTags.Contains(collision.gameObject.tag))
        {
            if (m_bounces <= 0)
            {
                CmdExplode();
            }
            m_bounces--;
        }
    }

    void CheckCollisions(Collision collision)
    {

        if (m_collisionTags.Contains(collision.collider.tag))
        {

            CmdExplode();

            PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Damage(m_damage, m_owner);
            }
        }
    }

    IEnumerator Expires()
    {
        m_collider.enabled = false;
        yield return new WaitForSeconds(m_delay);
        m_collider.enabled = true;

        yield return new WaitForSeconds(m_lifetime);
        CmdExplode();
    }



    // void Explode()
    // {
    //     m_collider.enabled = false;
    //     m_rigidBody.velocity = Vector3.zero;
    //     m_rigidBody.Sleep();


    //     foreach (ParticleSystem ps in m_allParticles)
    //     {
    //         ps.Stop();
    //     }

    //     if (m_explosionFX != null)
    //     {
    //         m_explosionFX.transform.parent = null;
    //         m_explosionFX.Play();
    //         Destroy(m_explosionFX.gameObject, 3f);
    //     }

    //     if (isServer)
    //     {
    //         foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
    //         {
    //             mr.enabled = false;
    //         }
    //         Destroy(gameObject, 1f);
    //     }
    // }

    [Command]
    void CmdExplode()
    {
        m_collider.enabled = false;
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.Sleep();


        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        // if (m_explosionFX != null)
        // {
        //     m_explosionFX.transform.parent = null;
        //     m_explosionFX.Play();
        //     Destroy(m_explosionFX.gameObject, 3f);
        // }

        HitFXobj = Instantiate(bulletFXobj, transform.position, transform.rotation) as GameObject;
        NetworkServer.Spawn(HitFXobj);
        Destroy(HitFXobj, 3f);


        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }

        Destroy(gameObject);



        // if (isServer)
        // {

        // }
    }

}
