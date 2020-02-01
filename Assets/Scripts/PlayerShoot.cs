using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{

    public Rigidbody m_bulletPrefab;
    public Transform m_bulletSpawn;

    public GameObject buffObj;
    public Buff buff;

    [SyncVar]
    public int m_shotsPerBurst = 1;
    int m_shotsLeft;
    bool m_isReloading;
    public float m_reloadTime = 2f;

    [SyncVar]
    public int m_bSpeed = 15;
    [SyncVar]
    public int m_bBounces = 3;
    [SyncVar]
    public float m_bLifetime = 5;

    public ParticleSystem m_missFireEffect;
    public LayerMask m_obstacleMask;

    bool m_canShoot = false;

    public AudioSource sound;
    public AudioClip cannon;

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public float shootVol;

    public Image CD;

    // Use this for initialization
    void Start()
    {
        m_shotsLeft = m_shotsPerBurst;
        m_isReloading = false;

        // buffObj = GameObject.FindGameObjectWithTag("Buff");
        // buff = buffObj.GetComponent<Buff>();
        sound = GetComponent<AudioSource>();

        hotSpot = new Vector2(cursorTexture.width / 1.3f, cursorTexture.height / 1.3f);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        CD = GameObject.FindGameObjectWithTag("CD").GetComponent<Image>();
        CD.fillAmount = 0;
    }

    void Update()
    {
        if (m_isReloading)
        {
            CD.fillAmount += 1f / m_reloadTime * Time.deltaTime;
        }
    }

    public void Enable()
    {
        m_canShoot = true;
    }

    public void Disable()
    {
        m_canShoot = false;
    }

    public void Shoot()
    {
        if (m_isReloading || m_bulletPrefab == null || !m_canShoot)
        {
            return;
        }

        RaycastHit hit;
        Vector3 center = new Vector3(transform.position.x, m_bulletSpawn.position.y, transform.position.z);
        Vector3 dir = (m_bulletSpawn.position - center).normalized;

        if (Physics.SphereCast(center, 0.25f, dir, out hit, 2.5f, m_obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (m_missFireEffect != null)
            {
                ParticleSystem effect = Instantiate(m_missFireEffect, hit.point, Quaternion.identity) as ParticleSystem;
                effect.Stop();
                effect.Play();
                Destroy(effect.gameObject, 3f);
            }

        }
        else
        {
            CmdShoot();

            m_shotsLeft--;
            if (m_shotsLeft <= 0)
            {

                StartCoroutine("Reload");
            }
        }
    }

    [Command]
    void CmdShoot()
    {
        Bullet bullet = null;
        // bullet = m_bulletPrefab.GetComponent<Bullet>();

        Rigidbody rbody = Instantiate(m_bulletPrefab, m_bulletSpawn.position, m_bulletSpawn.rotation) as Rigidbody;
        bullet = rbody.gameObject.GetComponent<Bullet>();

        if (rbody != null)
        {
            RpcShootSound();
            rbody.velocity = m_bSpeed * m_bulletSpawn.transform.forward;
            bullet.m_owner = GetComponent<PlayerManager>();
            NetworkServer.Spawn(rbody.gameObject);
        }
    }

    [ClientRpc]
    void RpcShootSound()
    {
        sound.PlayOneShot(cannon, shootVol);
    }

    IEnumerator Reload()
    {
        m_shotsLeft = m_shotsPerBurst;
        m_isReloading = true;
        yield return new WaitForSeconds(m_reloadTime);
        CD.fillAmount = 0;
        m_isReloading = false;
    }

    // public IEnumerator BuffBullet()
    // {
    //     var tempBSpeed = m_bSpeed;
    //     m_bSpeed = buff.buffedBulletSpeed;
    //     yield return new WaitForSeconds(buff.m_buffDuration);
    //     m_bSpeed = tempBSpeed;
    // }

    // public IEnumerator BuffBurst()
    // {
    //     var tempBurst = m_shotsPerBurst;
    //     m_shotsPerBurst = buff.buffedShotsPerBurst;
    //     yield return new WaitForSeconds(buff.m_buffDuration);
    //     m_shotsPerBurst = tempBurst;
    // }

    // public IEnumerator BuffBulletLife()
    // {

    //     var tempBounce = m_bBounces;
    //     var tempLifetime = m_bLifetime;

    //     m_bBounces = buff.buffedBulletBounces;
    //     m_bLifetime = buff.buffedBulletLifetime;

    //     yield return new WaitForSeconds(buff.m_buffDuration);

    //     m_bBounces = tempBounce;
    //     m_bLifetime = tempLifetime;
    // }
}
