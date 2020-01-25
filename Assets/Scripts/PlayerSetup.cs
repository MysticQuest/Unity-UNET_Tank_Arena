using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{

    [SyncVar(hook = "UpdateColor")]
    public Color m_playerColor;
    public string m_baseName = "PLAYER";

    [SyncVar(hook = "UpdateName")]
    public int m_playerNum = 1;
    public Text m_playerNameText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (m_playerNameText != null)
        {
            m_playerNameText.enabled = false;
        }
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            UpdateName(m_playerNum);
            UpdateColor(m_playerColor);
        }
    }

    void UpdateColor(Color pColor)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in meshes)
        {
            r.material.color = pColor;
        }
    }

    void UpdateName(int pNum)
    {
        if (m_playerNameText != null)
        {
            m_playerNameText.enabled = true;
            m_playerNameText.text = m_baseName + pNum.ToString();
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // UpdateColor();
        // UpdateName();
        CmdSetupPlayer();
    }

    [Command]
    void CmdSetupPlayer()
    {
        GameManager.Instance.AddPlayer(this);
        GameManager.Instance.m_playerCount++;
    }
}
