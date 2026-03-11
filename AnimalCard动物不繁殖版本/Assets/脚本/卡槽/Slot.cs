using UnityEditor;
using UnityEngine;

/// <summary>
/// 棋盘中的逻辑格子（Slot）
/// 负责：位置 + 状态
/// 不负责：消除 / 下落 / 匹配
/// </summary>
public class Slot : MonoBehaviour
{
    [Header("卡槽的位置")]
    [SerializeField] private int row;
    [SerializeField] private int col;
    /// <summary>
    /// 行（逻辑坐标）
    /// 建议：row = 0 表示最底部
    /// </summary>
    public int Row => row;

    /// <summary>
    /// 列（逻辑坐标）
    /// </summary>
    public int Col => col;
    [Header("是否被占用")]
    [SerializeField] private bool isOccupied;
    /// <summary>
    /// 当前是否被卡牌占用（仅作状态缓存）
    /// 实际是否为空，仍以 BoardManager 的 board[row,col] 为准
    /// </summary>
    public bool IsOccupied => isOccupied;

    [Header("Refs")]
    [Tooltip("用于拖拽 / 检测用（可选）")]
    public Collider2D slotCollider;

    /// <summary>
    /// 该 Slot 在世界坐标中的位置
    /// （只是表现层，不参与逻辑判断）
    /// </summary>
    public Vector3 WorldPosition => transform.position;


    private void Awake()
    {
        if(slotCollider == null)
        {
            slotCollider = GetComponent<Collider2D>();
        }
    }

    /// <summary>
    /// 初始化 Slot 的逻辑坐标
    /// 通常由 SlotManager / BoardLayout 调用
    /// </summary>
    public void Init(int row, int col)
    {
        this.row = row;
        this.col = col;
        isOccupied = false;
    }

    #region State Control

    public void SetOccupied()
    {
        isOccupied = true;
    }

    public void SetUnOccupied()
    {
        isOccupied = false;
    }

    public void ResetSlot()
    {
        isOccupied = false;
        PoolManager.Instance.SlotPool.Return(this);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 绘制方框
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);

        // 创建自定义样式
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black; // 改成你想要的颜色
        style.fontSize = 12; // 可以调节字体大小
        style.alignment = TextAnchor.MiddleCenter;

        // 绘制行列标签
        Handles.Label(transform.position, $"({row},{col})", style);
    }
#endif

}
