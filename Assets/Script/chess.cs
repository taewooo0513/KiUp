using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chess : MonoBehaviour
{
    public GameObject WhiteBlock;
    public GameObject BlackBlock;
    //흰색 말
    public GameObject WKing;
    public GameObject WQueen;
    public GameObject WPawn;
    public GameObject WKnight;
    public GameObject WRook;
    public GameObject WBishop;

    
    //검은색 말
    public GameObject BKing;
    public GameObject BPawn;
    public GameObject BQueen;
    public GameObject BRook;
    public GameObject BKnight;
    public GameObject BBishop;

    private GameObject[,] ChessBoard = new GameObject[8, 8];
    private GameObject Move;
    // Start is called before the first frame update
    void Start()
    {
        //체스판 생성
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    ChessBoard[j, i] = Instantiate(BlackBlock, new Vector3(j, 0, i), Quaternion.identity);
                }
                else
                {
                    ChessBoard[j, i] = Instantiate(WhiteBlock, new Vector3(j, 0, i), Quaternion.identity);
                }
                ChessBoard[j, i].name = i.ToString() + j.ToString();
            }
        }
        //하얀적 생성
        Instantiate(WRook, new Vector3(0, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[0, 0].transform);
        Instantiate(WKnight, new Vector3(1, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[1, 0].transform);
        Instantiate(WBishop, new Vector3(2, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[2, 0].transform);
        Instantiate(WKing, new Vector3(3, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[3, 0].transform);
        Instantiate(WQueen, new Vector3(4, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[4, 0].transform);
        Instantiate(WBishop, new Vector3(5, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[5, 0].transform);
        Instantiate(WKnight, new Vector3(6, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[6, 0].transform);
        Instantiate(WRook, new Vector3(7, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[7, 0].transform);


        Instantiate(BRook, new Vector3(0, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[0, 7].transform);
        Instantiate(BKnight, new Vector3(1, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[1, 7].transform);
        Instantiate(BBishop, new Vector3(2, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[2, 7].transform);
        Instantiate(BKing, new Vector3(3, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[3, 7].transform);
        Instantiate(BQueen, new Vector3(4, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[4, 7].transform);
        Instantiate(BBishop, new Vector3(5, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[5, 7].transform);
        Instantiate(BKnight, new Vector3(6, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[6, 7].transform);
        Instantiate(BRook, new Vector3(7, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[7, 7].transform);


    }
    // Update is called once per frame
    void Update()
    {
        //마우스피킹
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Tile"))
                {


                    if (Move != false)
                    {
                        if (hit.transform.childCount == 0)
                        {
                            Move.transform.SetParent(hit.transform);
                            Move.transform.position = new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z);

                        }
                        else
                        {
                            if (hit.transform.GetChild(0).tag != Move.tag)
                            {
                                Move.transform.SetParent(hit.transform);
                                Destroy(hit.transform.GetChild(0).gameObject);
                                Move.transform.position = new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z);

                            }
                        }
                        Move = null;
                    }
                    else
                    {
                        if (hit.transform.childCount != 0)
                        {
                            Move = hit.transform.GetChild(0).gameObject;
                        }
                    }
                }
            }
        }
    }
}
