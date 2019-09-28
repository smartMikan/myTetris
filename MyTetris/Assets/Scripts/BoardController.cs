using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    public GameObject gridUnitPrefab;
    public int gridSizeX, gridSizeY;
    

    public GameObject tetrisText;

    GridUnit[,] gameBoardUnits;
    


    private void Awake()
    {
        instance = this;
    }

    
    void Start()
    {
        CreateGrid();
        tetrisText.SetActive(false);
    }

    /// <summary>
    /// GameBoardを生成する
    /// </summary>
    private void CreateGrid()
    {
        gameBoardUnits = new GridUnit[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GridUnit newGridUnit = new GridUnit(gridUnitPrefab, transform, x, y);
                gameBoardUnits[x, y] = newGridUnit;
            }
        }
    }

    /// <summary>
    /// 目標座標はマップ内の座標ですかをチェックする
    /// </summary>
    /// <param name="coordToTest">チェックする座標</param>
    /// <returns>マップ外なら false 、マップ内なら true を返す</returns>
    public bool IsInBounds(Vector2Int coordToTest)
    {
        if (coordToTest.x < 0 || coordToTest.x >= gridSizeX || coordToTest.y < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 目標座標は空いているかをチェックする
    /// </summary>
    /// <param name="coordToTest"></param>
    /// <returns>空いている 或いは マップの上に浮かんでいる　→　true </returns>
    public bool IsPosEmpty(Vector2Int coordToTest)
    {
        if (coordToTest.y >= gridSizeY)
        {
            return true;
        }
        if (gameBoardUnits[coordToTest.x, coordToTest.y].isOccupied)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// テトリスピースをセットする時に呼ぶ
    /// </summary>
    /// <param name="coords">セットの位置座標</param>
    /// <param name="DesTile">テトリスピースの子供ブロック(Tile)</param>
    public void OccupyPos(Vector2Int coords, GameObject DesTile)
    {
        gameBoardUnits[coords.x, coords.y].isOccupied = true;
        gameBoardUnits[coords.x, coords.y].tileOnThisGrid = DesTile;
    }

    /// <summary>
    /// 下から上にいる行をチェックして、もし行が全部テトリスピースに入ったら、
    /// ClearLineを呼び出して、この行をクリアする
    /// </summary>
    public void CheckLineClears()
    {
        //クリアするべき行のリストインデックス
        List<int> linesToClear = new List<int>();

        //連続クリアされた行を計算して、得点を加算する
        //4行の場合、テトリスクリアです
        int consecutiveLineClears = 0;

        for(int y = 0; y < gridSizeY; y++)
        {
            bool lineClear = true;
            for(int x = 0; x < gridSizeX; x++)
            {
                if (!gameBoardUnits[x, y].isOccupied)
                {
                    lineClear = false;
                    consecutiveLineClears = 0;
                }
            }
            if (lineClear)
            {
                linesToClear.Add(y);
                consecutiveLineClears++;

                //テトリスクリアの処理
                if (consecutiveLineClears == 4)
                {
                    ShowTetrisText();
                    Debug.Log("<color=red>T</color>" +
                              "<color=orange>E</color>" +
                              "<color=yellow>T</color>" +
                              "<color=lime>R</color>" +
                              "<color=aqua>I</color>" +
                              "<color=purple>S</color>" +
                              "<color=blue>!</color>");
                }
                GameController.instance.Score += consecutiveLineClears * consecutiveLineClears * 10;
                ScoreView.instance.UpdateScore();
                AudioController.PlaySnd("SEB_mino4", Camera.main.transform.position, consecutiveLineClears * 0.25f);
                ClearLine(y);
            }
        }

        //行をクリアした場合、上の行を下にドロップさせる
        if (linesToClear.Count > 0)
        {
            for (int i = 0; i < linesToClear.Count; i++)
            {
                /*lineToDrop　=　クリアしたLinesToClear[i]行目（y行）
                 * 　　　　　　　+ 1 行で　
                 * 　　　　　　　- 今消した行数 i　から、
                 * 　　　　　　　上にいるすべての行
                 * 何故　-i　ですか？　　　　　　　
                 * 例えば、3行目と5行目を消したい、
                 * linesToClear.Count = 2,
                 * linesToClear[0] = 2,
                 * linesToClear[1] = 4,
                 * これで、まず i = 0 で
                 * 　　　　　　　2+1-0 = 3で、つまり4行目から、上のすべての行を下に1行落とした、
                 * そして元の6行目は5行になったから、次は5行目から上の行を落とすべきだろう、
                 * なら、二回目で、i = 1で
                 *                  4+1-1 = 4で、確か5行目からですね！
                 * -i　によって、さっき落とした行数を引き算できた。
                 * 数学の魅力を感じた(笑) by オウカンウ
                 */
                for (int lineToDrop = linesToClear[i] + 1 - i; lineToDrop < gridSizeY; lineToDrop++)
                {
                    for (int x = 0; x < gridSizeX; x++)
                    {
                        GridUnit curGridUnit = gameBoardUnits[x, lineToDrop];
                        if (curGridUnit.isOccupied)
                        {
                            MoveTileDown(curGridUnit);
                        }
                    }
                }
            }
        }

        linesToClear.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowTetrisText()
    {
        tetrisText.SetActive(true);
        Invoke("HideTetrisText", 4f);
    }

    /// <summary>
    /// 
    /// </summary>
    private void HideTetrisText()
    {
        tetrisText.SetActive(false);
    }

    /// <summary>
    /// タイルを一行下に動く
    /// </summary>
    /// <param name="curGridUnit"></param>
    private void MoveTileDown(GridUnit curGridUnit)
    {
        TileController curTile = curGridUnit.tileOnThisGrid.GetComponent<TileController>();
        curTile.MoveTile(Vector2Int.down);
        curTile.SetTile();
        curGridUnit.tileOnThisGrid = null;
        curGridUnit.isOccupied = false;
    }

    /// <summary>
    /// 一行のタイルをクリアする
    /// </summary>
    /// <param name="lineToClear">行のインデクス</param>
    private void ClearLine(int lineToClear)
    {
        if (lineToClear < 0 || lineToClear > gridSizeY)
        {
            Debug.LogError("Error: Cannot Clear Line" + lineToClear);
            return;
        }
        for (int x = 0; x < gridSizeX; x++)
        {
            PieceController curPC = gameBoardUnits[x, lineToClear].tileOnThisGrid.GetComponent<TileController>().pieceController;
            curPC.tiles[gameBoardUnits[x, lineToClear].tileOnThisGrid.GetComponent<TileController>().tileIndex] = null;
            Destroy(gameBoardUnits[x, lineToClear].tileOnThisGrid);
            if (!curPC.AnyTilesLeft())
            {
                Destroy(curPC.gameObject);
            }
            gameBoardUnits[x, lineToClear].tileOnThisGrid = null;
            gameBoardUnits[x, lineToClear].isOccupied = false;
        }
        
    }

    /// <summary>
    /// タイルがクリアされた時、タイルとマップの参照を解除する
    /// そしてクリアされたタイルの上のタイルを落とす
    /// </summary>
    /// <param name="pieceCoords">クリアされたタイルの座標配列</param>
    public void PieceRemoved(Vector2Int[] pieceCoords)
    {
        foreach (Vector2Int coords in pieceCoords)
        {
            GridUnit curGridUnit = gameBoardUnits[coords.x, coords.y];
            curGridUnit.tileOnThisGrid = null;
            curGridUnit.isOccupied = false;
        }

        for (int i = 0; i < pieceCoords.Length; i++)
        {
            for (int y = pieceCoords[i].y; y < gridSizeY; y++)
            {
                GridUnit curGridUnit = gameBoardUnits[pieceCoords[i].x, y];
                if (curGridUnit.isOccupied)
                {
                    MoveTileDown(curGridUnit);
                }
            }
        }
        CheckLineClears();
    }
}
