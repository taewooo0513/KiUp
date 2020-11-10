using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chess : MonoBehaviour
{
    public GameObject WhiteBlock;
    public GameObject BlackBlock;
    public GameObject WKing;
    public GameObject BKing;
    private GameObject[,] ChessBoard = new GameObject[8, 8];

    // Start is called before the first frame update
    void Start()
    {
        int n = -1;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    ChessBoard[i, j] = Instantiate(BlackBlock, new Vector3(i, 0, j), Quaternion.identity);
                }
                else
                {
                    ChessBoard[i, j] = Instantiate(WhiteBlock, new Vector3(i, 0, j), Quaternion.identity);
                }
                n++;
                ChessBoard[i, j].name = n.ToString();
            }
        }

        Instantiate(WKing, new Vector3(4, 0.5f, 0), Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1")) // left mouse click
        {
            if (Physics.Raycast(ray, out hit))
            {
                //int findIndex = ChessBoard.FindIndex(ChessBoard,ChessBoard == hit);
                if(hit.transform.gameObject.CompareTag("Tile"))
                    Debug.Log(int.Parse(hit.transform.gameObject.name.ToString()));
            }
        }
        Debug.DrawRay(ray.origin, Vector3.forward, Color.red);
    }
}
