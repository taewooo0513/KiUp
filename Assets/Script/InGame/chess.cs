using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chess : MonoBehaviour
{
    [Header("체스판")]
    public GameObject WhiteBlock;
    public GameObject BlackBlock;

    [Header("흰색 말")]
    public GameObject WKing;
    public GameObject WQueen;
    public GameObject WPawn;
    public GameObject WKnight;
    public GameObject WRook;
    public GameObject WBishop;

    [Header("검은 말")]
    public GameObject BKing;
    public GameObject BPawn;
    public GameObject BQueen;
    public GameObject BRook;
    public GameObject BKnight;
    public GameObject BBishop;

    [Header("선택 창 UI")]
    public Button moveButton;
    public Button skillButton;

    [HideInInspector] static public bool openUI;

    private GameObject[,] ChessBoard = new GameObject[8, 8];
    private GameObject Move;

    void Start()
    {
        AddChessBoard();
        AddChessPiece();
    }

    void Update()
    {
        //마우스피킹
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Tile"))//마우스 피킹을하여 그 타일의 자식오브젝트를 확인하여 움직일수있음 
                {
                    if (openUI) ChoiceUI(hit);
                    else MoveChess(hit);
                }
            }
        }
    }

    void AddChessBoard()
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
    }
    void AddChessPiece()
    {
        //하얀적 생성
        Instantiate(WRook, new Vector3(0, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[0, 0].transform);
        Instantiate(WKnight, new Vector3(1, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[1, 0].transform);
        Instantiate(WBishop, new Vector3(2, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[2, 0].transform);
        Instantiate(WKing, new Vector3(3, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[3, 0].transform);
        Instantiate(WQueen, new Vector3(4, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[4, 0].transform);
        Instantiate(WBishop, new Vector3(5, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[5, 0].transform);
        Instantiate(WKnight, new Vector3(6, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[6, 0].transform);
        Instantiate(WRook, new Vector3(7, 0.5f, 0), Quaternion.identity).transform.SetParent(ChessBoard[7, 0].transform);
        //폰 생성
        for (int i = 0; i < 8; i++)
        {
            Instantiate(WPawn, new Vector3(i, 0.5f, 1), Quaternion.identity).transform.SetParent(ChessBoard[i, 1].transform);
            Instantiate(BPawn, new Vector3(i, 0.5f, 6), Quaternion.identity).transform.SetParent(ChessBoard[i, 6].transform);
        }
        //검은적생성
        Instantiate(BRook, new Vector3(0, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[0, 7].transform);
        Instantiate(BKnight, new Vector3(1, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[1, 7].transform);
        Instantiate(BBishop, new Vector3(2, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[2, 7].transform);
        Instantiate(BKing, new Vector3(3, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[3, 7].transform);
        Instantiate(BQueen, new Vector3(4, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[4, 7].transform);
        Instantiate(BBishop, new Vector3(5, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[5, 7].transform);
        Instantiate(BKnight, new Vector3(6, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[6, 7].transform);
        Instantiate(BRook, new Vector3(7, 0.5f, 7), Quaternion.identity).transform.SetParent(ChessBoard[7, 7].transform);
    }

    void ChoiceUI(RaycastHit hit)
    {
        if (hit.transform.childCount != 0)
        {
            Move = hit.transform.GetChild(0).gameObject;

            moveButton.transform.position = new Vector3(Move.transform.position.x + 100, Move.transform.position.y, 0);
            skillButton.transform.position = new Vector3(Move.transform.position.x + 100, Move.transform.position.y - 100, 0);

            moveButton.gameObject.SetActive(true);
            skillButton.gameObject.SetActive(true);
        }
    }

    void MoveChess(RaycastHit hit)
    {
        if (hit.transform.childCount == 0)
        {
            switch (Move.tag)
            {//Move 의 태그를 검사하여 그 태그를 가진 말들마다 제한둠
                case "Pawn":
                    PawnMove(Move, hit.transform.gameObject);
                    break;
                case "King":
                    KingMove(Move, hit.transform.gameObject);
                    break;
                case "Queen":
                    QueenMove(Move, hit.transform.gameObject);
                    break;
                case "Knight":
                    KnightMove(Move, hit.transform.gameObject);
                    break;
                case "Rook":
                    RookMove(Move, hit.transform.gameObject);
                    break;
                case "Bishop":
                    BishopMove(Move, hit.transform.gameObject);
                    break;
            }
        }
        else
        {
            if (hit.transform.GetChild(0).gameObject.layer != Move.layer)
            {//이건 부모오브젝트에 자식수가 1이상이면 실행됨 레이어를 검사하여 흰색 검은색 검사함
                switch (Move.tag)
                {
                    case "Pawn":
                        PawnATK(Move, hit.transform.gameObject);
                        break;
                    case "King":
                        KingATK(Move, hit.transform.gameObject);
                        break;
                    case "Queen":
                        QueenATK(Move, hit.transform.gameObject);
                        break;
                    case "Knight":
                        KnightATK(Move, hit.transform.gameObject);
                        break;
                    case "Rook":
                        RookATK(Move, hit.transform.gameObject);
                        break;
                    case "Bishop":
                        BishopATK(Move, hit.transform.gameObject);
                        break;
                }

            }
        }//한번 움직이면 null 로만들어줌 그래서 다음 클릭할때 또이동하지 못하게함
        Move = null;
    }

    void KingMove(GameObject Obj, GameObject _Obj)
    {
        if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) <= 1 || ((Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) >= 9) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) <= 11)))
        {
            Obj.transform.SetParent(_Obj.transform);
            Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
    }
    void KingATK(GameObject Obj, GameObject _Obj)
    {
        if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) <= 1 || ((Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) >= 9) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) <= 11)))
        {
            Move.transform.SetParent(_Obj.transform);
            Destroy(_Obj.transform.GetChild(0).gameObject);
            Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
    }

    void PawnATK(GameObject Obj, GameObject _Obj)
    {
        Debug.Log("gd");
    }
    void PawnMove(GameObject Obj, GameObject _Obj)
    {
        if (!Obj.GetComponent<PawnSkill>().PlusMovement)
        {
            if ((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0)
            {
                Debug.Log(Obj.layer);
                if (Obj.layer == LayerMask.NameToLayer("White"))
                {
                    if (int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) < 19 && int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) > 0)
                    {
                        Obj.transform.SetParent(_Obj.transform);
                        Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                    }
                }
                if (Obj.layer == LayerMask.NameToLayer("Black"))
                {
                    if (int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) > -19 && int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) < 0)
                    {
                        Obj.transform.SetParent(_Obj.transform);
                        Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                    }
                }
            }
        }
        else
        {
            if ((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0 || (int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 20 == 0)
            {
                Debug.Log(Obj.layer);
                if (Obj.layer == LayerMask.NameToLayer("White"))
                {
                    if (int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) < 29 && int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) > 0)
                    {
                        Obj.transform.SetParent(_Obj.transform);
                        Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                    }
                }
                if (Obj.layer == LayerMask.NameToLayer("Black"))
                {
                    if (int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) > -29 && int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name) < 0)
                    {
                        Obj.transform.SetParent(_Obj.transform);
                        Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                    }
                }
            }
        }
    }

    void QueenMove(GameObject Obj, GameObject _Obj)
    {
        for (int i = 1; i < 5; i++)
        {
            if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 11 || Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 9)
            {
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
            }
        }
        if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Obj.transform.SetParent(_Obj.transform);
            Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
        else if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Obj.transform.SetParent(_Obj.transform);
            Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
    }
    void QueenATK(GameObject Obj, GameObject _Obj)
    {
        for (int i = 1; i < 5; i++)
        {
            if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 11 || Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 9)
            {
                Move.transform.SetParent(_Obj.transform);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                Destroy(_Obj.transform.GetChild(0).gameObject);
            }
        }
        if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Move.transform.SetParent(_Obj.transform);
            Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
            Destroy(_Obj.transform.GetChild(0).gameObject);
        }
        else if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Move.transform.SetParent(_Obj.transform);
            Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
            Destroy(_Obj.transform.GetChild(0).gameObject);
        }
    }

    void BishopMove(GameObject Obj, GameObject _Obj)
    {
        for (int i = 1; i < 8; i++)
        {
            if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 11 || Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 9)
            {
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
            }
        }

    }
    void BishopATK(GameObject Obj, GameObject _Obj)
    {
        for (int i = 1; i < 5; i++)
        {
            if (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 11 || Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) == i * 9)
            {
                Move.transform.SetParent(_Obj.transform);
                Destroy(_Obj.transform.GetChild(0).gameObject);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
            }
        }

    }

    void KnightATK(GameObject Obj, GameObject _Obj)
    {
        switch (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)))
        {
            case 8:
                Move.transform.SetParent(_Obj.transform);
                Destroy(_Obj.transform.GetChild(0).gameObject);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 12:
                Move.transform.SetParent(_Obj.transform);
                Destroy(_Obj.transform.GetChild(0).gameObject);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 19:
                Move.transform.SetParent(_Obj.transform);
                Destroy(_Obj.transform.GetChild(0).gameObject);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 21:
                Move.transform.SetParent(_Obj.transform);
                Destroy(_Obj.transform.GetChild(0).gameObject);
                Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
        }
    }
    void KnightMove(GameObject Obj, GameObject _Obj)
    {
        switch (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)))
        {
            case 8:
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 12:
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 19:
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
            case 21:
                Obj.transform.SetParent(_Obj.transform);
                Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
                break;
        }
    }

    void RookATK(GameObject Obj, GameObject _Obj)
    {
        Debug.Log((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10);
        if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Move.transform.SetParent(_Obj.transform);
            Destroy(_Obj.transform.GetChild(0).gameObject);
            Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
        else if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Move.transform.SetParent(_Obj.transform);
            Destroy(_Obj.transform.GetChild(0).gameObject);
            Move.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
    }
    void RookMove(GameObject Obj, GameObject _Obj)
    {
        Debug.Log((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10);
        if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) % 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Obj.transform.SetParent(_Obj.transform);
            Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
        else if (((int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) / 10 == 0) && (Mathf.Abs(int.Parse(_Obj.name) - int.Parse(Obj.transform.parent.name)) != 9))
        {
            Obj.transform.SetParent(_Obj.transform);
            Obj.transform.position = new Vector3(_Obj.transform.position.x, 0.5f, _Obj.transform.position.z);
        }
    }
}
