using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;
using UnityEngine.SocialPlatforms;
using System;
using UnityEngine.SceneManagement;
using BackEnd.Tcp;
public class Matching : MonoBehaviour
{


    ErrorInfo error;
    JoinChannelEventArgs Ev;
    public void GameMatching()
    {
        Backend.Match.OnJoinMatchMakingServer = (args) => { Debug.Log(args); };
        ErrorInfo errorInfo; Backend.Match.JoinMatchMakingServer(out errorInfo);
        Debug.Log(errorInfo);

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
