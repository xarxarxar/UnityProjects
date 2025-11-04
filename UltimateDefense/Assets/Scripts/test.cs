using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;




public class test : MonoBehaviour
{
    private string testUserId = "user001";

    void Start()
    {
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            TipManager.Instance.ShowTip("你好你好你好你好你好你好你好你");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TipManager.Instance.ShowConfirmTip("你好");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            TipManager.Instance.ShowChooseTip("你好", onCancel: () =>
            {
                Debug.Log("点击了取消按钮");
            },
            onConfirm: () =>
            {
                Debug.Log("点击了确认按钮");
            });
        }
    }


}

