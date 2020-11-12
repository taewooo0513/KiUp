using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public Scene Scene;
    private string ID,PassWord;
    public InputField InputID,InputPW;
    public Button Nextbutton;
    
    public void OnClick()
    {
        ID = InputID.text;
        PassWord = InputPW.text;
        _Login();
    }
 
    // Start is called before the first frame update
    void _Login()
    {
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행
            if (Backend.IsInitialized)
            {
                Backend.BMember.CustomLogin(ID, PassWord);
                SceneManager.LoadScene("GameScene");

            }
            // 초기화 실패한 경우 실행
            else
            {
                _Login();
            }
        });
    }
    void Register()
    {
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행
            if (Backend.IsInitialized)
            {
                Backend.BMember.CustomSignUp(ID, PassWord);
                SceneManager.LoadScene("GameScene");
            }
            // 초기화 실패한 경우 실행
            else
            {
                Register();
            }
        });
    }
    void Start()
    {
    }

    // Update is called once per frame

    void Update()
    {
    }
}