﻿using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using Protocol;
using Battlehub.Dispatcher;
using System.Linq;

public partial class MatchManager : MonoBehaviour
{
    public class ServerInfo
    {
        public string host;
        public ushort port;
        public string roomToken;
    }


    public class MatchInfo
    {
        public string indate;
        public MatchType matchType;
        public MatchModeType matchModeType;
        public string HeadCount;
    }
    private ServerInfo roomInfo = null;             // 게임 룸 정보

    private static MatchManager Instance = null;
    public List<MatchInfo> MatchInfos { get; private set; } = new List<MatchInfo>();
    public bool isConnectMatchServer { get; private set; } = false;
    public bool isReconnectProcess { get; private set; } = false;
    private int numOfClient = 2;                    // 매치에 참가한 유저의 총 수
    private bool isHost = false;
    private bool isJoinGameRoom = false;
    public SessionId hostSession { get; private set; }  // 호스트 세션

    private bool isConnectInGameServer = false;
    public List<SessionId> sessionIdList { get; private set; }  // 매치에 참가중인 유저들의 세션 목록
    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // 매치에 참가중인 유저들의 매칭 기록
    private Queue<KeyMessage> localQueue = null;    // 호스트에서 로컬로 처리하는 패킷을 쌓아두는 큐 (로컬처리하는 데이터는 서버로 발송 안함)
    public bool isReconnectEnable { get; private set; } = false;

    private string inGameRoomToken = string.Empty;  // 게임 룸 토큰 (인게임 접속 토큰)
    public bool isSandBoxGame { get; private set; } = false;

    public void GetMatchList(Action<bool, string> func)
    {
        Backend.Match.GetMatchList(callback =>
        {
            if (callback.IsSuccess() == false)
            {
                Debug.Log("매칭 카드 불러오기 실패");
                Dispatcher.Current.BeginInvoke(() =>
                {
                    GetMatchList(func);
                }
                );
                return;
            }
            foreach (LitJson.JsonData row in callback.Rows())
            {
                MatchInfo info = new MatchInfo();
                info.indate = row["inDate"]["S"].ToString();
                foreach (MatchType type in Enum.GetValues(typeof(MatchType)))
                {
                    info.matchType = type;
                }
                foreach (MatchModeType type in Enum.GetValues(typeof(MatchModeType)))
                {
                    info.matchModeType = type;
                }

                MatchInfos.Add(info);
            }
            Debug.Log("매치 카드 생성 완료");
            func(true, string.Empty);
        });
    }
    public MatchInfo GetMatchInfo(string indate)
    {
        var result = MatchInfos.FirstOrDefault(x => x.indate == indate);
        if (result.Equals(default(MatchInfo)) == true)
        {
            return null;
        }
        return result;
    }
    // Start is called before the first frame update
    public static MatchManager GetInstance()
    {
        if (!Instance)
        {
            return null;
        }
        return Instance;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    public void IsMatchGameActivate()
    {
        roomInfo = null;
        isReconnectEnable = false;
        JoinMatchServer();

    }

    private void Start()
    {

        ExceptionHandler();
        MatchMakingHandler();
        GameHandle();
    }
    private void ExceptionHandler()
    {
        // 예외가 발생했을 때 호출
        Backend.Match.OnException += (args) =>
        {
            Debug.Log(args);
        };
    }
    public void AddMsgToLocalQueue(KeyMessage message)
    {
        // 로컬 큐에 메시지 추가
        if (isHost == false || localQueue == null)
        {
            return;
        }

        localQueue.Enqueue(message);
    }
    private void OnApplicationQuit()
    {
        if (isConnectMatchServer)
        {
            LeaveMatchServer();
            Debug.Log("ApplicationQuit - LeaveMatchServer");

        }
    }

    private void MatchMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("서버 접속중" + args.ErrInfo);
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // 매칭 신청 관련 작업에 대한 호출
            ProcessMatchMakingResponse(args);
        };

