using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class InputManager : MonoBehaviour
{
    //public virtualSkick
    private bool isMove = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    void MobileInput()
    {
        int keyCode = 0;
        KeyMessage msg;
        keyCode |= KeyEventCode.MOVE;
        Vector3 moveVector = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        msg = new KeyMessage(keyCode, moveVector);
        if (MatchManager.GetInstance().IsHost())
        {
            MatchManager.GetInstance().AddMsgToLocalQueue(msg);
        }
        else
        {
            MatchManager.GetInstance().SendDataToInGame<KeyMessage>(msg);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
