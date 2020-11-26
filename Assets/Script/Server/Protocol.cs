using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Protocol
{
    // 이벤트 타입
    public static class KeyEventCode
    {
        public const int NONE = 0;
        public const int MOVE = 1;      // 이동 메시지
        public const int ATTACK = 2;    // 공격 메시지
        public const int NO_MOVE = 4;   // 이동 멈춤 메시지
    }
    public enum Type : sbyte
    {
        Key = 0,        // 키(가상 조이스틱) 입력
        PlayerMove,     // 플레이어 이동

        LoadGameScene,      // 인게임 씬으로 전환
        StartCount,     // 시작 카운트
        GameStart,      // 게임 시작
        GameEnd,        // 게임 종료
        GameSync,       // 플레이어 재접속 시 게임 현재 상황 싱크
        Max
    }

    public class Message
    {
        public Type type;

        public Message(Type type)
        {
            this.type = type;
        }
    }
    public class KeyMessage : Message
    {
        public int keyData;
        Vector3 MousePos;
        public KeyMessage(int data, Vector3 Pos) : base(Type.Key)
        {
            this.keyData = data;
            MousePos.x = Pos.x;
            MousePos.y = Pos.y;
            MousePos.z = Pos.z;

        }
    }

    public class LoadGameSceneMessage : Message
    {
        public LoadGameSceneMessage() : base(Type.LoadGameScene)
        {

        }
    }
    public class LoadRoomSceneMessage : Message
    {
        public LoadRoomSceneMessage() : base(Type.LoadGameScene)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}