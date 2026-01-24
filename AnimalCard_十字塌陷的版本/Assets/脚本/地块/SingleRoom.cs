using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SingleRoom : MonoBehaviour
{
    public int roomId;//主区域的ID为-1
    public List<Vector3> Bounds;
    public bool IsUnlock=false;
    private SpriteRenderer SpriteRenderer;
    public Vector3 Pos;//该房间的位置
    public int MaxAnimalCount;//该房间最多待几只动物

    /// <summary>
    /// 初始化房间
    /// </summary>
    /// <param name="id"></param>
    /// <param name="unlocked"></param>
    public void Init(int id,bool unlocked,Vector3 pos)
    {
        if (SpriteRenderer == null)
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
        roomId=id;
        IsUnlock = unlocked;
        Pos=pos;
        Bounds.Clear();
        Bounds.Add(new Vector3(pos.x - 1, pos.y+1, pos.z));//左上角
        Bounds.Add(new Vector3(pos.x + 1, pos.y+1, pos.z));//右上角
        Bounds.Add(new Vector3(pos.x + 1, pos.y - 1, pos.z));//左下角
        Bounds.Add(new Vector3(pos.x - 1, pos.y - 1, pos.z));//左下角
        MaxAnimalCount=3;
        ChangeLockState(IsUnlock);
    }

    /// <summary>
    /// 改变房间的解锁状态
    /// </summary>
    /// <param name="unlocked"></param>
    public void ChangeLockState(bool unlocked)
    {
        SpriteRenderer.color= unlocked?Color.white:Color.black;
    }
    /// <summary>
    ///  修改该房间最多待几只动物
    /// </summary>
    /// <param name="count"></param>
    public void ChangeRoomMaxAnimalCount(int count)
    {
        MaxAnimalCount= count;
    }
}
