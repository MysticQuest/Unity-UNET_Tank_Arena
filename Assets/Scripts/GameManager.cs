using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    static GameManager instance;

    public Text m_messageText;

    int m_minPlayers = 1;
    int m_maxPlayers = 4;

    [SyncVar]
    public int m_playerCount = 0;

    public Color[] m_playerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public List<PlayerController> m_allPlayers;
    public List<Text> m_nameLabelText;
    public List<Text> m_playerScoreText;

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
    }

    IEnumerator EnterLobby()
    {

        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(true);
            m_messageText.text = "Waiting for Players";
        }

        while (m_playerCount < m_minPlayers)
        {
            DisablePlayers();
            yield return null;
        }
    }

    IEnumerator PlayGame()
    {
        EnablePlayers();
        UpdateScoreBoard();

        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(false);
        }

        yield return null;
    }

    IEnumerator EndGame()
    {
        yield return null;
    }

    void SetPlayerState(bool state)
    {
        PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController p in allPlayers)
        {
            p.enabled = state;
        }
    }

    void EnablePlayers()
    {
        SetPlayerState(true);
    }
    void DisablePlayers()
    {
        SetPlayerState(false);
    }

    public void AddPlayer(PlayerSetup pSetup)
    {
        if (m_playerCount < m_maxPlayers)
        {
            m_allPlayers.Add(pSetup.GetComponent<PlayerController>());
            pSetup.m_playerColor = m_playerColors[m_playerCount];
            pSetup.m_playerNum = m_playerCount + 1;
        }
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

}

