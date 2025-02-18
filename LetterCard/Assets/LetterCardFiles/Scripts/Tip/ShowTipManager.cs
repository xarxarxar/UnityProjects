using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowTipManager : MonoBehaviour
{
    public static ShowTipManager instance;
    public Transform tipParent;
    public Tip tip;

    public GameObject dustbinGameobject;

    private Queue<(string,UnityAction)> tipTexts = new Queue<(string, UnityAction)>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 启动协程
        Coroutine cor= StartCoroutine(InstantiateObjects());
    }

    public void ShowTip(string showText,UnityAction callback=null)
    {
        if (tipTexts.Count != 0&&tipTexts.Last().Item1 == showText) return;
        tipTexts.Enqueue((showText,callback));
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
                (string,UnityAction) firstText = tipTexts.Dequeue();
                Instantiate(tip, tipParent).GetComponent<Tip>().showString = firstText.Item1;
                delay = tip.duration;
                AudioManager.instance.PlaySoundEffect("GetScore");
                // 等待 1 秒
                yield return new WaitForSeconds(delay);
                firstText.Item2?.Invoke();
            }
            else
            {
                delay = 0.2f;
                // 等待 1 秒
                yield return new WaitForSeconds(delay);
            }

            
        }
    }
}
