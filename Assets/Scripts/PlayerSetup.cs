using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{

    [SyncVar(hook = "UpdateColor")]
    public Color m_playerColor;

    [SyncVar(hook = "UpdateName")]
    public string m_name = "PLAYER";

    public Text m_playerNameText;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    void Start()
    {
        UpdateName(m_name);
        UpdateColor(m_playerColor);
    }

    void UpdateColor(Color pColor)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in meshes)
        {
            r.material.color = pColor;
        }
    }

    void UpdateName(string name)
    {
        if (m_playerNameText != null)
        {
            m_playerNameText.enabled = true;
            m_playerNameText.text = m_name;
        }
    }

    // public override void OnStartLocalPlayer()
    // {
    //     base.OnStartLocalPlayer();
    //     // UpdateColor();
    //     // UpdateName();
    //     CmdSetupPlayer();
    // }

    // [Command]
    // void CmdSetupPlayer()
    // {
    //     GameManager.Instance.AddPlayer(this);
    //     GameManager.Instance.m_playerCount++;
    // }
}
