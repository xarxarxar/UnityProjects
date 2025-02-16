using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTipManager : MonoBehaviour
{
    public static ShowTipManager instance;
    public Transform tipParent;
    public Tip tip;

    public GameObject dustbinGameobject;

    private Queue<string> tipTexts = new Queue<string>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 启动协程
        Coroutine cor= StartCoroutine(InstantiateObjects());
    }

    public void ShowTip(string showText)
    {
        tipTexts.Enqueue(showText);
    }

    public void ToggleDustbin(bool isShow)
    {
        dustbinGameobject.SetActive(isShow);
    }

    public void DustbinRed()
    {
        dustbinGameobject.transform.GetChild(0).GetComponent<Image>().color = new Color32(139, 0, 0, 255);
        Text text = dustbinGameobject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = "松开移除卡牌";
        text.color = new Color32(255, 255, 165, 255);
    }

    public void DustbinWhite()
    {
        dustbinGameobject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        Text text = dustbinGameobject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = "拖到此处移除";
        text.color = new Color32(139, 0, 0, 255);
    }

    // 每隔 1 秒实例化一个物体并移除队列
    private IEnumerator InstantiateObjects()
    {
        float delay = 0.2f;
        while (true)
        {
            // 如果队列有物体
            if (tipTexts.Count > 0)
            {
                // 实例化队列中的第一个物体
                string firstText = tipTexts.Dequeue();
                 Instantiate(tip, tipParent).GetComponent<Tip>().showString = firstText;
                delay = tip.duration;
            }
            else
            {
                delay = 0.2f;
            }

            // 等待 1 秒
            yield return new WaitForSeconds(delay);
        }
    }
}
