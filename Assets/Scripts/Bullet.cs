using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;

public class Bullet : NetworkBehaviour
{
    public int m_speed = 100;

    List<ParticleSystem> m_allParticles;

    public float m_lifetime = 5f;

    // Use this for initialization
    void Start()
    {
        m_allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
