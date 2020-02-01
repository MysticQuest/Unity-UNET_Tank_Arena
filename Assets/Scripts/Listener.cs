using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Listener : MonoBehaviour
{

    public AudioListener listener;
    // Use this for initialization
    void Start()
    {

        listener = GetComponent<AudioListener>();
        listener.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
