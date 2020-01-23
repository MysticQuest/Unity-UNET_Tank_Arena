using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerShoot))]

public class PlayerController : MonoBehaviour
{

    PlayerShoot m_pShoot;
    PlayerMotor m_pMotor;
    PlayerSetup m_pSetup;
    PlayerHealth m_pHealth;

    // Use this for initialization
    void Start()
    {
        m_pHealth = GetComponent<PlayerHealth>();
        m_pMotor = GetComponent<PlayerMotor>();
        m_pShoot = GetComponent<PlayerShoot>();
        m_pSetup = GetComponent<PlayerSetup>();
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
        Vector3 inputDirection = GetInput();
        m_pMotor.MovePlayer(inputDirection);
    }

    void Update()
    {
        Vector3 inputDirection = GetInput();
        if (inputDirection.sqrMagnitude > 0.25f)  //sqrMag is to compare V3 to float
        {
            m_pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointScreen(Input.mousePosition, m_pMotor.m_turret.position.y) - m_pMotor.m_turret.position;
        m_pMotor.RotateTurret(turretDir);
    }
}
