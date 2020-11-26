using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using BackEnd;
using BackEnd.Tcp;

public class WorldManager : MonoBehaviour
{
    private Stack<SessionId> gameRecord;
    static public WorldManager instance;
    private Dictionary<SessionId, Player> players;
    private SessionId myPlayerIndex = SessionId.None;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        var matchInstance = MatchManager.GetInstance();
        if(matchInstance = null)
        {
            return;
        }
        if(matchInstance.isReconnectProcess)
        {
            
        }
    }
    public bool InitializeGame()
    {
        Debug.Log("게임 초기화 진행");
        gameRecord = new Stack<SessionId>();

        myPlayerIndex = SessionId.None;

        return true;
    }
    //public void OnGameStart()
    //{
       
    //    if (MatchManager.GetInstance().IsHost())
    //    {
    //        Debug.Log("플레이어 세션정보 확인");

    //        if (MatchManager.GetInstance().IsSessionListNull())
    //        {
    //            Debug.Log("Player Index Not Exist!");
    //            // 호스트 기준 세션데이터가 없으면 게임을 바로 종료한다.
    //            foreach (var session in MatchManager.GetInstance().sessionIdList)
    //            {
    //                // 세션 순서대로 스택에 추가
    //                gameRecord.Push(session);
    //            }
               
    //            return;
    //        }
    //    }
    //    SetPlayerInfo();
    //}
    //public void SetPlayerInfo()
    //{
    //    if (MatchManager.GetInstance().sessionIdList == null)
    //    {
    //        // 현재 세션ID 리스트가 존재하지 않으면, 0.5초 후 다시 실행
    //        Invoke("SetPlayerInfo", 0.5f);
    //        return;
    //    }
    //    var gamers = MatchManager.GetInstance().sessionIdList;
    //    int size = gamers.Count;
    //    if (size <= 0)
    //    {
    //        Debug.Log("No Player Exist!");
    //        return;
    //    }
    //    if (size > 2)
    //    {
    //        Debug.Log("Player Pool Exceed!");
    //        return;
    //    }

    //    players = new Dictionary<SessionId, Player>();
    //    BackEndMatchManager.GetInstance().SetPlayerSessionList(gamers);

    //    int index = 0;
    //    foreach (var sessionId in gamers)
    //    {
    //        GameObject player = Instantiate(playerPrefeb, new Vector3(statringPoints[index].x, statringPoints[index].y, statringPoints[index].z), Quaternion.identity, playerPool.transform);
    //        players.Add(sessionId, player.GetComponent<Player>());

    //        if (BackEndMatchManager.GetInstance().IsMySessionId(sessionId))
    //        {
    //            myPlayerIndex = sessionId;
    //            players[sessionId].Initialize(true, myPlayerIndex, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[index].w);
    //        }
    //        else
    //        {
    //            players[sessionId].Initialize(false, sessionId, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[index].w);
    //        }
    //        index += 1;
    //    }
    //    Debug.Log("Num Of Current Player : " + size);

    //    // 스코어 보드 설정
    //    alivePlayer = size;
    //    InGameUiManager.GetInstance().SetScoreBoard(alivePlayer);

    //    if (BackEndMatchManager.GetInstance().IsHost())
    //    {
    //        StartCoroutine("StartCount");
    //    }
    //}
    //// Update is called once per frame
    void Update()
    {
        
    }
}
