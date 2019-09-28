using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//テトリスピースの種類
public enum PieceType { O, I, S, Z, L, J, T }

/// <summary>
/// テトリスピース単位にコントロールするクラス
/// </summary>
public class PieceController : MonoBehaviour
{

    public PieceType curType;
    public Sprite[] tileSprites;

    public int RotationIndex { get; private set; }
    //public int setDelayTime;
    //public bool isDisableFromSacrifice;

    public TileController[] tiles;
    Vector2Int spawnLocation;

    /// <summary>
    /// Initializeの後すぐ呼び出す
    /// </summary>
    private void Awake()
    {
        spawnLocation = GameController.instance.spawnPos;
        RotationIndex = 0;

        tiles = new TileController[4];
        //名前によって子供タイルを取得
        for (int i = 1; i <= tiles.Length; i++)
        {
            string tileName = "Tile" + i;
            TileController newTile = transform.Find("Tiles").Find(tileName).GetComponent<TileController>();
            tiles[i - 1] = newTile;
        }
    }

    /// <summary>
    /// テトリスタイルをタイプによって位置を設定する、その後、色タイルspriteを指定する
    /// </summary>
    /// <param name="newType">テトリスピースのタイプ</param>
    public void SpawnPiece(PieceType newType , Vector2Int spawnLoc)
    {
        curType = newType;
        tiles[0].UpdatePosition(spawnLoc);

        switch (curType)
        {
            case PieceType.O:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.right);
                tiles[2].UpdatePosition(spawnLoc + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.up);
                SetTileSprites(tileSprites[0]);
                break;
            case PieceType.I:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLoc + (Vector2Int.right * 2));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.right);
                SetTileSprites(tileSprites[1]);
                break;
            case PieceType.S:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLoc + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.up);
                SetTileSprites(tileSprites[2]);
                break;
            case PieceType.Z:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.up);
                tiles[2].UpdatePosition(spawnLoc + new Vector2Int(-1, 1));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.right);
                SetTileSprites(tileSprites[3]);
                break;
            case PieceType.L:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLoc + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.right);
                SetTileSprites(tileSprites[4]);
                break;
            case PieceType.J:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLoc + new Vector2Int(-1, 1));
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.right);
                SetTileSprites(tileSprites[5]);
                break;
            case PieceType.T:
                tiles[1].UpdatePosition(spawnLoc + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLoc + Vector2Int.up);
                tiles[3].UpdatePosition(spawnLoc + Vector2Int.right);
                SetTileSprites(tileSprites[6]);
                break;
            default:
                break;
        }

        int index = 0;
        foreach (TileController tc in tiles)
        {
            tc.InitializzeTile(this, index);
            index++;
        }
    }

    public void SpawnPiece(PieceType newType)
    {
        curType = newType;
        tiles[0].UpdatePosition(spawnLocation);

        switch (curType)
        {
            case PieceType.O:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.right);
                tiles[2].UpdatePosition(spawnLocation + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.up);
                SetTileSprites(tileSprites[0]);
                break;
            case PieceType.I:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLocation + (Vector2Int.right * 2));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.right);
                SetTileSprites(tileSprites[1]);
                break;
            case PieceType.S:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLocation + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.up);
                SetTileSprites(tileSprites[2]);
                break;
            case PieceType.Z:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.up);
                tiles[2].UpdatePosition(spawnLocation + new Vector2Int(-1, 1));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.right);
                SetTileSprites(tileSprites[3]);
                break;
            case PieceType.L:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLocation + new Vector2Int(1, 1));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.right);
                SetTileSprites(tileSprites[4]);
                break;
            case PieceType.J:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLocation + new Vector2Int(-1, 1));
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.right);
                SetTileSprites(tileSprites[5]);
                break;
            case PieceType.T:
                tiles[1].UpdatePosition(spawnLocation + Vector2Int.left);
                tiles[2].UpdatePosition(spawnLocation + Vector2Int.up);
                tiles[3].UpdatePosition(spawnLocation + Vector2Int.right);
                SetTileSprites(tileSprites[6]);
                break;
            default:
                break;
        }

        int index = 0;
        foreach (TileController tc in tiles)
        {
            tc.InitializzeTile(this, index);
            index++;
        }
    }
    /// <summary>
    /// テトリスピースの色を指定する
    /// </summary>
    /// <param name="newSprite">色タイルsprite</param>
    public void SetTileSprites(Sprite newSprite)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                continue;
            }
            tiles[i].gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }

    /// <summary>
    /// このテトリスピースの子供タイルの座標を取得
    /// </summary>
    /// <returns>このテトリスピースの内のすべてのタイルの座標のArrayを返す</returns>
    public Vector2Int[] GetTileCoords()
    {
        List<Vector2Int> curTileCoords = new List<Vector2Int>();

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                continue;
            }
            curTileCoords.Add(tiles[i].coordinates);
        }
        curTileCoords.OrderBy(t => t.x).ThenByDescending(t => t.y).ToList();
        foreach (Vector2Int v2i in curTileCoords)
        {
            Debug.Log("CurTile is " + v2i.ToString());
        }
        Vector2Int[] curCoords = curTileCoords.ToArray();
        return curCoords;
    }

    /// <summary>
    /// ピースを移動できるかをチェックする
    /// マップ外、及び他のタイルがあるところは移動できません
    /// </summary>
    /// <param name="moveDir">X,Y移動方向</param>
    /// <returns></returns>
    public bool CanMovePiece(Vector2Int moveDir)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if(!tiles[i].CanTileMove(moveDir + tiles[i].coordinates))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// このピースはまだタイルが残っているかをチェックする、残っていない場合、このgameObjectを消す
    /// </summary>
    /// <returns></returns>
    public bool AnyTilesLeft()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                return true;
            }
        }
        Debug.Log("No Tiles Left");
        GameController.instance.RemovePiece(gameObject);
        return false;
    }

    /// <summary>
    /// ピースを移動する
    /// </summary>
    /// <param name="moveDir">X,Y移動方向</param>
    /// <returns>移動できる → true 　移動できぬ → false</returns>
    public bool MovePiece(Vector2Int moveDir)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if(!tiles[i].CanTileMove(moveDir + tiles[i].coordinates))
            {
                Debug.Log("Can't Move To There!");
                if(moveDir == Vector2Int.down)
                {
                    SetPiece();
                }
                return false;
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].MoveTile(moveDir);
        }

        return true;
    }

    /// <summary>
    /// ピースを90°回転する
    /// 
    /// </summary>
    /// <param name="clockwise">true → 時計回り  false → 反時計回り</param>
    /// <param name="shouldCheckOffset">Offset回転操作を実行するか、常時にtrueで、
    /// 　　　　　　　　　　　　　　　　もし回転できませんなら元の状態に戻す用</param>
    public void RotatePiece(bool clockwise, bool shouldCheckOffset)
    {
        int oldRotationIndex = RotationIndex;
        RotationIndex += clockwise ? 1 : -1;
        RotationIndex = Mod(RotationIndex, 4);

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].RotateTile(tiles[0].coordinates, clockwise);
        }

        if (!shouldCheckOffset)
        {
            return;
        }

        bool canOffset = CheckOffset(oldRotationIndex, RotationIndex);

        if (!canOffset)
        {
            RotatePiece(!clockwise, false);
        }
    }

    /// <summary>
    /// モジュロ演算によって配列内の要素を循環参照する
    /// </summary>
    /// <param name="x">今の要素number</param>
    /// <param name="m">全体の要素数</param>
    /// <returns></returns>
    int Mod(int x,int m)
    {
        return (x % m + m) % m;
    }

    /// <summary>
    /// 各テトリスピースのOFFSETDATAの両面チェック、TSpinチェックを実行し、
    /// 最初の回転できる位置に回転する
    /// </summary>
    /// <param name="oldRotationIndex">今の回転状態</param>
    /// <param name="newRotationINdex">次に回転状態</param>
    /// <returns></returns>
    bool CheckOffset(int oldRotationIndex,int newRotationINdex)
    {
        Vector2Int offsetValue1, offsetValue2, endOffset;
        Vector2Int[,] curOffsetData;
        if(curType == PieceType.O)
        {
            curOffsetData = GameController.instance.O_OFFSET_DATA;
        }
        else if (curType == PieceType.I)
        {
            curOffsetData = GameController.instance.I_OFFSET_DATA;
        }
        else
        {
            curOffsetData = GameController.instance.JLSTZ_OFFSET_DATA;
        }

        endOffset = Vector2Int.zero;

        bool movePossible = false;

        for (int testIndex = 0; testIndex < 5; testIndex++)
        {
            offsetValue1 = curOffsetData[testIndex, oldRotationIndex];
            offsetValue2 = curOffsetData[testIndex, newRotationINdex];
            endOffset = offsetValue1 - offsetValue2;
            if (CanMovePiece(endOffset))
            {
                movePossible = true;
                break;
            }
        }
        if (movePossible)
        {
            MovePiece(endOffset);
        }
        else
        {
            Debug.Log("Move impossible");
        }
        return movePossible;
    }


    /// <summary>
    /// ピースをマップ内にセットする
    /// </summary>
    public void SetPiece()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].SetTile())
            {
                Debug.Log("GAME OVER!");
                GameController.instance.GameOver();
                return;
            }
        }
        BoardController.instance.CheckLineClears();
        GameController.instance.PieceSet();
        if (!GameController.instance.Gameover)
        {
            GameController.instance.SpawnPiece();
        }
    }


    /// <summary>
    /// ピースを一番下まで落とす
    /// </summary>
    public void SendPieceToFloor()
    {
        while (MovePiece(Vector2Int.down)) { }
    }
}
