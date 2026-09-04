using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelRetreat : MonoBehaviour
{
    public Text label;
    public Text content;
    
    void Enable()
    {
        label.text = LangManager.Instance.Get("Retreat");
        content.text = LangManager.Instance.Get("ContentRetreat");
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void No()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void Yes()
    {
        Time.timeScale = 1f;
        GameManager.instance.GameOver(false);
        gameObject.SetActive(false);
    }
}
