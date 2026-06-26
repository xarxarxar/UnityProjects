using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战报滚动列表管理器。
/// </summary>
public class Broadcast : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Transform textParent;
    public Text broadText;

    public static Broadcast instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 添加一条战报并滚动到底部。
    /// </summary>
    /// <param name="news">战报文本内容。</param>
    /// <param name="color">战报文本颜色。</param>
    public void BroadCastNews(string news, Color32 color)
    {
        CreateNewsText(news, color);
        RefreshScrollToBottom();
    }

    /// <summary>
    /// 创建并填充一条战报文本。
    /// </summary>
    /// <param name="news">战报文本内容。</param>
    /// <param name="color">战报文本颜色。</param>
    private void CreateNewsText(string news, Color32 color)
    {
        Text tmp = Instantiate(broadText, textParent);
        tmp.text = news;
        tmp.color = color;
    }

    /// <summary>
    /// 立即刷新布局并滚动到最新战报。
    /// </summary>
    private void RefreshScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
