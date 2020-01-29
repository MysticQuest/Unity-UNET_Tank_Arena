using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    static GameManager instance;

    public Text m_messageText;

    public int m_minPlayers = 2;
    public int m_maxPlayers = 4;

    [SyncVar]
    public int m_playerCount = 0;

    // public Color[] m_playerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public List<PlayerManager> m_allPlayers;
    public List<Text> m_nameLabelText;
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

    void Start()
    {
        StartCoroutine("GameLoop");
    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine("EnterLobby");
        yield return StartCoroutine("PlayGame");
        yield return StartCoroutine("EndGame");
        StartCoroutine("GameLoop");
    }

    IEnumerator EnterLobby()
    {
        while (m_playerCount < m_minPlayers)
        {
            UpdateMessage("Waiting for players...");
            DisablePlayers();
            yield return null;
        }
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(1f);
        UpdateMessage("3");
        yield return new WaitForSeconds(1f);
        UpdateMessage("2");
        yield return new WaitForSeconds(1f);
        UpdateMessage("1");
        yield return new WaitForSeconds(1f);
        UpdateMessage("FIGHT!");
        yield return new WaitForSeconds(1f);


        EnablePlayers();
        UpdateScoreBoard();

        UpdateMessage("");

        m_winner = null;
        while (m_gameOver == false)
        {
            yield return null;
        }
    }

    IEnumerator EndGame()
    {
        DisablePlayers();
        UpdateMessage("GAME OVER \n " + m_winner.m_pSetup.m_playerNameText.text + " is victorious!");
        Reset();
        yield return new WaitForSeconds(3f);
        UpdateMessage("Restarting in 3...");
        yield return new WaitForSeconds(1f);
        UpdateMessage("Restarting in 2...");
        yield return new WaitForSeconds(1f);
        UpdateMessage("Restarting in 1...");
        yield return new WaitForSeconds(1f);
    }

    [ClientRpc]
    void RpcSetPlayerState(bool state)
    {
        PlayerManager[] allPlayers = GameObject.FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager p in allPlayers)
        {
            p.enabled = state;
        }
    }

    void EnablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(true);
        }

    }
    void DisablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(false);
        }
    }

    public void AddPlayer(PlayerSetup pSetup)
    {
        // if (m_playerCount < m_maxPlayers)
        // {
        //     m_allPlayers.Add(pSetup.GetComponent<PlayerManager>());
        //     pSetup.m_playerColor = m_playerColors[m_playerCount];
        //     pSetup.m_playerNum = m_playerCount + 1;
        // }
    }

    [ClientRpc]
    void RpcUpdateScoreBoard(string[] playerNames, int[] playerScores)
    {
        for (int i = 0; i < m_playerCount; i++)
        {
            if (playerNames[i] != null)
            {
                m_nameLabelText[i].text = playerNames[i];
            }

            m_playerScoreText[i].text = playerScores[i].ToString();

        }

    }

    public void UpdateScoreBoard()

    {
        if (isServer)
        {
            m_winner = GetWinner();
            if (m_winner != null)
            {
                m_gameOver = true;
            }

            string[] names = new string[m_playerCount];
            int[] scores = new int[m_playerCount];

            for (int i = 0; i < m_playerCount; i++)
            {
                names[i] = m_allPlayers[i].GetComponent<PlayerSetup>().m_playerNameText.text;
                scores[i] = m_allPlayers[i].m_score;
            }

            RpcUpdateScoreBoard(names, scores);
        }
    }

    [ClientRpc]
    void RpcUpdateMessage(string msg)
    {
        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(true);
            m_messageText.text = msg;
        }
    }

    public void UpdateMessage(string msg)
    {
        if (isServer)
        {
            RpcUpdateMessage(msg);
        }
    }

    PlayerManager GetWinner()
    {
        if (isServer)
        {
            for (int i = 0; i < m_playerCount; i++)
            {
                if (m_allPlayers[i].m_score >= m_maxScore)
                {
                    return m_allPlayers[i];
                }
            }
        }
        return null;
    }

    void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScoreBoard();
            m_winner = null;
            m_gameOver = false;
        }
    }

    [ClientRpc]
    void RpcReset()
    {
        PlayerManager[] allPlayers = GameObject.FindObjectsOfType<PlayerManager>();
        foreach (PlayerManager p in allPlayers)
        {
            p.m_score = 0;
            p.m_pHealth.m_currentHealth = p.m_pHealth.m_maxHealth;
        }
    }

}

