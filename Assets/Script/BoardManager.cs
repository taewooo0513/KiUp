using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private const float tileSize = 1;
    private const float tileOffset = 0.5f;

    private int selX = -1;
    private int selY = -1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DrawChessBoard();
        SelectUpdate();
    }

    private void SelectUpdate()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selX = (int)hit.point.x;
            selY = (int)hit.point.z;
        }
        else
        {
            selX = -1;
            selY = -1;
        }
    }

    private void DrawChessBoard()
    {
        Vector3 width = Vector3.right * 8;
        Vector3 height = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++) //체스판 칸 나누기
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + width);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + height);
            }
        }

        if (selX >= 0 && selY >= 0) //마우스 위치 표시
        {
            Debug.DrawLine(
                Vector3.forward * selY + Vector3.right * selX,
                Vector3.forward * (selY + 1) + Vector3.right * (selX + 1)
                );

            Debug.DrawLine(
                Vector3.forward * (selY + 1) + Vector3.right * selX,
                Vector3.forward * selY + Vector3.right * (selX + 1)
                );
        }
    }
}
