using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public List<Card> upCards=new List<Card>();
    public List<Card> downCards=new List<Card>();
    public int level = 0;
    public bool isclickable = true;
    public bool isUsed=false;
    public int type;
    public event Action<Card> clicked;

    public bool Isclickable {
        get => isclickable;
        set
        {
            if (isclickable != value)
            {
                isclickable = value;
                ChangeColor(isclickable);
            }
        }
    }
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    void ChangeColor(bool isclicked)
    {
        this.GetComponent<Image>().color = isclicked ? Color.green:Color.red;
    }

    public void Click()
    {
        if (!isclickable) return;
        isUsed = true;
        //gameObject.SetActive(false);
        for (int i = 0; i < downCards.Count; i++)
        {
            CardManager.instance.IsClickable(downCards[i]);
        }
        CardMove.Instance.MoveCard(this);
        
    }
}
