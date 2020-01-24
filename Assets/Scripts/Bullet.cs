using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : NetworkBehaviour
{
    Rigidbody m_rigidBody;
    Collider m_collider;
    List<ParticleSystem> m_allParticles;

    public ParticleSystem m_explosionFX;
    public List<string> m_bounceTags;

    public int m_speed = 100;
    public float m_lifetime = 3f;
    public int m_bounces = 3;

    // Use this for initialization
    void Start()
    {
        m_allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        StartCoroutine("Expires");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionExit(Collision collision)
    {
        if (m_rigidBody.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_rigidBody.velocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (m_bounceTags.Contains(collision.gameObject.tag))
        {
            if (m_bounces <= 0)
            {
                Explode();
            }
            m_bounces--;
        }
    }

    IEnumerator Expires()
    {
        yield return new WaitForSeconds(m_lifetime);
        Explode();
    }

    void Explode()
    {
        m_collider.enabled = false;
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.Sleep();


        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        if (m_explosionFX != null)
        {
            m_explosionFX.transform.parent = null;
            m_explosionFX.Play();
        }
        if (isServer) //example of how they can be destroyed on the server and be destroyed in the clients as well
        {
            Destroy(gameObject);
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }
    }
}
