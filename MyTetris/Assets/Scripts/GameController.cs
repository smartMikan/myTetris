using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ゲームを管理し、テトリスの回転DATAを持つ
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Button StartButton;
    public Button ResetButton;


    public GameObject piecePrefab;
    public GameObject nextPieceView;

    public Vector2Int spawnPos;
    public float droptime;

    public Coroutine dropCurPiece;
    public Vector2Int[,] JLSTZ_OFFSET_DATA { get; private set; }
    public Vector2Int[,] I_OFFSET_DATA { get; private set; }
    public Vector2Int[,] O_OFFSET_DATA { get; private set; }
    public List<GameObject> piecesInGame;
    public GameObject pieceToDestroy = null;
    public GameObject gameOverText;

    GameObject curPiece = null;
    GameObject nextPiece = null;
    PieceController curPieceController = null;

    int turnCounter;

    public int Score = 0;

    public bool Gameover { get; private set; }

    private void Awake()
    {
        instance = this;

        //DATA From https://harddrop.com/wiki/SRS
        JLSTZ_OFFSET_DATA = new Vector2Int[5, 4];
        JLSTZ_OFFSET_DATA[0, 0] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[0, 1] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[0, 2] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[0, 3] = Vector2Int.zero;

        JLSTZ_OFFSET_DATA[1, 0] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[1, 1] = new Vector2Int(1, 0);
        JLSTZ_OFFSET_DATA[1, 2] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[1, 3] = new Vector2Int(-1, 0);

        JLSTZ_OFFSET_DATA[2, 0] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[2, 1] = new Vector2Int(1, -1);
        JLSTZ_OFFSET_DATA[2, 2] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[2, 3] = new Vector2Int(-1, -1);

        JLSTZ_OFFSET_DATA[3, 0] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[3, 1] = new Vector2Int(0, 2);
        JLSTZ_OFFSET_DATA[3, 2] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[3, 3] = new Vector2Int(0, 2);

        JLSTZ_OFFSET_DATA[4, 0] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[4, 1] = new Vector2Int(1, 2);
        JLSTZ_OFFSET_DATA[4, 2] = Vector2Int.zero;
        JLSTZ_OFFSET_DATA[4, 3] = new Vector2Int(-1, 2);

        I_OFFSET_DATA = new Vector2Int[5, 4];
        I_OFFSET_DATA[0, 0] = Vector2Int.zero;
        I_OFFSET_DATA[0, 1] = new Vector2Int(-1, 0);
        I_OFFSET_DATA[0, 2] = new Vector2Int(-1, 1);
        I_OFFSET_DATA[0, 3] = new Vector2Int(0, 1);

        I_OFFSET_DATA[1, 0] = new Vector2Int(-1, 0);
        I_OFFSET_DATA[1, 1] = Vector2Int.zero;
        I_OFFSET_DATA[1, 2] = new Vector2Int(1, 1);
        I_OFFSET_DATA[1, 3] = new Vector2Int(0, 1);

        I_OFFSET_DATA[2, 0] = new Vector2Int(2, 0);
        I_OFFSET_DATA[2, 1] = Vector2Int.zero;
        I_OFFSET_DATA[2, 2] = new Vector2Int(-2, 1);
        I_OFFSET_DATA[2, 3] = new Vector2Int(0, 1);

        I_OFFSET_DATA[3, 0] = new Vector2Int(-1, 0);
        I_OFFSET_DATA[3, 1] = new Vector2Int(0, 1);
        I_OFFSET_DATA[3, 2] = new Vector2Int(1, 0);
        I_OFFSET_DATA[3, 3] = new Vector2Int(0, -1);

        I_OFFSET_DATA[4, 0] = new Vector2Int(2, 0);
        I_OFFSET_DATA[4, 1] = new Vector2Int(0, -2);
        I_OFFSET_DATA[4, 2] = new Vector2Int(-2, 0);
        I_OFFSET_DATA[4, 3] = new Vector2Int(0, 2);

        O_OFFSET_DATA = new Vector2Int[1, 4];
        O_OFFSET_DATA[0, 0] = Vector2Int.zero;
        O_OFFSET_DATA[0, 1] = Vector2Int.down;
        O_OFFSET_DATA[0, 2] = new Vector2Int(-1, -1);
        O_OFFSET_DATA[0, 3] = Vector2Int.left;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        piecesInGame = new List<GameObject>();
        gameOverText.SetActive(false);
        Gameover = true;
        ResetButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Gameover)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                curPieceController.SendPieceToFloor();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveCurPiece(Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveCurPiece(Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveCurPiece(Vector2Int.right);
            }
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                curPieceController.RotatePiece(true, true);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                curPieceController.RotatePiece(false, true);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GameStart();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        
    }

    IEnumerator DropCurPiece()
    {
        while (true)
        {
            MoveCurPiece(Vector2Int.down);
            yield return new WaitForSeconds(droptime);
        }
    }

    public void PieceSet()
    {
        StopCoroutine(dropCurPiece);
    }

    public void GameOver()
    {
        PieceSet();
        Camera.main.GetComponent<AudioSource>().Stop();
        gameOverText.SetActive(true);
        Gameover = true;

        ResetButton.gameObject.SetActive(true);
    }

    public void RemovePiece(GameObject pieceToRemove)
    {
        piecesInGame.Remove(pieceToRemove);
    }

    void DestroyPiece()
    {
        PieceController curPC = pieceToDestroy.GetComponent<PieceController>();
        Vector2Int[] tileCoords = curPC.GetTileCoords();
        RemovePiece(pieceToDestroy);
        Destroy(pieceToDestroy);
        BoardController.instance.PieceRemoved(tileCoords);
    }

    public void SpawnPiece()
    {
        turnCounter++;

        
        GameObject Piece = Instantiate(piecePrefab, transform);
        curPiece = Piece;
        curPieceController = curPiece.GetComponent<PieceController>();
        curPieceController.SpawnPiece(nextPiece.GetComponent<PieceController>().curType);

        piecesInGame.Add(Piece);

        dropCurPiece = StartCoroutine(DropCurPiece());

        if (nextPiece != null)
        {
            Destroy(nextPiece);
        }
        PieceType randPiece = (PieceType)Random.Range(0, 7);
        GameObject npiece = Instantiate(piecePrefab, nextPieceView.transform);
        npiece.GetComponent<PieceController>().curType = randPiece;
        npiece.GetComponent<PieceController>().SpawnPiece(randPiece, new Vector2Int((int)nextPieceView.transform.position.x,
                                                                                    (int)nextPieceView.transform.position.y));
        nextPiece = npiece;

    }

    public void MoveCurPiece(Vector2Int moveDir)
    {
        if(curPiece == null)
        {
            return;
        }
        curPieceController.MovePiece(moveDir);
    }

    public void GameStart()
    {
        if (curPieceController != null)
        {
            return;
        }

        StartButton.gameObject.SetActive(false);
        

        Gameover = false;
        turnCounter = 0;
        Score = 0;
        Camera.main.GetComponent<AudioSource>().Play();
        ScoreView.instance.UpdateScore();
        GameObject npiece = Instantiate(piecePrefab, nextPieceView.transform);
        PieceType randPiece = (PieceType)Random.Range(0, 6);
        npiece.GetComponent<PieceController>().curType = randPiece;
        npiece.GetComponent<PieceController>().SpawnPiece(randPiece, new Vector2Int((int)nextPieceView.transform.position.x,
                                                                                   (int)nextPieceView.transform.position.y));
        nextPiece = npiece;
        SpawnPiece();
    }
    public void MoveLeft()
    {
        if (!Gameover) {
            MoveCurPiece(Vector2Int.left);
        }
    }
    public void MoveRight()
    {
        if (!Gameover)
        {
            MoveCurPiece(Vector2Int.right);
        }
    }
    public void MoveDown()
    {
        if (!Gameover)
        {
            MoveCurPiece(Vector2Int.down);
        }
    }

    public void RotateClockwise()
    {
        if (!Gameover)
        {
            curPieceController.RotatePiece(true, true);
        }
    }
    public void RotateConterClockwise()
    {
        if (!Gameover)
        {
            curPieceController.RotatePiece(false, true);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
