using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


// This example shows setting a constant color value.
public class FXbuffOnPlayer : MonoBehaviour
{
    ParticleSystem myParticleSystem;
    ParticleSystem.MainModule effect;

    public Color green = new Color(0, 60, 0);
    public Color magenda = new Color(245, 19, 245);
    public Color red = new Color(255, 19, 19);
    public Color blue = new Color(21, 21, 131);
    public Color yellow = new Color(255, 237, 0);

    public Color color;

    void Start()
    {
        myParticleSystem = GetComponentInChildren<ParticleSystem>();
        effect = myParticleSystem.main;
        effect.startColor = new ParticleSystem.MinMaxGradient(color);
    }

    IEnumerator StupidUnity()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void SetRegenC()
    {
        color = green;
    }

    public void SetBulletLifeC()
    {
        color = magenda;
    }

    public void SetBurstC()
    {
        color = red;
    }

    public void SetSpeedC()
    {
        color = blue;
    }

    public void SetBSpeedC()
    {
        color = yellow;
    }
}