using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchUI : MonoBehaviour
{
    private static MatchUI instance;    // 인스턴스

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
    public void StartMatch()
    {
        MatchManager.GetInstance().JoinMatchServer();
    }
    public void StopMatch()
    {
        MatchManager.GetInstance().LeaveMatchServer();
    }
}
