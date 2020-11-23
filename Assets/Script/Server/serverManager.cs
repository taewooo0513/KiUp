using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;
using UnityEngine.SocialPlatforms;
using System;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using Battlehub.Dispatcher;

public class serverManager : MonoBehaviour
{
    
    private static serverManager instance;   // 인스턴스
    public bool isLogin { get; private set; }   // 로그인 여부
    BackEnd.Tcp.MatchMakingInteractionEventArgs args;
    private string tempNickName;                        // 설정할 닉네임 (id와 동일)
    public string myNickName { get; private set; } = string.Empty;  // 로그인한 계정의 닉네임
    public string myIndate { get; private set; } = string.Empty;    // 로그인한 계정의 inDate
    private Action<bool, string> loginSuccessFunc = null;

    private const string BackendError = "statusCode : {0}\nErrorCode : {1}\nMessage : {2}";
    // Start is called before the first frame update


  
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
    }
    public static serverManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackEndServerManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }
    void Start()
    {
        isLogin = false;
        try
        {
            Backend.Initialize(() =>
            {
                if (Backend.IsInitialized)
                {

                    // 비동기 함수 큐 초기화
                    StartSendQueue(true);
                }
                else
                {
                    Debug.Log("뒤끝 초기화 실패");
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log("[예외]뒤끝 초기화 실패\n" + e.ToString());
        }
    }


    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        StopSendQueue();
    }

    // Update is called once per frame
    // 뒤끝 토큰으로 로그인
    public void BackendTokenLogin(Action<bool, string> func)
    {
        Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("토큰 로그인 성공");
                loginSuccessFunc = func;
                OnPrevBackendAuthorized();
                return;
            }

            Debug.Log("토큰 로그인 실패\n" + callback.ToString());
            func(false, string.Empty);
        });
    }

    void OnApplicationPause(bool isPause)
    {
        Debug.Log("OnApplicationPause : " + isPause);
        if (isPause == false)
        {
            ResumeSendQueue();
        }
        else
        {
            PauseSendQueue();
        }
    }
    public void CustomLogin(string id, string pw, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.CustomLogin, id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("커스텀 로그인 성공");
                loginSuccessFunc = func;
                OnPrevBackendAuthorized();

                return;
            }

            Debug.Log("커스텀 로그인 실패\n" + callback);
            func(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }
    public void CustomSignIn(string id, string pw, Action<bool, string> func)
    {
        tempNickName = id;
        Enqueue(Backend.BMember.CustomSignUp, id, pw, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("커스텀 회원가입 성공");
                loginSuccessFunc = func;

                OnPrevBackendAuthorized();
                return;
            }

            Debug.LogError("커스텀 회원가입 실패\n" + callback.ToString());
            func(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }
    public void UpdateNickname(string nickname, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.UpdateNickname, nickname, bro =>
        {
            // 닉네임이 없으면 매치서버 접속이 안됨
            if (!bro.IsSuccess())
            {
                Debug.LogError("닉네임 생성 실패\n" + bro.ToString());
                func(false, string.Format(BackendError,
                    bro.GetStatusCode(), bro.GetErrorCode(), bro.GetMessage()));
                return;
            }
            loginSuccessFunc = func;
            OnBackendAuthorized();
        });
    }
    private void OnPrevBackendAuthorized()
    {
        isLogin = true;

        OnBackendAuthorized();
    }
    private void OnBackendAuthorized()
    {
        Enqueue(Backend.BMember.GetUserInfo, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("유저 정보 불러오기 실패\n" + callback);
                loginSuccessFunc(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
                return;
            }
            Debug.Log("유저정보\n" + callback);

            var info = callback.GetReturnValuetoJSON()["row"];
            if (info["nickname"] == null)
            {
                LoginUI.GetInstance().ActiveNickNameObject();
                return;
            }
            myNickName = info["nickname"].ToString();
            myIndate = info["inDate"].ToString();
            if (loginSuccessFunc != null)
            {
                MatchManager.GetInstance().GetMatchList(loginSuccessFunc);
                loginSuccessFunc(true, string.Empty);
            }
        });
            SceneManager.LoadScene("Matching");
    }
    void Update()
    {
        SendQueue.Poll();
        Backend.Match.Poll();
    }

    public void GuestLogin(Action<bool, string> func)
    {
        Enqueue(Backend.BMember.GuestLogin, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("게스트 로그인 성공");
                loginSuccessFunc = func;
                OnPrevBackendAuthorized();
                return;

            }
            Backend.BMember.DeleteGuestInfo();
            Debug.Log("게스트 로그인 실패\n" + callback);
            func(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }

    //이아래는 친구추가 관련 항목임 알잘딱갈센하샘
    //    public void GetFriendList(Action<bool, List<Friend>> func)
    //    {
    //        Enqueue(Backend.Social.Friend.GetFriendList, 15, callback =>
    //        {
    //            if (callback.IsSuccess() == false)
    //            {
    //                func(false, null);
    //                return;
    //            }

    //            var friendList = new List<Friend>();

    //            foreach (LitJson.JsonData tmp in callback.Rows())
    //            {
    //                if (tmp.Keys.Contains("nickname") == false)
    //                {
    //                    continue;
    //                }
    //                Friend friend = new Friend();
    //                friend.nickName = tmp["nickname"]["S"].ToString();
    //                friend.inDate = tmp["inDate"]["S"].ToString();

    //                friendList.Add(friend);
    //            }

    //            func(true, friendList);
    //        });
    //    }
    //    public void GetReceivedRequestFriendList(Action<bool, List<Friend>> func)
    //    {
    //        Enqueue(Backend.Social.Friend.GetReceivedRequestList, 15, callback =>
    //        {
    //            if (callback.IsSuccess() == false)
    //            {
    //                func(false, null);
    //                return;
    //            }

    //            var friendList = new List<Friend>();

    //            foreach (LitJson.JsonData tmp in callback.Rows())
    //            {
    //                if (tmp.Keys.Contains("nickname") == false)
    //                {
    //                    continue;
    //                }
    //                Friend friend = new Friend();
    //                friend.nickName = tmp["nickname"]["S"].ToString();
    //                friend.inDate = tmp["inDate"]["S"].ToString();

    //                friendList.Add(friend);
    //            }

    //            func(true, friendList);
    //        });
    //    }
    //    public void RequestFirend(string nickName, Action<bool, string> func)
    //    {
    //        Enqueue(Backend.Social.GetGamerIndateByNickname, nickName, callback =>
    //        {
    //            Debug.Log(callback);
    //            if (callback.IsSuccess() == false)
    //            {
    //                func(false, callback.GetMessage());
    //                return;
    //            }
    //            if (callback.Rows().Count <= 0)
    //            {
    //                func(false, "존재하지 않는 유저입니다.");
    //                return;
    //            }
    //            string inDate = callback.Rows()[0]["inDate"]["S"].ToString();
    //            Enqueue(Backend.Social.Friend.RequestFriend, inDate, callback2 =>
    //            {
    //                Debug.Log(callback2);
    //                if (callback2.IsSuccess() == false)
    //                {
    //                    func(false, callback2.GetMessage());
    //                    return;
    //                }

    //                func(true, string.Empty);
    //            });
    //        });
    //    }

    //    public void AcceptFriend(string inDate, Action<bool, string> func)
    //    {
    //        Enqueue(Backend.Social.Friend.AcceptFriend, inDate, callback2 =>
    //        {
    //            if (callback2.IsSuccess() == false)
    //            {
    //                func(false, callback2.GetMessage());
    //                return;
    //            }

    //            func(true, string.Empty);
    //        });
    //    }

    //    public void RejectFriend(string inDate, Action<bool, string> func)
    //    {
    //        Enqueue(Backend.Social.Friend.RejectFriend, inDate, callback2 =>
    //        {
    //            if (callback2.IsSuccess() == false)
    //            {
    //                func(false, callback2.GetMessage());
    //                return;
    //            }

    //            func(true, string.Empty);
    //        });
    //    }

}

