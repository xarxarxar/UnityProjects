
/// <summary>
/// 处理元素的单击，拖动等操作
/// </summary>
public interface IDraggable
{
    void OnClick();             // 单击事件
    void OnDoubleClick();       // 双击事件
    void OnDragStart();         // 开始拖动
    void OnDragging();          // 拖动中
    void OnDragEnd();           // 停止拖动
}

