using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public GameObject TopTile;
    public GameObject MiddleTile;
    public GameObject BottomTile;

    //坐标问题
    //public List<Vector3> MainBound;//主圈内的边界
    private float TileHeight = 2;//主地区每个Tile的高度

    public int TileCount = 3;
    private Vector3 startPos=new Vector3(-1,5,0);
    public  Transform TileParent;
    public int CountPerTile = 5;//每块地皮待几只动物

    private void Awake()
    {
        Instance = this;
        //UpdateTile();
    }

    private void Start()
    {
        UpdateTile();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U)) 
        {
            DestroyAllTile();
            TileCount++;
            UpdateTile();
        }
    }

    private void UpdateTile()
    {
        RoomManager.instance.MainRoom.Bounds.Clear();
        RoomManager.instance.MainRoom.Bounds.Add(new Vector3(startPos.x - 2, startPos.y , startPos.z));//左上角
        RoomManager.instance.MainRoom.Bounds.Add(new Vector3(startPos.x + 2, startPos.y , startPos.z));//右上角
        RoomManager.instance.MainRoom.Bounds.Add(new Vector3(startPos.x + 2, startPos.y -(TileCount-1) * TileHeight, startPos.z));//左下角
        RoomManager.instance.MainRoom.Bounds.Add(new Vector3(startPos.x - 2, startPos.y -(TileCount-1) * TileHeight, startPos.z));//左下角
        Debug.Log($"四个角分别为{RoomManager.instance.MainRoom.Bounds[0]}，{RoomManager.instance.MainRoom.Bounds[1]}，{RoomManager.instance.MainRoom.Bounds[2]}，{RoomManager.instance.MainRoom.Bounds[3]}");
        for (int i = 0; i < TileCount; i++)
        {
            Vector3 pos = new Vector3(startPos.x, startPos.y - i* TileHeight, startPos.z);
            if (i==0)
            {
                Instantiate(TopTile,pos,Quaternion.identity, TileParent);
                continue;
            }
            if(i== TileCount - 1)
            {
                Instantiate(BottomTile, pos, Quaternion.identity, TileParent);
                continue;
            }
            Instantiate(MiddleTile, pos, Quaternion.identity, TileParent);
        }
    }

    private void DestroyAllTile()
    {
        for(int i = TileCount-1; i >=0; i--)
        {
            Destroy(TileParent.GetChild(i).gameObject);
        }
    }
}
