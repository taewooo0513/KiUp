using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.Dispatcher;
using UnityEngine.SceneManagement;
using BackEnd;
using TheBackend;
public class LoginUI : MonoBehaviour
{
    private static LoginUI instance;    // 인스턴스

    public GameObject mainTitle;
    public GameObject subTitle;
    public GameObject touchStart;
    public GameObject loginObject;
    public GameObject customLoginObject;
    public GameObject signUpObject;
    public GameObject errorObject;
    public GameObject nicknameObject;

    private InputField[] loginField;
    private InputField[] signUpField;
    private InputField nicknameField;
    private Text errorText;
    private GameObject loadingObject;
   // private FadeAnimation fadeObject;

    private const byte ID_INDEX = 0;
    private const byte PW_INDEX = 1;
    private const string VERSION_STR = "Ver {0}";

    void Awake()
    {
        instance = this;
    }

    public static LoginUI GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("LoginUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    void Start()
    {
       // mainTitle.SetActive(true);
    //    touchStart.SetActive(true);
       // subTitle.SetActive(false);
        loginObject.SetActive(true);
        customLoginObject.SetActive(false);
        signUpObject.SetActive(false);
        errorObject.SetActive(false);
        nicknameObject.SetActive(false);

        loginField = customLoginObject.GetComponentsInChildren<InputField>();
        signUpField = signUpObject.GetComponentsInChildren<InputField>();
        nicknameField = nicknameObject.GetComponentInChildren<InputField>();
        errorText = errorObject.GetComponentInChildren<Text>();

        


        //  var fade = GameObject.FindGameObjectWithTag("Fade");
        //   if (fade != null)
        //{
        // fadeObject = fade.GetComponent<FadeAnimation>();
        // }

        //  mainTitle.GetComponentInChildren<Text>().text = string.Format(VERSION_STR, Application.version);


    }

    public void TouchStart()
    {
     
        serverManager.GetInstance().BackendTokenLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                Debug.Log("gdasn");
                if (result)
                {
                 //   ChangeLobbyScene();

                    return;
                }
                //loadingObject.SetActive(false);
                if (!error.Equals(string.Empty))
                {
                    errorText.text = "유저 정보 불러오기 실패\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                mainTitle.SetActive(false);
                touchStart.SetActive(false);
                subTitle.SetActive(true);
                customLoginObject.SetActive(true);
                loginObject.SetActive(true);
            });
        });

    }

    public void Login()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string id = loginField[ID_INDEX].text;
        string pw = loginField[PW_INDEX].text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            errorText.text = "ID 혹은 PW 를 먼저 입력해주세요.";
            errorObject.SetActive(true);
            return;
        }

        serverManager.GetInstance().CustomLogin(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                Debug.Log("gdasn");
                if (!result)
                {
                    //loadingObject.SetActive(false);
                    errorText.text = "로그인 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }

            });
        });
    }

    public void SignUp()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string id = signUpField[ID_INDEX].text;
        string pw = signUpField[PW_INDEX].text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            errorText.text = "ID 혹은 PW 를 먼저 입력해주세요.";
            errorObject.SetActive(true);
            return;
        }

      // loadingObject.SetActive(true);
        serverManager.GetInstance().CustomSignIn(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
             
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "회원가입 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                
               
            });
        });
    }

    public void ActiveNickNameObject()
    {
        
        Dispatcher.Current.BeginInvoke(() =>
        {
            loginObject.SetActive(false);
            customLoginObject.SetActive(false);
            signUpObject.SetActive(false);
            errorObject.SetActive(false);
            nicknameObject.SetActive(true);
        });
    }
    public void NextScene()
    {
        SceneManager.LoadScene("Matching");
    }
    public void UpdateNickName()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string nickname = nicknameField.text;
        if (nickname.Equals(string.Empty))
        {
            errorText.text = "닉네임을 먼저 입력해주세요";
            errorObject.SetActive(true);
            return;
        }
     //  loadingObject.SetActive(true);
        serverManager.GetInstance().UpdateNickname(nickname, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "닉네임 생성 오류\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
               // ChangeLobbyScene();
            });
        });
    }



    

    public void GuestLogin()
    {
        if (errorObject.activeSelf)
        {
            return;
        }

        //   loadingObject.SetActive(true);
        serverManager.GetInstance().GuestLogin((bool result, string error) =>
        {

            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "로그인 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }

            });
        });

    }

    //void ChangeLobbyScene()
    //{
    //    if (fadeObject != null)
    //    {
    //        GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby, (bool isDone) =>
    //        {
    //            Dispatcher.Current.BeginInvoke(() => loadingObject.transform.Rotate(0, 0, -10));
    //            if (isDone)
    //            {
    //                fadeObject.ProcessFadeOut();
    //            }
    //        });
    //    }
    //    else
    //    {
    //    //    GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
    //    }
    //}
}
