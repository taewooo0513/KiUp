using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
public partial class MatchManager : MonoBehaviour
{
    public MatchType nowmatchType { get; private set; } = MatchType.None;
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None;
    public void JoinMatchServer()
    {
        if(isConnectMatchServer)
        {
            return;
        }
        ErrorInfo errorInfo;
        isConnectMatchServer = true;
        if(!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            var errorLog = string.Format("실패", errorInfo.ToString());
            Debug.Log(errorLog);
        }
    }
    public void MakeRoom()
    {
        MatchManager.GetInstance().CreateMatchRoom();
    }
    public void LeaveMatchServer()
    {
        isConnectMatchServer = false;
        Backend.Match.LeaveMatchMakingServer();
    }
    public void SetNowMatchInfo(MatchType matchType, MatchModeType matchModeType)
    {
        nowmatchType = matchType;
        nowModeType = matchModeType;
        Debug.Log(string.Format("매칭 타입/모드 : {0}/{1}", nowmatchType, nowModeType));

    }
    public bool CreateMatchRoom()
    {
        if(!isConnectMatchServer)
        {
            Debug.Log("매칭중");
            JoinMatchServer();
            return false ;
        }
        Debug.Log("방 생성 요청을 서버로 보냄");
        Backend.Match.CreateMatchRoom();
        return true;
    }
    public void _LeaveMatcRoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
    public void RequestMatchMaking(int index)
    {
        if(!isConnectMatchServer)
        {
            Debug.Log("서버연결 실패");
            Debug.Log("서버에 재연결중");
            JoinMatchServer();
            return;
        }
        isConnectInGameServer = false;
        Backend.Match.RequestMatchMaking(MatchInfos[index].matchType,MatchInfos[index].matchModeType,MatchInfos[index].indate);
        if(isConnectInGameServer)
        {
            Backend.Match.LeaveGameServer();
        }
    }
    public void CancelRegistMatchMakin()
    {
        Backend.Match.CancelMatchMaking();
    }
    private void Update()
    {
        if(isConnectInGameServer || isConnectMatchServer)
        {
            Backend.Match.Poll();
            if(localQueue != null)
            {
                while(localQueue.Count > 0)
                {
                    var msg = localQueue.Dequeue();
                    
                }
            }
        }
    }
    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        string debugLog = string.Empty;
        switch(args.ErrInfo)
        {
            case ErrorCode.Success ://매칭성공시
                debugLog = string.Format("성공", args.Reason);

                break;
            case ErrorCode.Match_InProgress://매치 중일때 재신청하면
                if(args.Reason == string.Empty)
                {
                    debugLog = "성공";
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled://매칭 취소했을때
                debugLog = string.Format("취소", args.Reason);

                break;
                
        }
    }
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format("서버 접속 실패", errInfo.ToString());
            // 접속 실패
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            Debug.Log("서버 접속 성공");
        }
    }

}
