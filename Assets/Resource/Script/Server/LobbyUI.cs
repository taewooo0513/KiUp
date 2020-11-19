using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.Tcp;
using Battlehub.Dispatcher;

public class LobbyUI : MonoBehaviour
{
    public GameObject nickNameObject;

    public static LobbyUI GetInstance()
    {
        if(instance ==  null)
        {
            Debug.LogError("Lobby인스턴스 없음");
            return null;
        }
        return instance;
    }
    private static LobbyUI instance;
    // Start is called before the first frame update
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(instance );
        }
        instance = this;

        MatchManager.GetInstance().IsMatchGameActivate();
    }
    void Start()
    {
        if(MatchManager.GetInstance() != null)
        {
            SetNickName();
        }
    }
    private void SetNickName()
    {
        var name = serverManager.GetInstance().myNickName;
        if(name.Equals(string.Empty))
        {
            Debug.LogError("닉네임 불러오기 실패");
            name = "test123";
        }
        Text nickName = nickNameObject.GetComponent<Text>();
        RectTransform rect = nickName.GetComponent<RectTransform>();
        nickName.text = name;
        rect.sizeDelta = new Vector2(nickName.preferredWidth, nickName.preferredHeight);
    }
    public void RequestCancel()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

    }
}
