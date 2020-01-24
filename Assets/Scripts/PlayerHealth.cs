using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    [SyncVar(hook = "UpdateHealthBar")]
    public float m_currentHealth;

    public float m_maxHealth = 3;
    public bool m_isDead = false;

    public GameObject m_deathPrefab;

    public RectTransform m_healthBar;

    // Use this for initialization
    void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateHealthBar(float value)
    {
        if (m_healthBar != null)
        {
            m_healthBar.sizeDelta = new Vector2(value / m_maxHealth * 150f, m_healthBar.sizeDelta.y);
        }
    }

    public void Damage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        m_currentHealth -= damage;

        if (m_currentHealth <= 0 && !m_isDead)
        {
            m_isDead = true;
            Die();
        }
    }

    void Die()
    {
        if (m_deathPrefab)
        {
            GameObject deathFx = Instantiate(m_deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            GameObject.Destroy(deathFx, 3f);
        }
        SetActiveState(false);
        gameObject.SendMessage("Disable");
    }

    void SetActiveState(bool state)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = false;
        }
    }
}
