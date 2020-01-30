using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(Bullet))]

public class Buff : MonoBehaviour
{
    public PlayerShoot m_pShoot;
    public PlayerMotor m_pMotor;
    public PlayerHealth m_pHealth;

    Collider m_collider;
    public GameObject m_buff;
    public ParticleSystem m_buffFX;

    GameObject buffedPlayer;

    public int m_buffCD;
    public int m_buffDuration;

    public int buffedBulletSpeed;

    public int buffedShotsPerBurst;

    public int buffedBulletLifetime;
    public int buffedBulletBounces;

    public int buffedSpeed;
    public int buffedRotation;
    public int buffedTurretRotation;


    // Use this for initialization
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;
        m_buffFX.Stop();

        StartCoroutine("EnableBuff");
    }

    IEnumerator EnableBuff()
    {
        yield return new WaitForSeconds(m_buffCD);
        m_collider.enabled = true;
        m_buffFX.Play();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject buffedChild = collision.gameObject;
            buffedPlayer = buffedChild.transform.root.gameObject;

            m_collider.enabled = false;
            m_buffFX.Stop();

            BuffPlayer();
            StartCoroutine("EnableBuff");
        }
    }

    void BuffPlayer()
    {
        BuffPicker();
        PlayerBuffEffect();
    }

    void PlayerBuffEffect()
    {
        GameObject buffEffect = Instantiate(m_buff, buffedPlayer.transform.position, buffedPlayer.transform.rotation) as GameObject;
        buffEffect.transform.parent = buffedPlayer.transform;
        Destroy(buffEffect, m_buffDuration);
    }

    void BuffPicker()
    {
        int actions = Random.Range(0, 6);
        switch (actions)
        {
            case 0:
                StartCoroutine("BuffBullet");
                Debug.Log("buff bullet");
                break;

            case 1:
                StartCoroutine("BuffRegen");
                Debug.Log("buff regen");
                break;

            case 2:
                StartCoroutine("BuffBulletLife");
                Debug.Log("buff bullet life");
                break;

            case 3:
                StartCoroutine("BuffBurst");
                Debug.Log("buff burst");
                break;

            case 4:
                StartCoroutine("BuffSpeed");
                Debug.Log("buff speed");
                break;

            default:
                break;

        }
    }

    IEnumerator BuffRegen()
    {
        m_pHealth = buffedPlayer.GetComponent<PlayerHealth>();
        m_pHealth.m_currentHealth += 1;
        yield return new WaitForSeconds(1f);
        m_pHealth.m_currentHealth += 1;
        yield return new WaitForSeconds(1f);
        m_pHealth.m_currentHealth += 1;
    }

    IEnumerator BuffBulletLife()
    {
        m_pShoot = buffedPlayer.GetComponent<PlayerShoot>();
        var temp = m_pShoot.m_bBounces;
        var temp2 = m_pShoot.m_bLifetime;

        m_pShoot.m_bBounces = buffedBulletBounces;
        m_pShoot.m_bLifetime = buffedBulletLifetime;

        yield return new WaitForSeconds(m_buffDuration);

        m_pShoot.m_bBounces = temp;
        m_pShoot.m_bLifetime = temp2;
    }

    IEnumerator BuffBurst()
    {
        m_pShoot = buffedPlayer.GetComponent<PlayerShoot>();
        var temp = m_pShoot.m_shotsPerBurst;
        m_pShoot.m_shotsPerBurst = buffedShotsPerBurst;
        yield return new WaitForSeconds(m_buffDuration);
        m_pShoot.m_shotsPerBurst = temp;
    }

    IEnumerator BuffSpeed()
    {
        m_pMotor = buffedPlayer.GetComponent<PlayerMotor>();

        var temp = m_pMotor.m_moveSpeed;
        var temp2 = m_pMotor.m_turretRotateSpeed;
        var temp3 = m_pMotor.m_chassisRotateSpeed;

        m_pMotor.m_moveSpeed = buffedSpeed;
        m_pMotor.m_turretRotateSpeed = buffedTurretRotation;
        m_pMotor.m_chassisRotateSpeed = buffedRotation;

        yield return new WaitForSeconds(m_buffDuration);

        m_pMotor.m_moveSpeed = temp;
        m_pMotor.m_turretRotateSpeed = temp2;
        m_pMotor.m_chassisRotateSpeed = temp3;
    }

    IEnumerator BuffBullet()
    {
        m_pShoot = buffedPlayer.GetComponent<PlayerShoot>();
        var temp = m_pShoot.m_bSpeed;
        m_pShoot.m_bSpeed = buffedBulletSpeed;
        yield return new WaitForSeconds(m_buffDuration);
        m_pShoot.m_bSpeed = temp;
    }


}
