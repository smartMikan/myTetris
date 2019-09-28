using UnityEngine;
/// <summary>
/// Grid単位のクラス
/// </summary>
public class GridUnit
{
    //Grid単位のgameobject
    public GameObject gameObject { get; private set; }
    //今このGrid上にいるテトリスブロック
    public GameObject tileOnThisGrid;
    //Gridの位置
    public Vector2Int location { get; private set; }
    //占拠されているか
    public bool isOccupied;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="newGameObject">Instantiate化のPrefab</param>
    /// <param name="boardParent">BoardObject</param>
    /// <param name="x">横位置（左から右に）</param>
    /// <param name="y">縦位置（下から上に）</param>
    public GridUnit(GameObject newGameObject, Transform boardParent, int x, int y)
    {
        gameObject = GameObject.Instantiate(newGameObject, boardParent);
        location = new Vector2Int(x, y);
        isOccupied = false;

        gameObject.transform.position = new Vector3(location.x, location.y);
    }
}
