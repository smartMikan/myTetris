using UnityEngine;
/// <summary>
/// テトリスピースのタイルを制御するクラス
/// </summary>
public class TileController : MonoBehaviour
{
    public Vector2Int coordinates;　　//このタイルのマップ座標

    public PieceController pieceController;
    public int tileIndex;


    /// <summary>
    /// ジェネレーター
    /// </summary>
    /// <param name="myPC">このタイルの親テトリスピースのPieceControllerの参照</param>
    /// <param name="index">このタイルは親テトリスピースの何番目のタイルのインデックス</param>
    public void InitializzeTile(PieceController myPC,int index)
    {
        pieceController = myPC;
        tileIndex = index;
    }

    /// <summary>
    /// タイルが目標に移動できるかをチェックする
    /// </summary>
    /// <param name="endPos">タイルの行き先</param>
    /// <returns>移動できる → true  移動できぬ → false</returns>
    public bool CanTileMove(Vector2Int endPos)
    {
        if (!BoardController.instance.IsInBounds(endPos))
        {
            return false;
        }
        if (!BoardController.instance.IsPosEmpty(endPos))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    ///タイルを移動する 
    /// </summary>
    /// <param name="movedir">X,Y移動距離（「+」は右、上　「-」は左、下）</param>
    public void MoveTile(Vector2Int movedir)
    {
        Vector2Int endPos = coordinates + movedir;
        UpdatePosition(endPos);
    }

    /// <summary>
    /// タイルのgameObjectのデータ更新
    /// </summary>
    /// <param name="newPos"></param>
    public void UpdatePosition(Vector2Int newPos)
    {
        coordinates = newPos;
        Vector3 newV3Pos = new Vector3(newPos.x, newPos.y,-1);
        gameObject.transform.position = newV3Pos;
        if (gameObject.transform.position.y >= BoardController.instance.gridSizeY)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    /// <summary>
    /// タイルを今の位置にセットする
    /// </summary>
    /// <returns>タイルがマップ内にセットした　→　true、
    /// 　　　　　タイルがマップの上にいた　→　false　且つ　GAME OVER</returns>
    public bool SetTile()
    {
        if (coordinates.y >= BoardController.instance.gridSizeY)
        {
            return false;
        }

        BoardController.instance.OccupyPos(coordinates, gameObject);
        return true;
    }

    /// <summary>
    /// タイルを90°回す
    /// </summary>
    /// <param name="originPos">このタイルの親テトリスピースの中心タイル</param>
    /// <param name="clockwise">true →　時計回り　false　→　反時計回り</param>
    public void RotateTile(Vector2Int originPos,bool clockwise)
    {
        Vector2Int relativePos = coordinates - originPos;

        /*
         * 回転行列
         * 
         *  |cosθ, -sinθ|
         *  |sinθ,  cosθ|
         * 
         * θは反時計回り角度
         * 
         * ここのVector2Int[]  rotationMatrix の中身は
         *  |rotationMatrix[0].x    rotationMatrix[1].x|
         *  |rotationMatrix[0].y    rotationMatrix[1].y|
         *  
         * 時計回り(-90°)
         *  | 0  1|     
         *  |-1  0|      
         * 
         * 反時計回り(90°)
         *  |0  -1|
         *  |1   0|
         * 
         * かける
         *  |relativePos.x|
         *  |relativePos.y|
         */
        Vector2Int[] rotationMatrix = 
            clockwise ? new Vector2Int[2] { new Vector2Int(0, -1), new Vector2Int(1, 0) }
                    　: new Vector2Int[2] { new Vector2Int(0, 1), new Vector2Int(-1, 0) };

        int newXPos = (rotationMatrix[0].x * relativePos.x) + (rotationMatrix[1].x * relativePos.y);
        int newYPos = (rotationMatrix[0].y * relativePos.x) + (rotationMatrix[1].y * relativePos.y);
        Vector2Int newRelativePos = new Vector2Int(newXPos, newYPos);
        Vector2Int newPos = originPos + newRelativePos;

        UpdatePosition(newPos);

    }

}
