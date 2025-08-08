using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Broadcast : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Transform textParent;
    public Text broadText;

    public static Broadcast instance;

    Color32 color1= new Color32(255, 217, 59, 255);
    Color32 color2= new Color32(59, 255, 139, 255);

    private int count = 0;

    private void Awake()
    {
        instance = this;
    }

    public void BroadCastNews(string news,Color32 color)
    {
        count++;
        Text tmp= Instantiate(broadText, textParent);
        tmp.text = news;
        tmp.color = color;

        Canvas.ForceUpdateCanvases(); // 立即刷新布局
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
