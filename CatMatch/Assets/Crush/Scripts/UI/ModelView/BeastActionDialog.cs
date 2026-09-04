using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BeastActionDialog : MonoBehaviour
{
    public static BeastActionDialog Instance;

    public Text txt1;
    public Text txt2;

    public Action action1, action2;
    private bool mouseIsOver = false;

    public void SetUp(bool hide1, string text1, string text2, Action action1, Action action2)
    {
        Instance = this;

        txt1.text = text1;
        txt2.text = text2;

        txt1.transform.parent.GetComponent<Button>().interactable = hide1;

        this.action1 = action1;
        this.action2 = action2;
    }

    public void Action1()
    {
        this.action1?.Invoke();
        transform.parent.gameObject.SetActive(false);
    }

    public void Action2()
    {
        this.action2?.Invoke();
        transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("OnDeselect");
        if (!mouseIsOver) gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
        Debug.Log("OnPointerEnter");

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOver = false;
        Debug.Log("OnPointerExit");

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseIsOver = true;
        Debug.Log("OnPointerDown");

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseIsOver = false;
        Debug.Log("OnPointerUp");

        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
