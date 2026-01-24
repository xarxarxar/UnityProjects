using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    //index=0为主房间，index最后一个为未解锁的房间，该列表的数量应该为RoomCount+2
    public List<SingleRoom> AllRooms = new List<SingleRoom>();

    public int RoomCount = 1;//这个数字仅包括已解锁的小房间，不包括主区域和未解锁的小房间，
    public SingleRoom RoomPrefab;
    public Transform RoomParent;
    private Vector3 StartPos=new Vector3(2.5f,4.5f,0);
    private float RoomHeight = 2;

    public int MainRoomMaxAnimalCount =>TileManager.Instance.TileCount*TileManager.Instance.CountPerTile;//主区域最多几只动物，暂时

    //主房间
    public  SingleRoom MainRoom => AllRooms[0];

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateRoom();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyAllTile();
            RoomCount++;
            UpdateRoom();
        }
    }

    /// <summary>
    /// 根据id获取一个房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleRoom GetRoom(int id)
    {
        for (int i = 0; i < AllRooms.Count; i++)
        {
            if (AllRooms[i].roomId == id)
            {
                return AllRooms[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取当前房间最多待几只动物
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetRoomAnimalMaxCount(int id)
    {
        return id == -1 ? MainRoomMaxAnimalCount : 3;
    }

    /// <summary>
    /// 获取当前房间有几只动物
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetRoomAnimalCount(int id)
    {
        int count = 0;
        for(int i = 0;i<AnimalManager.Instance.AllAnimals.Count;i++)
        {
            if(AnimalManager.Instance.AllAnimals[i].RoomID== id)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// 该房间是否满了
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsRoomFull(int id)
    {
        return GetRoomAnimalCount(id) >= GetRoomAnimalMaxCount(id);
    }

    /// <summary>
    /// 判断某个点是否在某个房间内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="roomID"></param>
    /// <returns></returns>
    public bool IsInRoom(Vector3 pos,out int roomID)
    {
        if (IsPointInMainRoom(pos))
        {
            roomID = -1;
            return true;
        }
        
        for (int i = 0;i< AllRooms.Count;i++)
        {
            if(i==0) continue;//跳过第一个，也就是主区域
            Collider2D collider2D = AllRooms[i].GetComponent<Collider2D>();
            if (collider2D==null) continue;
            if (collider2D.OverlapPoint(pos))
            {
                roomID = AllRooms[i].roomId;
                return true;
            }
        }
        roomID = -2;
        return false;
    }

    //更新所有房间的显示
    private void UpdateRoom()
    {
        for (int i = 0; i < RoomCount+1; i++)
        {
            Vector3 pos = new Vector3(StartPos.x,StartPos.y- RoomHeight * i,StartPos.z);
            SingleRoom singleRoom= Instantiate(RoomPrefab, pos, Quaternion.identity, RoomParent);
            singleRoom.Init(i,i!= RoomCount, pos);//最后一个是未解锁的
            AllRooms.Add(singleRoom);
        }
        AllRooms[0].ChangeRoomMaxAnimalCount(MainRoomMaxAnimalCount);
    }
    //摧毁所有的小房间，不包括主房间
    private void DestroyAllTile()
    {
        for (int i = RoomParent.childCount-1; i >= 0; i--)
        {
            AllRooms.Remove(RoomParent.GetChild(i).GetComponent<SingleRoom>());
            Destroy(RoomParent.GetChild(i).gameObject);
        }
    }

    //某个点是否在主区域
    private  bool IsPointInMainRoom(Vector3 worldPoint)
    {
        foreach (Transform child in TileManager.Instance.TileParent)
        {
            Collider2D col = child.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(worldPoint))
                return true;
        }
        return false;
    }
}
