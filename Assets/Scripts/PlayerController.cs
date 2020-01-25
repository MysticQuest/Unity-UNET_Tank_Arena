using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerShoot))]

public class PlayerController : NetworkBehaviour
{

    PlayerShoot m_pShoot;
    PlayerMotor m_pMotor;
    PlayerSetup m_pSetup;
    PlayerHealth m_pHealth;

    Vector3 m_originalPosition;
    NetworkStartPosition[] m_spawnPoints;

    public GameObject m_spawnFX;

    // Use this for initialization
    void Start()
    {
        m_pHealth = GetComponent<PlayerHealth>();
        m_pMotor = GetComponent<PlayerMotor>();
        m_pShoot = GetComponent<PlayerShoot>();
        m_pSetup = GetComponent<PlayerSetup>();
    }

    public override void OnStartLocalPlayer()
    {
        m_spawnPoints = GameObject.FindObjectsOfType<NetworkStartPosition>();
        m_originalPosition = transform.position;
    }

    Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v).normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer || m_pHealth.m_isDead)
        {
            return;
        }

        Vector3 inputDirection = GetInput();
        m_pMotor.MovePlayer(inputDirection);
    }

    void Update()
    {
        if (!isLocalPlayer || m_pHealth.m_isDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_pShoot.Shoot();
        }

        Vector3 inputDirection = GetInput();
        if (inputDirection.sqrMagnitude > 0.25f)  //sqrMag is to compare V3 to float
        {
            m_pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointScreen(Input.mousePosition, m_pMotor.m_turret.position.y) - m_pMotor.m_turret.position;
        m_pMotor.RotateTurret(turretDir);

        Camera.main.transform.position = GetComponent<Transform>().position + transform.up * 30 - transform.forward * 12;
    }

    void Disable()
    {
        StartCoroutine("Respawn");
    }

    IEnumerator Respawn()
    {
        transform.position = GetRandomSpawn();
        m_pMotor.m_rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(3f);
        m_pHealth.Reset();

        if (m_spawnFX != null)
        {
            GameObject spawnFX = Instantiate(m_spawnFX, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            Destroy(spawnFX, 3f);
        }
    }

    Vector3 GetRandomSpawn()
    {
        if (m_spawnPoints != null)
        {
            if (m_spawnPoints.Length > 0)
            {
                NetworkStartPosition startPos = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                return startPos.transform.position;
            }
        }
        return m_originalPosition;
    }
}
