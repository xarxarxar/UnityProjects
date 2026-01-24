using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public TouchHandler2D _touchHandler;

    private void Start()
    {
        _touchHandler.OnClick += () => { Debug.Log("测试单击"); };
        _touchHandler.OnDoubleClick += () => { Debug.Log("测试双击"); };
        _touchHandler.OnLongPressStart += () => { Debug.Log("测试开始长按"); };
        _touchHandler.OnLongPressStay += () => { Debug.Log("测试长按中"); };
        _touchHandler.OnLongPressEnd += () => { Debug.Log("测试结束长按"); };
        _touchHandler.OnDragBegin += (eventData) => { Debug.Log("测试开始拖拽"); };
        _touchHandler.OnDragging += (eventData) => { Debug.Log("测试拖拽中"); };
        _touchHandler.OnDragEnd += (eventData) => { Debug.Log("测试拖拽结束"); };


    }
}
