using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour
{
    static GameManager instance;

    public Text m_messageText;

    // public int m_minPlayers = 2;
    // public int m_maxPlayers = 4;

    // [SyncVar]
    // public int m_playerCount = 0;

    // public Color[] m_playerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public static List<PlayerManager> m_allPlayers = new List<PlayerManager>();
    public List<Text> m_playerNameText;
    public List<Text> m_playerScoreText;

    public int m_maxScore = 3;

    [SyncVar]
    bool m_gameOver = false;
    PlayerManager m_winner;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<GameManager>();
                }
            }
            return instance;
        }

        set
        {

        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    void Start()
    {
        StartCoroutine("GameLoop");
    }

    IEnumerator GameLoop()
    {
        LobbyManager lobbyManager = LobbyManager.s_Singleton;

        if (lobbyManager != null)
        {
            while (m_allPlayers.Count < lobbyManager._playerNumber)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine("StartGame");
            yield return StartCoroutine("PlayGame");
            yield return StartCoroutine("EndGame");
            StartCoroutine("GameLoop");
        }
        else
        {
            Debug.LogWarning("Please launch the game from the lobby");
        }
    }

    [ClientRpc]
    void RpcStartGame()
    {
        UpdateMessage("FIGHT!");
        DisablePlayers();
    }

    IEnumerator StartGame()
    {
        Reset();
        RpcStartGame();
        UpdateScoreBoard();
        yield return new WaitForSeconds(1f);
    }

    [ClientRpc]
    void RpcPlayGame()
    {
        EnablePlayers();
        UpdateMessage("");
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(1f);

        RpcPlayGame();

        while (m_gameOver == false)
        {
            CheckScores();
            yield return null;
        }
    }

    [ClientRpc]
    void RpcEndGame()
    {
        DisablePlayers();
    }

    IEnumerator EndGame()
    {
        RpcEndGame();
        RpcUpdateMessage(m_winner.m_pSetup.m_name + " is victorious!");
        yield return new WaitForSeconds(3f);
        Reset();

        LobbyManager.s_Singleton._playerNumber = 0;
        LobbyManager.s_Singleton.SendReturnToLobby();
    }

    // [ClientRpc]
    // void RpcSetPlayerState(bool state)
    // {
    //     PlayerManager[] allPlayers = GameObject.FindObjectsOfType<PlayerManager>();
    //     foreach (PlayerManager p in allPlayers)
    //     {
    //         p.enabled = state;
    //     }
    // }

    void EnablePlayers()
    {
        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            if (m_allPlayers[i] != null)
            {
                m_allPlayers[i].EnableControls();
            }
        }

    }
    void DisablePlayers()
    {
        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            if (m_allPlayers[i] != null)
            {
                m_allPlayers[i].DisableControls();
            }
        }
    }

    // public void AddPlayer(PlayerSetup pSetup)
    // {
    //     // if (m_playerCount < m_maxPlayers)
    //     // {
    //     //     m_allPlayers.Add(pSetup.GetComponent<PlayerManager>());
    //     //     pSetup.m_playerColor = m_playerColors[m_playerCount];
    //     //     pSetup.m_playerNum = m_playerCount + 1;
    //     // }
    // }


    [ClientRpc]
    void RpcUpdateScoreBoard(string[] playerNames, int[] playerScores)
    {
        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            if (playerNames[i] != null)
            {
                m_playerNameText[i].text = playerNames[i];
            }
            // if (playerScores[i] != null)
            // {
            m_playerScoreText[i].text = playerScores[i].ToString();
            //}
        }

    }

    [Server]
    public void UpdateScoreBoard()
    {
        string[] pNames = new string[m_allPlayers.Count];
        int[] pScores = new int[m_allPlayers.Count];

        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            if (m_allPlayers[i] != null)
            {
                pNames[i] = m_allPlayers[i].GetComponent<PlayerSetup>().m_name;
                pScores[i] = m_allPlayers[i].m_score;
            }
        }
        RpcUpdateScoreBoard(pNames, pScores);
    }

    [ClientRpc]
    void RpcUpdateMessage(string msg)
    {
        UpdateMessage(msg);
    }

    public void UpdateMessage(string msg)
    {
        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(true);
            m_messageText.text = msg;
        }
    }

    public void CheckScores()
    {
        m_winner = GetWinner();
        if (m_winner != null)
        {
            m_gameOver = true;
        }
    }

    PlayerManager GetWinner()
    {
        m_maxScore = m_allPlayers.Count * 2 - 1;
        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            if (m_allPlayers[i].m_score >= m_maxScore)
            {
                return m_allPlayers[i];
            }
        }

        return null;
    }

    void Reset()
    {
        for (int i = 0; i < m_allPlayers.Count; i++)
        {
            PlayerHealth pHealth = m_allPlayers[i].GetComponent<PlayerHealth>();
            pHealth.Reset();

            m_allPlayers[i].m_score = 0;
        }
    }

    // [ClientRpc]
    // void RpcReset()
    // {
    //     PlayerManager[] allPlayers = GameObject.FindObjectsOfType<PlayerManager>();
    //     foreach (PlayerManager p in allPlayers)
    //     {
    //         p.m_score = 0;
    //         p.m_pHealth.m_currentHealth = p.m_pHealth.m_maxHealth;
    //     }
    // }

}

