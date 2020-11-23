using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public partial class MatchManager : MonoBehaviour
{
    private bool inSetHost = false;
    private MatchGameResult matchGameResult;

    // Start is called before the first frame update
    public void OnGameReady()
    {
        if(inSetHost == false)
        {
            inSetHost = SetHostSession();
        }
        Debug.Log("호스트 설정 완료");
        if(IsHost() == true)
        {
            Invoke("ReadyToLoadRoom",0.5f);
        }
    }
    private void OnGameRecoonect()
    {
        localQueue = null;
        Debug.Log("재접속 프로세스 진행중");
    }
    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        sessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach(var record in args.GameRecords)
        {
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        sessionIdList.Sort();
    }
    public void LeaveInGameRoom()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
    }
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if(isReconnectProcess)
        {
            Debug.Log("재접속 프로세스 진행중 ");
            return;
        }
        Debug.Log(string.Format("성공", args.ErrInfo));
        if(args.ErrInfo != ErrorCode.Success)
        {
            var errorLog = string.Format("접속실패", args.ErrInfo, args.Reason);
            Debug.Log(errorLog);
            LeaveInGameRoom();
            return;
        }
        var record = args.GameRecord;
        Debug.Log(string.Format(string.Format("인게임 접속 유저 정보 [{0}] : {1} ",args.GameRecord,args.GameRecord.m_nickname)));
        if (!sessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId,record);
            Debug.Log(string.Format("인게임내 세션 갯수" + sessionIdList.Count));
        }

    }
  

    private void AccessInGameRoom(string roomToken)
    {
        Backend.Match.JoinGameRoom(roomToken);
    }
    
   
    private void GameSetUp()
    {
        Debug.Log("게임 시작 메시지 수신. 게임 설정 시작");
        OnGameReady();

        Invoke("ReadyToLoadRoom", 0.5f);

    }
    private void ReadyToLoadRoom()
    {
        Debug.Log("1초 후 룸 씬 전환 메시지 송신");
        Invoke("SendChangeRoomScene", 1f);
    }
    private void SendChangeRoomScene()
    {
        Debug.Log("룸씬 전환 메세지 송신");
        SendDataToInGame(new Protocol.LoadRoomSceneMessage());
    }
    private void SendChangeGameScene()
    {
        Debug.Log("룸씬 전환 메세지 송신");
        SendDataToInGame(new Protocol.LoadGameSceneMessage());
    }
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }
    public void MatchGameOver(Stack<SessionId> record)
    {
        if(nowModeType == MatchModeType.OneOnOne)
        {
            matchGameResult = OneOnOneRecord(record);
        }
        else
        {
            Debug.LogError("알수없는 매치모드 타입" +  nowModeType);
            return;
        }
        RemoveAISessionInGameResult();
        Backend.Match.MatchEnd(matchGameResult);

    }
    private void RemoveAISessionInGameResult()
    {
        string str = string.Empty;
        List<SessionId> aiSession = new List<SessionId>();
        if (matchGameResult.m_winners != null)
        {
            str += "승자 : ";
            foreach (var tmp in matchGameResult.m_winners)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_winners.RemoveAll(aiSession.Contains);
        }

        aiSession.Clear();
        if (matchGameResult.m_losers != null)
        {
            str += "패자 : ";
            foreach (var tmp in matchGameResult.m_losers)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_losers.RemoveAll(aiSession.Contains);
        }
        Debug.Log(str);
    }
    private MatchGameResult OneOnOneRecord(Stack<SessionId> record)
    {
        MatchGameResult nowGameResult = new MatchGameResult();

        nowGameResult.m_winners = new List<SessionId>();
        nowGameResult.m_winners.Add(record.Pop());

        nowGameResult.m_losers = new List<SessionId>();
        nowGameResult.m_losers.Add(record.Pop());

        nowGameResult.m_draws = null;

        return nowGameResult;
    }
    public void SetPlayerSessionList(List<SessionId>sessions)
    {
        sessionIdList = sessions;
    }
    private void SendGameSyncMessage()
    {
        // 현재 게임 상황 (위치, hp 등등...)
      //  var message = WorldManager.instance.GetNowGameState(hostSession);
      //  SendDataToInGame(message);
    }
}
