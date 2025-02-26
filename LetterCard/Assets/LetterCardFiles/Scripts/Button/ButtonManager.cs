using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;
    private void Awake()
    {
        instance = this;
    }

    //按钮
    public Button drawCardButton;//抽牌按钮
    public Button playCardButton;//出牌按钮
}
