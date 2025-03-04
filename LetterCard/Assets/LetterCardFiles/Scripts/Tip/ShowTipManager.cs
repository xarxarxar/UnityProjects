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
    public Tip tipWithMoney;
    public Tip tipNomoney;

    public GameObject dustbinGameobject;

    private Queue<(string,UnityAction)> tipTexts = new Queue<(string, UnityAction)>();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowTip("ÄãºÃ");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ShowTip("ÄãºÃ",10);
        }
    }


    public void ShowTip(string showText,UnityAction callback=null)
    {
        Instantiate(tipNomoney, tipParent).GetComponent<Tip>().showString = showText;
        callback?.Invoke();
    }

    public void ShowTip(string showText,int count, UnityAction callback = null)
    {
        Tip tipGameobject= Instantiate(tipWithMoney, tipParent).GetComponent<Tip>();
        tipGameobject.showString = showText;
        tipGameobject.countNumber = count;
        callback?.Invoke();
    }

    public void ToggleDustbin(bool isShow)
    {
        dustbinGameobject.SetActive(isShow);
    }

    public void DustbinRed()
    {
        dustbinGameobject.transform.GetChild(0).GetComponent<Image>().color = new Color32(139, 0, 0, 255);
        Text text = dustbinGameobject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = "ËÉ¿ªÒÆ³ý¿¨ÅÆ";
        text.color = new Color32(255, 255, 165, 255);
    }

    public void DustbinWhite()
    {
        dustbinGameobject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        Text text = dustbinGameobject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text.text = "ÍÏµ½´Ë´¦ÒÆ³ý";
        text.color = new Color32(139, 0, 0, 255);
    }

}
