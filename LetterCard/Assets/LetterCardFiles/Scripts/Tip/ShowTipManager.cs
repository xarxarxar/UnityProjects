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



}
