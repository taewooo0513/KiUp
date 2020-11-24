using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.Tcp;
using Battlehub.Dispatcher;



public class MatchUI : MonoBehaviour
{
    public GameObject nickNameObject;
    private static MatchUI instance;    // 인스턴스

    private void Start()
    {
        if (serverManager.GetInstance() != null)
        {
            SetNickName();
        }
        instance = this;
    }
    public static MatchUI GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("LoginUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }
    // Start is called before the first frame update 
    private void SetNickName()
    {
        var name = serverManager.GetInstance().myNickName;
        if (name.Equals(string.Empty))
        {
            Debug.LogError("닉네임 불러오기 실패");
            name = "test123";
        }
        Text nickname = nickNameObject.GetComponent<Text>();
        RectTransform rect = nickNameObject.GetComponent<RectTransform>();

        nickname.text = name;
        rect.sizeDelta = new Vector2(nickname.preferredWidth, nickname.preferredHeight);
    }
    public void StartMatch()
    {
        MatchManager.GetInstance().JoinMatchServer();
    }
    public void StopMatch()
    {
        MatchManager.GetInstance().LeaveMatchServer();
    }
}