        Backend.Match.OnLeaveMatchMakingServer += (args) =>
        {
            // 매칭 서버에서 접속 종료할 때 호출
            Debug.Log("OnLeaveMatchMakingServer : " + args.ErrInfo);
            isConnectMatchServer = false;

            if (args.ErrInfo.Category.Equals(ErrorCode.DisconnectFromRemote) || args.ErrInfo.Category.Equals(ErrorCode.Exception)
                || args.ErrInfo.Category.Equals(ErrorCode.NetworkTimeout))
            {
                // 서버에서 강제로 끊은 경우
                //if (LobbyUI.GetInstance())
                {
                    //LobbyUI.GetInstance().MatchRequestCallback(false);
                    //LobbyUI.GetInstance().CloseRoomUIOnly();
                    //LobbyUI.GetInstance().SetErrorObject("매칭서버와 연결이 끊어졌습니다.\n\n" + args.ErrInfo.Reason);
                }
            }
        };
    }

    public bool IsHost()
    {
        return isHost;
    }
    public bool isMySessionId(SessionId session)
    {
        return Backend.Match.GetMySessionId() == session;
    }
    private bool SetHostSession()
    {
        Debug.Log("호스트 세션 설정 진입");
        sessionIdList.Sort();
        isHost = false;
        foreach (var record in gameRecords)
        {
            if (record.Value.m_isSuperGamer == true)
            {
                if (record.Value.m_sessionId.Equals(Backend.Match.GetMySessionId()))
                {
                    isHost = true;
                }
                hostSession = record.Value.m_sessionId;
                break;
            }
            Debug.Log("호스트 여부 : " + isHost);
            if (isHost)
            {
                localQueue = new Queue<KeyMessage>();
            }
            else
            {
                localQueue = null;
            }
        }
        return true;
    }
    public bool IsSessionListNull()
    {
        return sessionIdList == null || sessionIdList.Count == 0;

    }
    public bool IsMySessionId(SessionId session)
    {
        return Backend.Match.GetMySessionId() == session;
    }
    public string GetNickNameBySessionId(SessionId session)
    {
        return gameRecords[session].m_nickname;
    }
    private void MatcgMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            ProcessMatchMakingResponse(args);
        };
        Backend.Match.OnMatchRelay += (args) =>
        {
            // 각 클라이언트들이 서버를 통해 주고받은 패킷들
            // 서버는 단순 브로드캐스팅만 지원 (서버에서 어떠한 연산도 수행하지 않음)

            // 게임 사전 설정
            //if (PrevGameMessage(args.BinaryUserData) == true)
            //{
            //    // 게임 사전 설정을 진행하였으면 바로 리턴
            //    return;
            //}ProcessMatchMakingResponse

            //if (WorldManager.instance == null)ProcessMatchMakingResponse
            //{
            //    // 월드 매니저가 존재하지 않으면 바로 리턴
            //    return;
            //}

            //WorldManager.instance.OnRecieve(args);
        };
       
        Backend.Match.OnLeaveMatchMakingServer += (args) =>
        {
            Debug.Log("OnLeaveMatchMakingServer : " + args.ErrInfo);

            isConnectMatchServer = false;
            if (args.ErrInfo.Category.Equals(ErrorCode.DisconnectFromRemote) || args.ErrInfo.Category.Equals(ErrorCode.NetworkTimeout))
            {

            }
        };
        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate + " + args.ErrInfo + " : " + args.Reason);
        };

        Backend.Match.OnMatchMakingRoomJoin += (args) =>
        {
            Debug.Log(string.Format("OnMatchMakingRoomJoin : {0} : {1}", args.ErrInfo, args.Reason));
            if (args.ErrInfo.Equals(ErrorInfo.Success))
            {
                Debug.Log("user join in Room : " + args.UserInfo.m_nickName);
            }
        };
        Backend.Match.OnMatchMakingRoomUserList += (args) =>
        {
            Debug.Log(string.Format("OnMatchMakingRoomUserList : {0} : {1}", args.ErrInfo, args.Reason));
            List<MatchMakingUserInfo> UserList = null;
            if (args.ErrInfo.Equals(ErrorCode.Success))
            {
                UserList = args.UserInfos;
                Debug.Log("ready room user count : " + UserList.Count);
            }

        };

        Backend.Match.OnMatchMakingRoomLeave += (args) =>
        {
            Debug.Log(string.Format("OnMatchMakingRoomLeave : {0} : {1}", args.ErrInfo, args.Reason));
            if (args.ErrInfo.Equals(ErrorCode.Success) || args.ErrInfo.Equals(ErrorCode.Match_Making_KickedByOwner))
            {
                Debug.Log("user leave in room : " + args.UserInfo.m_nickName);
                if (args.UserInfo.m_nickName.Equals(serverManager.GetInstance().myNickName))
                {

                }
                Debug.Log("자기사인이 방에서 나갔습니다.");
                return;
            }
        };
        Backend.Match.OnMatchMakingRoomDestory += (args) =>
        {
            Debug.Log(string.Format("OnMatchMakingRoomDestory : {0} : {1}", args.ErrInfo, args.Reason));
        };
    }
    private void GameHandle()
    {
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Debug.Log("OnSessionJoinInServer : " + args.ErrInfo);
            if (args.ErrInfo != ErrorInfo.Success)
            {

                if (isReconnectProcess)
                {
                    if (args.ErrInfo.Reason.Equals("Reconnect Success"))
                    {
                        //GameManager.GetInstance().ChangeState(GameManager.GameState.Reconnect);
                        Debug.Log("재접속 성공");
                    }
                    else if (args.ErrInfo.Reason.Equals("Fail to Reconnect"))
                    {
                        Debug.Log("재접속 실패");
                        JoinMatchServer();
                        isConnectMatchServer = false;
                    }
                }
                return;
            }
            if (isJoinGameRoom)
            {
                return;
            }
            if (inGameRoomToken == string.Empty)
            {
                Debug.LogError("룸토큰없");
                return;
            }
            Debug.Log("인게임 서버 접속 성공");
            isJoinGameRoom = true;
            AccessInGameRoom(inGameRoomToken);
        };
        Backend.Match.OnSessionListInServer += (args) =>
        {
            Debug.Log("OnSessionListInServer : " + args.ErrInfo);

            ProcessMatchInGameSessionList(args);
        };
        Backend.Match.OnMatchInGameAccess += (args) =>
        {
            Debug.Log("OnMatchInGameAccess : " + args.ErrInfo);
            ProcessMatchInGameAccess(args);

        };
        Backend.Match.OnMatchInGameStart += () =>
        {
            GameSetUp();
            //
        };
        Backend.Match.OnMatchResult += (args) =>
        {
            Debug.Log("게임 결과값 업로드 결과 : " + string.Format("{ 0 } : {1}", args.ErrInfo, args.Reason));
            if (args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {

            }
            else if (args.ErrInfo == BackEnd.Tcp.ErrorCode.Match_InGame_Timeout)
            {
                Debug.Log("게임 입장 실패 : " + args.ErrInfo);
                //LobbyUI.GetInstance().MatchCancelCallback();
            }
            else
            {
                //InGameUiManager.instance.SetGameResult("결과 종합 실패\n호스트와 연결이 끊겼습니다.");
                Debug.Log("게임 결과 업로드 실패 : " + args.ErrInfo);
            }
            sessionIdList = null;
        };
        Backend.Match.OnMatchRelay += (args) =>
        {
        };
        Backend.Match.OnLeaveInGameServer += (args) =>
        {
            Debug.Log("OnLeaveInGameServer" + args.ErrInfo + " : " + args.Reason);
            if (args.Reason.Equals("Fail To Reconnect"))
            {
                JoinMatchServer();
            }
            isConnectInGameServer = false;
        };

    }
}
